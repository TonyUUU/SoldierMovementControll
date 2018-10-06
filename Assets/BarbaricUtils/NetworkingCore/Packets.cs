using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;

namespace BarbaricCode {
    namespace Networking
    {

        public class PacketStruct : Attribute
        {
            public MessageType type;
            public PacketStruct(MessageType type)
            {
                this.type = type;
            }
        }

        public static class PacketUtils {
            public static int MESSAGE_HEADER = Marshal.SizeOf(typeof(MessageHeader));
            public static int SEGMENT_HEADER = Marshal.SizeOf(typeof(SegmentHeader));
            public static int ESTABLISH_CONNECTION = Marshal.SizeOf(typeof(EstablishConnectionMessage));

            public static Dictionary<MessageType, int> MessageToStructSize = new Dictionary<MessageType, int>();

            static PacketUtils() {
                // this is making the assumption that all assemblies we need are already loaded.
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes().Where(type => Attribute.IsDefined(type, typeof(PacketStruct))))
                    {
                        PacketStruct handler = type.GetCustomAttributes(typeof(PacketStruct), true).First() as PacketStruct;
                        MessageToStructSize.Add(handler.type, Marshal.SizeOf(type));
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MessageHeader
        {
            public int nodeSource;
            public int size;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SegmentHeader {
            public MessageType type;
        }

        // net connectivity messages

        [PacketStruct(MessageType.ESTABLISH_CONNECTION)]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EstablishConnectionMessage {
            public SegmentHeader SegHead;
            public int newID;
        }

        // inherited? struct
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct StateDataMessage {
            public SegmentHeader SegHead;
            public int NetID;
            public int TimeStep;
            public MessageType StateType;
        }

        [PacketStruct(MessageType.SPAWN)]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SpawnMessage {
            public SegmentHeader SegHead;
            public int PrefabID;
            public int NetID;
            public int AuthorityID;
            public Vector3 Position;
            public Quaternion Rotation; // find a way to marry this with state
        }

        public enum MessageType {
            ESTABLISH_CONNECTION,
            STATE_DATA,
            SPAWN,
            DESPAWN,
            CONNECTION_INFO,
            // state
            SIMPLE_STATE,
            SOLDIER_STATE,
            // message
            GOT_HIT,

        }
    }
}