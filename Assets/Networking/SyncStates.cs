using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SoldierState
{
    public Vector3 pos;
    public Vector3 vel;
    public Quaternion headrot;
    public Quaternion bodrot;
    public Soldier.SoldierControlState inputState;
    public int health;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct GeneralState
{
    public Vector3 pos;
    public Quaternion bodrot;
}