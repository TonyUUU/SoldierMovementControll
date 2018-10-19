using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.InteropServices;
namespace BarbaricCode
{
    namespace Networking
    {

        public abstract class StateSynchronizableMonoBehaviour : MonoBehaviour {
            public bool LocalAuthority;
            public int PrefabID;
            public int NetID;
            public int AuthNodeID;
            public abstract void Synchronize(byte[] state);
            public abstract byte[] GetState(out int size);
            public abstract void Init();
            public abstract void OnDespawn();
        }

        public class Connection {
            public int connectionID;
            public int hostID;
			public int nodeID;
			public int port;
            public string ipv4;
            public MemoryStream UDPmemstream;
            public MemoryStream TCPmemstream;
            public List<long> UDPcutoffs;
            public List<long> TCPcutoffs;
            private bool UDPhasdata = false;
            private bool TCPhasdata = false;
            private int UDPpos, TCPpos;

            public List<StateSynchronizableMonoBehaviour> LocalAuthMonos;

			public Connection(int connectionID, int hostID, int nodeID, int port, string ipv4) {
                this.connectionID = connectionID;
                this.hostID = hostID;
                this.port = port;
                this.ipv4 = ipv4;
				this.nodeID = nodeID;
                UDPmemstream = new MemoryStream();
                UDPcutoffs = new List<long>();
                TCPmemstream = new MemoryStream();
                TCPcutoffs = new List<long>();
                UDPmemstream.Seek(PacketUtils.MESSAGE_HEADER, SeekOrigin.Begin);
                TCPmemstream.Seek(PacketUtils.MESSAGE_HEADER, SeekOrigin.Begin);
                UDPpos = PacketUtils.MESSAGE_HEADER;
                TCPpos = PacketUtils.MESSAGE_HEADER;
            }

            public void QSendUDP(byte[] data, int size) {
                if (UDPpos + size > NetworkMessage.MaxMessageSize) {
                    UDPcutoffs.Add(UDPmemstream.Position);
                    UDPmemstream.Seek(PacketUtils.MESSAGE_HEADER, SeekOrigin.Current);
                    UDPpos = PacketUtils.MESSAGE_HEADER;
                }
                UDPmemstream.Write(data, 0, size);
                UDPhasdata = true;
            }

            public void QSendTCP(byte[] data, int size) {
                if (TCPpos + size > NetworkMessage.MaxMessageSize)
                {
                    TCPcutoffs.Add(TCPmemstream.Position);
                    TCPmemstream.Seek(PacketUtils.MESSAGE_HEADER, SeekOrigin.Current);
                    TCPpos = PacketUtils.MESSAGE_HEADER;
                }
                TCPmemstream.Write(data, 0, size);
                TCPhasdata = true;
            }

            public void SendAll() {
                byte[] buffer = new byte[NetworkMessage.MaxMessageSize];
                byte error;
                int UDPend = (int) UDPmemstream.Position;
                int TCPend = (int) TCPmemstream.Position;
                int count = 0;
                UDPmemstream.Seek(0, 0);
                if (UDPhasdata)
                {
                    MessageHeader head;
                    foreach (int cutoff in UDPcutoffs)
                    {
                        head.nodeSource = NetEngine.NodeId;
                        head.size = cutoff - count;
                        UDPmemstream.Write(NetworkSerializer.GetBytes<MessageHeader>(head), 0, PacketUtils.MESSAGE_HEADER);
                        UDPmemstream.Seek(-PacketUtils.MESSAGE_HEADER, SeekOrigin.Current);
                        UDPmemstream.Read(buffer, 0, head.size);
                        count += head.size;
                        NetworkTransport.Send(hostID, connectionID, NetEngine.UDPChannel, buffer, head.size, out error);
                    }

                    head.nodeSource = NetEngine.NodeId;
                    head.size = UDPend - count;
                    UDPmemstream.Write(NetworkSerializer.GetBytes<MessageHeader>(head), 0, PacketUtils.MESSAGE_HEADER);
                    UDPmemstream.Seek(-PacketUtils.MESSAGE_HEADER, SeekOrigin.Current);
                    UDPmemstream.Read(buffer, 0, head.size);
                    count += head.size;
                    NetworkTransport.Send(hostID, connectionID, NetEngine.UDPChannel, buffer, head.size, out error);
                }

                TCPmemstream.Seek(0, 0);
                if (TCPhasdata)
                {
                    MessageHeader head;
                    count = 0;
                    foreach (int cutoff in TCPcutoffs)
                    {
                        head.nodeSource = NetEngine.NodeId;
                        head.size = cutoff - count;
                        TCPmemstream.Write(NetworkSerializer.GetBytes<MessageHeader>(head), 0, PacketUtils.MESSAGE_HEADER);
                        TCPmemstream.Seek(-PacketUtils.MESSAGE_HEADER, SeekOrigin.Current);
                        TCPmemstream.Read(buffer, 0, head.size);
                        count += head.size;
                        NetworkTransport.Send(hostID, connectionID, NetEngine.TCPChannel, buffer, head.size, out error);
                    }

                    head.nodeSource = NetEngine.NodeId;
                    head.size = TCPend - count;
                    TCPmemstream.Write(NetworkSerializer.GetBytes<MessageHeader>(head), 0, PacketUtils.MESSAGE_HEADER);
                    TCPmemstream.Seek(-PacketUtils.MESSAGE_HEADER, SeekOrigin.Current);
                    TCPmemstream.Read(buffer, 0, head.size);
                    count += head.size;
                    NetworkTransport.Send(hostID, connectionID, NetEngine.TCPChannel, buffer, head.size, out error);
                }

                UDPmemstream.Seek(PacketUtils.MESSAGE_HEADER, 0);
                TCPmemstream.Seek(PacketUtils.MESSAGE_HEADER, 0);
                UDPpos = PacketUtils.MESSAGE_HEADER;
                TCPpos = PacketUtils.MESSAGE_HEADER;
                UDPcutoffs.Clear();
                TCPcutoffs.Clear();
                UDPhasdata = false;
                TCPhasdata = false;
            }
        }

        public static class NetEngineConfig {
            public static float POSITION_EPSILON = 0.5f;
            public static float INTERP_COEFF = 5.0f;
        }
    }
}