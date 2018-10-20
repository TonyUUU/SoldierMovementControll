using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;
using System.Runtime.InteropServices;

[PacketStruct(MessageType.CONNECTION_INFO)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ConnectionInfo {
    public SegmentHeader seghead;
    public Player player;
}

[PacketStruct(MessageType.SOLDIER_STATE)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SoldierState {
    public StateDataMessage statehead;
    public Vector3 pos;
    public Vector3 vel;
    public Quaternion headrot;
    public Quaternion bodrot;
    public Soldier.SoldierControlState inputState;
    public int health;
}

[PacketStruct(MessageType.GENERAL_STATE)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GeneralState {
    public StateDataMessage statehead;
    public Vector3 pos;
    public Quaternion bodrot;
}
[PacketStruct(MessageType.GOT_HIT)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HIT {
    public SegmentHeader seghead;
    public int NetID;
    public int Damage;
}