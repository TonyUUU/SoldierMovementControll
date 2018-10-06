using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using BarbaricCode.Networking;

public enum GameRole {
    GENERAL,
    SOLDIER,
    NONE
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Player {
    public GameRole role;
    public fixed char name[32];
    public int nodeID; // doubles as a player id
}

public static class GameState {
    public static Dictionary<int, Player> players = new Dictionary<int, Player>();
    public static flow currentFlowStatus = flow.IDLE;
}