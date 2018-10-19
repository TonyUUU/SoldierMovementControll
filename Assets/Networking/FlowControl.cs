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
    PLAYER_DIED
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
            NetEngine.Spawn(0, c.nodeID);
        }
        // spawn for server
        NetEngine.Spawn(0, 0);
    }

    [FlowHandlerType(flow.PLAYER_DIED)]
    public static void PLAYER_DIED(int node_id) {
        if (!NetEngine.IsServer) {
            return;
        }

        // respawn player for that node
        NetEngine.Spawn(0, node_id);

    }
}
