using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarbaricCode.Networking;
public class DebugNetEngine : MonoBehaviour {
    public Text text;
    private void Update()
    {
        text.text = "NodeID: " + NetEngine.NodeId + "\nConnections: " + NetEngine.Connections.Count;
    }
}
