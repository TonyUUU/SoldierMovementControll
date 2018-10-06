using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum flow {
    IDLE,
    PLAY,
    FINISH,
}


public class FlowHandlerType: Attribute {
    public flow type;
    public FlowHandlerType(flow type) {
        this.type = type;
    } 
}
public static class FlowControl {
    public static Dictionary<flow, FlowHandler> FlowHandlerMapping = new Dictionary<flow, FlowHandler>();
    public delegate void FlowHandler();

    static FlowControl() {
        var meths = typeof(FlowControl).GetMethods().Where(meth => Attribute.IsDefined(meth, typeof(FlowHandler)));
        foreach (System.Reflection.MethodInfo method in meths)
        {
            FlowHandlerType handler = method.GetCustomAttributes(typeof(FlowHandlerType), true).First() as FlowHandlerType;
            FlowHandler flowHandler = (FlowHandler)Delegate.CreateDelegate(typeof(FlowHandler), method);
            FlowHandlerMapping.Add(handler.type, flowHandler);
        }
        
    }
    // all the flow handlers
    [FlowHandlerType(flow.PLAY)]
    public static void play_flow_handler() {
        Debug.Log("loading play scene");
        // SceneManager.LoadScene("PlayScene");
    }

}
