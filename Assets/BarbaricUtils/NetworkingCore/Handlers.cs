using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;

namespace BarbaricCode {
    namespace Networking {

        public class HandlerClass : Attribute { }

        public class NetEngineHandler : Attribute {
            public NetEngineEvent type;
            public NetEngineHandler(NetEngineEvent type)
            {
                this.type = type;
            }
        }

        public enum NetEngineEvent
        {
            NewConnection,
            ConnectionFailed,
            Hosted,
            HostFailed,
            HostDisconnect,
			Timeout,
			FlowControl
        }

        public class NetHandle : Attribute
        {
            public NetworkEventType NetType;
            public NetHandle(NetworkEventType type) {
                this.NetType = type;
            }
        }

        public class SegHandle : Attribute {
            public MessageType type;
            public SegHandle(MessageType type) {
                this.type = type;
            }
        }

		public class ErrorHandle: Attribute {
			public NetworkError type;
			public ErrorHandle(NetworkError type) {
				this.type = type;
			}
		}

        public static partial class Handlers {
            // NetworkEventHandlers
            [NetHandle(NetworkEventType.ConnectEvent)]
            public static void HandleConnect(int connectionID, byte[] buffer, int recievedSize) {
                string address; int port; UnityEngine.Networking.Types.NetworkID network; UnityEngine.Networking.Types.NodeID NodeID; byte error; 
                NetworkTransport.GetConnectionInfo(NetEngine.SocketID, connectionID, out address, out port, out network, out NodeID, out error);
                Debug.Log("Recieving connection " + NetEngine.SocketID + "|" + connectionID + "|" + address + ":" + port);

                if (NetEngine.Connections.ContainsKey(connectionID)) {
                    Debug.LogWarning("Already has connection: " + connectionID);
                    return;
                }

				Connection conn = new Connection(connectionID, NetEngine.SocketID, NetEngine.NextNodeID, port, address);
                NetEngine.Connections.Add(connectionID, conn);
                SegmentHeader seghead;
                seghead.type = MessageType.ESTABLISH_CONNECTION;
                EstablishConnectionMessage estab;
                estab.SegHead = seghead;
                estab.newID = NetEngine.NextNodeID;
                NetEngine.NextNodeID++;
                conn.QSendTCP(NetworkSerializer.GetBytes<EstablishConnectionMessage>(estab), PacketUtils.MessageToStructSize[seghead.type]);
                if (NetEngine.IsServer) {
                    NetEngine.NotifyListeners(NetEngineEvent.NewConnection, estab.newID, connectionID, buffer, recievedSize);
                }
            }

            [NetHandle(NetworkEventType.DataEvent)]
            public static void HandleData(int connectionID, byte[] buffer, int recieveSize) {
                if (!NetEngine.SocketOpen) {
                    Debug.LogWarning("Can't handle data if socket is not open");
                    return;
                }

                byte[] readbuff = new byte[NetworkMessage.MaxMessageSize];
                MemoryStream stream = new MemoryStream(buffer);
                stream.Seek(0, 0);
                stream.Read(readbuff, 0, PacketUtils.MESSAGE_HEADER);
                MessageHeader mshead = NetworkSerializer.GetStruct<MessageHeader>(readbuff);

                // Debug.Log("Recieved message from " + mshead.nodeSource + " " + mshead.size);

                while (stream.Position < mshead.size) {
                    stream.Read(readbuff, 0, PacketUtils.SEGMENT_HEADER);
                    SegmentHeader seghead = NetworkSerializer.GetStruct<SegmentHeader>(readbuff);
                    int size = PacketUtils.MessageToStructSize[seghead.type];
                    stream.Seek(-PacketUtils.SEGMENT_HEADER, SeekOrigin.Current);
                    stream.Read(readbuff, 0, size);

                    // special case for state_data. requires one
                    // more layer of indirection
                    if (seghead.type == MessageType.STATE_DATA)
                    {
                        HandleState(mshead.nodeSource, connectionID, readbuff, stream, size);
                        continue;
                    }
                    else if (seghead.type == MessageType.USER_DATA) {
                        HandleUserData(mshead.nodeSource, connectionID, readbuff, stream, size);
                        continue;
                    }
                    else if (!NetEngine.segmentHandlers.ContainsKey(seghead.type))
                    {
                        Debug.LogError("Unhandled Segment Type " + seghead.type.ToString());
                        continue;
                    }

                    NetEngine.segmentHandlers[seghead.type].Invoke(mshead.nodeSource, connectionID, readbuff, size);
                }
                // do stuff
            }

