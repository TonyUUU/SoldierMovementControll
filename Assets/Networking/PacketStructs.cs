﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;
using System.Runtime.InteropServices;

[PacketStruct(MessageType.SOLDIER_STATE)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SoldierState {
    public StateDataMessage statehead;
    public Vector3 pos;
    public Vector3 vel;
    public Quaternion rot;
    public int health;
}


[PacketStruct(MessageType.GOT_HIT)]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct HIT {
    public int NetID;
    public int Damage;
}