using System;   
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace BarbaricCode
{
    namespace Networking
    {
        public enum EngineState {
            DISCONNECTED,
            CONNECTING,
            CONNECTED
        }

        // single socket
        public static class NetEngine
        {

            public delegate void NetworkEventHandler(int connectionID, byte[] buffer, int recievedSize);
            public delegate void SegmentHandler(int nodeID, int connectionID, byte[] buffer, int recievedSize);

            public static ConnectionConfig cconfig;
            public static HostTopology topology;

            public static int NodeId = -1;
            public static int SimStep = 0;
            public static int SocketID = -1;
            public static int NextNodeID = 1;
            public static int NextNetID = 0;
            public static bool SocketOpen = false;
            public static bool IsServer = false;
            public static Dictionary<int, Connection> Connections = new Dictionary<int, Connection>();
            public static EngineState State = EngineState.DISCONNECTED;

            public static Dictionary<int, StateSynchronizableMonoBehaviour> NetworkObjects = new Dictionary<int, StateSynchronizableMonoBehaviour>();
            public static Dictionary<int, StateSynchronizableMonoBehaviour> LocalAuthorityObjects = new Dictionary<int, StateSynchronizableMonoBehaviour>();

            public static int UDPChannel = -1;
            public static int TCPChannel = -1;

            static byte[] buffer = new byte[NetworkMessage.MaxMessageSize];
            public static Dictionary<NetworkEventType, NetworkEventHandler> evtTypeHandlers = new Dictionary<NetworkEventType, NetworkEventHandler>();
            public static Dictionary<MessageType, SegmentHandler> segmentHandlers = new Dictionary<MessageType, SegmentHandler>();
            public static Dictionary<NetEngineEvent, List<SegmentHandler>> netEngineEvtHandlers = new Dictionary<NetEngineEvent, List<SegmentHandler>>();

            public static Dictionary<int, GameObject> SpawnablePrefabs = new Dictionary<int, GameObject>();

            static NetEngine()
            {
                NetworkTransport.Init();
                InitConfig();
                Application.runInBackground = true;
                // grabs all handler methods from Handlers class and 
                // adds them according to attribute
                
                var meths = typeof(Handlers).GetMethods().Where(meth => Attribute.IsDefined(meth, typeof(NetHandle)));
                foreach (System.Reflection.MethodInfo method in meths)
                {
                    NetHandle handler = method.GetCustomAttributes(typeof(NetHandle), true).First() as NetHandle;
                    NetworkEventHandler netHandler = (NetworkEventHandler)Delegate.CreateDelegate(typeof(NetworkEventHandler), method);
                    evtTypeHandlers.Add(handler.NetType, netHandler);
                }
                meths = typeof(Handlers).GetMethods().Where(meth => Attribute.IsDefined(meth, typeof(SegHandle)));
                foreach (System.Reflection.MethodInfo method in meths)
                {
                    SegHandle handler = method.GetCustomAttributes(typeof(SegHandle), true).First() as SegHandle;
                    SegmentHandler segHandler = (SegmentHandler)Delegate.CreateDelegate(typeof(SegmentHandler), method);
                    segmentHandlers.Add(handler.type, segHandler);
                }
            }

            private static void InitConfig() {
                cconfig = new ConnectionConfig();
                UDPChannel = cconfig.AddChannel(QosType.Unreliable);
                TCPChannel = cconfig.AddChannel(QosType.Reliable);
                topology = new HostTopology(cconfig, 64);
            }

            private static void SendStatesToClients() {
                foreach (StateSynchronizableMonoBehaviour mono in NetworkObjects.Values)
                {
                    int size;
                    byte[] State = mono.GetState(out size);
                    foreach (Connection c in Connections.Values) {
                        c.QSendUDP(State, size);
                    }
                }
            }
            private static void SendLocalAuthToServer() {
                foreach (StateSynchronizableMonoBehaviour mono in LocalAuthorityObjects.Values)
                {
                    int size;
                    byte[] State = mono.GetState(out size);
                    foreach (Connection c in Connections.Values)
                    {
                        c.QSendUDP(State, size);
                    }
                }
            }
            // to be called in fixed delta time or in separate thread.
            // will require some queueing mechanism for thread asynch
            public static void Step() {
                int hostID, connectionID, channelID, recievedSize;
                byte error;
                NetworkEventType net;
                while ((net = NetworkTransport.Receive(out hostID, out connectionID, out channelID, buffer, NetworkMessage.MaxMessageSize, out recievedSize, out error))!=NetworkEventType.Nothing) {
                    if ((NetworkError)error != NetworkError.Ok) {
                        Debug.LogError("Failed to recieve: " + ((NetworkError)error).ToString());
                        continue;
                    }
                    if (evtTypeHandlers.ContainsKey(net))
                    {
                        evtTypeHandlers[net].Invoke(connectionID, buffer, recievedSize);
                    }
                    else {
                        Debug.Log("Unhandled network event: " + net.ToString());
                        return;
                    }
                }

                if (IsServer)
                {
                    SendStatesToClients();
                }
                else {
                    SendLocalAuthToServer();
                }

                foreach (Connection c in Connections.Values)
                {
                    c.SendAll();
                }
                SimStep++;
            }
            public static void StartSocket(int port) {
                if (SocketOpen) {
                    Debug.LogWarning("Socket Already Open " + SocketID);
                    return;
                }

                SocketID = NetworkTransport.AddHost(topology, port);

                if (SocketID == -1) {
                    Debug.LogError("Failed to open socket");
                    return;
                }
                SocketOpen = true;
                State = EngineState.DISCONNECTED;
            }
            public static void StartServer(int port) {
                if (SocketOpen) {
                    Debug.LogWarning("Socket Already Open please close socket first " + SocketID);
                    return;
                }
                StartSocket(port);
                if (SocketID == -1) {
                    Debug.LogError("Failed To Start Server");
                    return;
                }
                State = EngineState.CONNECTED;
                NodeId = 0;
                IsServer = true;
            }
            public static void Disconnect(int connectionID) {
                
                // @TODO this func
                byte error;
                NetworkTransport.Disconnect(SocketID, connectionID, out error);
                Connections.Remove(connectionID);
                if ((NetworkError)error != NetworkError.Ok) {
                    Debug.Log("Error disconnecting " + connectionID + " " + ((NetworkError)error).ToString());
                    return;
                }

            }
            public static void Connect(string ipv4, int port) {
                if (!SocketOpen) {
                    Debug.LogWarning("Socket not open");
                }
                byte error;
                NetworkTransport.Connect(SocketID, ipv4, port, 0, out error);
            }
            public static void Spawn(int id)
            {
                SegmentHeader sg;
                sg.type = MessageType.SPAWN;
                SpawnMessage sm;
                sm.AuthorityID = NetEngine.NodeId;
                sm.NetID = -1;
                sm.Position = Vector3.zero;
                sm.Rotation = Quaternion.identity;
                sm.SegHead = sg;
                sm.PrefabID = id;

                if (IsServer)
                {
                    Handlers.Spawn(0, 0, NetworkSerializer.GetBytes<SpawnMessage>(sm), PacketUtils.MessageToStructSize[MessageType.SPAWN]);
                }
                else {
                    foreach (Connection c in Connections.Values) {
                        c.QSendTCP(NetworkSerializer.GetBytes<SpawnMessage>(sm), PacketUtils.MessageToStructSize[MessageType.SPAWN]);
                    }
                }
            }

            public static void BroadcastTCP(byte[] data, int size) {
                foreach (Connection conn in Connections.Values) {
                    conn.QSendTCP(data, size);
                }
            }
            public static void BroadcastUDP(byte[] data, int size) {
                foreach (Connection conn in Connections.Values) {
                    conn.QSendUDP(data, size);
                }
            }
            public static void SendTCP(byte[] data, int size, int connection) {
                if (!Connections.ContainsKey(connection))
                {
                    Debug.LogWarning("Trying to send to non-existent conenction");
                    return;
                }
                Connections[connection].QSendTCP(data, size);
            }
            public static void SendUDP(byte[] data, int size, int connection) {
                if (!Connections.ContainsKey(connection)) {
                    Debug.LogWarning("Trying to send to non-existent conenction");
                    return;
                }
                Connections[connection].QSendUDP(data, size);
            }

        }
    }
}
