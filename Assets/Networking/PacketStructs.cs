using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;
using System.Runtime.InteropServices;

// State Synch Should NOT be User Space Handled
public enum UserMessageType : int {

}

public enum FlowMessageType : int {
    // 200 - 299
    IDLE = 200,
    PLAY = 201,
    FINISH = 202,
    LOAD_FINISH = 203,
    PLAYER_DIED = 204,
    ROLE_CHANGE_SOLDIER = 205,
    ROLE_CHANGE_GENERAL = 206,
}


[PacketStruct(MessageType.CONNECTION_INFO)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct ConnectionInfo {
    public SegmentHeader seghead;
    public Player player;
}

[PacketStruct(MessageType.GOT_HIT)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HIT {
    public SegmentHeader seghead;
    public int NetID;
    public int Damage;
}