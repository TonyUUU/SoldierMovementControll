using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using BarbaricCode.Networking;

public enum flow {
    IDLE,
    PLAY,
    FINISH,
	LOAD_FINISH,
    PLAYER_DIED,
    ROLE_CHANGE_SOLDIER,
    ROLE_CHANGE_GENERAL,
}


public class FlowHandlerType: Attribute {
    public flow type;
    public FlowHandlerType(flow type) {
        this.type = type;
    } 
}
public static class FlowControl {
    public static Dictionary<flow, FlowHandler> FlowHandlerMapping = new Dictionary<flow, FlowHandler>();
	public delegate void FlowHandler(int node_id);

    static FlowControl() {
        var meths = typeof(FlowControl).GetMethods().Where(meth => Attribute.IsDefined(meth, typeof(FlowHandlerType)));
        foreach (System.Reflection.MethodInfo method in meths)
        {
            FlowHandlerType handler = method.GetCustomAttributes(typeof(FlowHandlerType), true).First() as FlowHandlerType;
            FlowHandler flowHandler = (FlowHandler)Delegate.CreateDelegate(typeof(FlowHandler), method);
            FlowHandlerMapping.Add(handler.type, flowHandler);
        }
        
    }
    // all the flow handlers
    [FlowHandlerType(flow.PLAY)]
	public static void play_flow_handler(int node_id) {
        Debug.Log("loading play scene");
        SceneManager.LoadScene("PlayScene");
    }

    [FlowHandlerType(flow.LOAD_FINISH)]
    public static void FinishLoadHandler(int node_id)
    {
        if (!NetEngine.IsServer) {
            return;
        }

        Debug.Log("Finish loading");
        if (!GameState.loadedNodes.Contains(node_id) && node_id != 0)
        {
            GameState.loadedNodes.Add(node_id);
            GameState.players[node_id].loaded = true;
        }

        foreach (Connection c in NetEngine.Connections.Values)
        {
            Debug.Log("Connection " + c.nodeID);
            if (!GameState.loadedNodes.Contains(c.nodeID))
            {
                return;
            }
        }

        Debug.Log("Start Play");
        // spawn person for player
        // should run on server.
        foreach (Connection c in NetEngine.Connections.Values)
        {
            Debug.Log("Spawning for " + c.nodeID);
            if (GameState.players[c.nodeID].role == GameRole.GENERAL)
            {
                NetEngine.Spawn(1, c.nodeID);
            }
            else {
                NetEngine.Spawn(0, c.nodeID);
            }
            
        }
        // spawn for server
        if (GameState.players[0].role == GameRole.GENERAL)
        {
            NetEngine.Spawn(1, 0);
        }
        else {
            NetEngine.Spawn(0, 0);
        }
    }

    [FlowHandlerType(flow.PLAYER_DIED)]
    public static void PLAYER_DIED(int node_id) {
        if (!NetEngine.IsServer) {
            return;
        }

        // respawn player for that node
        NetEngine.Spawn(0, node_id);

    }

    [FlowHandlerType(flow.ROLE_CHANGE_GENERAL)]
    public static void RoleChangeGeneralHandler(int nodeid) {
        if (!NetEngine.IsServer) {
            return;
        }
        GameState.players[nodeid].role = GameRole.GENERAL;
    }

    [FlowHandlerType(flow.ROLE_CHANGE_SOLDIER)]
    public static void RoleChangeSoldierHandler(int nodeid) {
        if (!NetEngine.IsServer) {
            return;
        }
        GameState.players[nodeid].role = GameRole.SOLDIER;
    }
}