            [NetHandle(NetworkEventType.DisconnectEvent)]
            public static void Disconnect(int connectionID, byte[] buffer, int recieveSize) {
                if (NetEngine.Connections.ContainsKey(connectionID))
                {
                    Debug.Log("Disconnected from: " + connectionID);
                    NetEngine.Connections.Remove(connectionID);
                }
                else {
                    Debug.LogWarning("Disconnection connectionID: " + connectionID + " does not exist");
                }
            }

			// Network error handlers
			[ErrorHandle(NetworkError.Timeout)]
			public static void HandleTimeout(int connectionID, byte[] buffer, int recievedSize) {
				NetEngine.NotifyListeners (NetEngineEvent.Timeout, 0, connectionID, buffer, recievedSize);
				NetEngine.Disconnect (connectionID);
			}

            // NetworkSegmentHandlers
            [SegHandle(MessageType.ESTABLISH_CONNECTION)]
            public static void EstablishConnection(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                EstablishConnectionMessage ec = NetworkSerializer.GetStruct<EstablishConnectionMessage>(buffer);
                if (nodeID == 0)
                {
                    if (NetEngine.NodeId == 0)
                    {
                        Debug.LogWarning("Host Collision, 2 hosts are connecting. Disconnecting");
                        NetEngine.Disconnect(connectionID);
                        return;
                    }
                    // from server
                    NetEngine.NodeId = ec.newID;
                    Debug.Log("Set NodeID To: " + NetEngine.NodeId);
                    // @FLAG SET
                    NetEngine.State = EngineState.CONNECTED;
                    NetEngine.NotifyListeners(NetEngineEvent.NewConnection, nodeID, connectionID, buffer, recieveSize);
					NetEngine.Connections [connectionID].nodeID = 0; // Client set node id to 0 since host

                    // @TODO REMOVE DEBUG HELLO WORLD
                    SegmentHeader seghead;
                    seghead.type = MessageType.USER_DATA;
                    UserDataHeader udh;
                    udh.MessageId = (int)UserHandlers.NetEngineUserDataTypes.HELLO_WORLD;
                    udh.MessageSize = 0;
                    udh.SegHead = seghead;
                    HelloWorldPacket hwp;
                    hwp.secret = 42;

                    byte[] b2 = NetworkSerializer.GetBytes<HelloWorldPacket>(hwp);
                    udh.MessageSize = b2.Length;
                    byte[] b1 = NetworkSerializer.GetBytes<UserDataHeader>(udh);
                    byte[] b3 = NetworkSerializer.Combine(b1, b2);
                    NetEngine.SendTCP(b3, b3.Length, connectionID);

				}
                else {
                    if (NetEngine.NodeId != 0)
                    {
                        Debug.Log("Two clients connected, unhandled case");
                        // @TODO
                    }
                    // otherwise ignore
                }
            }

            [SegHandle(MessageType.SPAWN)]
            public static void Spawn(int nodeID, int connectionID, byte[] buffer, int recieveSize) {
                SpawnMessage sm = NetworkSerializer.GetStruct<SpawnMessage>(buffer);
                if (NetEngine.IsServer)
                {

                    if (!NetEngine.SpawnablePrefabs.ContainsKey(sm.PrefabID)) {
                        Debug.LogWarning("Cannot spawn prefab, no id");
                        return;
                    }

                    sm.NetID = NetEngine.NextNetID;
                    NetEngine.NextNetID++;

                    // send to all
                    foreach (Connection conn in NetEngine.Connections.Values) {
                        conn.QSendTCP(NetworkSerializer.GetBytes<SpawnMessage>(sm), PacketUtils.MessageToStructSize[MessageType.SPAWN]);
                    }
                }

                // only accept spawn commands from server
                // If we are the server then we'll want to accept spawn commands
                // from clients as well.
                if (nodeID == 0 || NetEngine.IsServer) {
                    // create the go go
                    GameObject go = GameObject.Instantiate(NetEngine.SpawnablePrefabs[sm.PrefabID]);
                    go.transform.position = sm.Position;
                    go.transform.rotation = sm.Rotation;
                    StateSynchronizableMonoBehaviour ssmb = go.GetComponent<StateSynchronizableMonoBehaviour>();
                    ssmb.AuthNodeID = sm.AuthorityID;
                    ssmb.NetID = sm.NetID;
                    ssmb.PrefabID = sm.PrefabID;

                    // init to prevent state synch source vars from being undefined
                    ssmb.Init();

                    if (NetEngine.NodeId == sm.AuthorityID)
                    {
                        NetEngine.LocalAuthorityObjects.Add(ssmb.NetID, ssmb);
                        ssmb.LocalAuthority = true;
                    }
                    NetEngine.NetworkObjects.Add(ssmb.NetID, ssmb);
                    // @TODO Connection To Local Auth list
                }
            }

