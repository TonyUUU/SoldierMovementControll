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

public class Player {
    public GameRole role;
    // public fixed char name[32];
    public int nodeID; // doubles as a player id
    public bool loaded;
}

// put this in a state synchronizable mono? :o
public static class GameState {
    public static Dictionary<int, Player> players = new Dictionary<int, Player>();
    public static flow currentFlowStatus = flow.IDLE;
	public static List<int> loadedNodes = new List<int> ();
}