using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;

public class SceneLoaded : MonoBehaviour {

    private void Start()
    {
        if (NetEngine.IsServer)
        {
            FlowControl.FinishLoadHandler(0);
        }
        NetInterface.SendFlowMessage(flow.LOAD_FINISH);
    }
}