            [SegHandle(MessageType.DESPAWN)]
            public static void Despawn(int nodeID, int connectionID, byte[] buffer, int recieveSize) {
                DespawnMessage dm = NetworkSerializer.GetStruct<DespawnMessage>(buffer);
                // from server, then okay to trust
                if (nodeID == 0) {
                    StateSynchronizableMonoBehaviour ssmb = null;
                    if (NetEngine.NetworkObjects.ContainsKey(dm.netid))
                    {
                        ssmb = NetEngine.NetworkObjects[dm.netid];
                        NetEngine.NetworkObjects.Remove(dm.netid);
                    }
                    if (NetEngine.LocalAuthorityObjects.ContainsKey(dm.netid))
                    {
                        ssmb = NetEngine.LocalAuthorityObjects[dm.netid];
                        NetEngine.LocalAuthorityObjects.Remove(dm.netid);
                    }
                    if (ssmb != null)
                    {
                        ssmb.OnDespawn();
                        GameObject.Destroy(ssmb.gameObject);
                    }
                }
                if (NetEngine.IsServer) {
                    foreach (Connection con in NetEngine.Connections.Values) {
                        con.QSendTCP(buffer, PacketUtils.MessageToStructSize[MessageType.DESPAWN]);
                    }
                    // local despawn handler
                    StateSynchronizableMonoBehaviour ssmb = null;
                    if (NetEngine.NetworkObjects.ContainsKey(dm.netid))
                    {
                        ssmb = NetEngine.NetworkObjects[dm.netid];
                        NetEngine.NetworkObjects.Remove(dm.netid);
                    }
                    if (NetEngine.LocalAuthorityObjects.ContainsKey(dm.netid))
                    {
                        ssmb = NetEngine.LocalAuthorityObjects[dm.netid];
                        NetEngine.LocalAuthorityObjects.Remove(dm.netid);
                    }
                    if (ssmb != null)
                    {
                        ssmb.OnDespawn();
                        GameObject.Destroy(ssmb.gameObject);
                    }
                }
            }

			[SegHandle(MessageType.FLOW_CONTROL)]
			public static void HandleFlow(int nodeID, int connectionID, byte[] buffer, int recieveSize){
				NetEngine.NotifyListeners (NetEngineEvent.FlowControl, nodeID, connectionID, buffer, recieveSize);
			}

            // special handlers
            public static void HandleState(int nodeID, int connectionID, byte[] buffer, Stream stream, int recieveSize)
            {
                StateDataMessage sdm = NetworkSerializer.GetStruct<StateDataMessage>(buffer);
                byte[] readbuf = new byte[NetworkMessage.MaxMessageSize];
                stream.Read(readbuf, 0, sdm.StateSize);

                if (!NetEngine.NetworkObjects.ContainsKey(sdm.NetID))
                {
                    Debug.LogWarning("Network object does not exist");
                    return;
                }
                NetEngine.NetworkObjects[sdm.NetID].Synchronize(readbuf, sdm.TimeStep);
            }

            public static void HandleUserData(int nodeID, int connectionID, byte[] buffer, Stream stream, int recieveSize)
            {
                
                UserDataHeader udh = NetworkSerializer.GetStruct<UserDataHeader>(buffer);
                byte[] readbuf = new byte[NetworkMessage.MaxMessageSize];
                stream.Read(readbuf, 0, udh.MessageSize);
                if (!NetEngine.userHandlers.ContainsKey(udh.MessageId)) {
                    Debug.LogError("No user defined Handler for user data message id: " + udh.MessageId);
                    return;
                }
                NetEngine.userHandlers[udh.MessageId].Invoke(nodeID, connectionID, readbuf, udh.MessageSize);
            }
        }
    }
}