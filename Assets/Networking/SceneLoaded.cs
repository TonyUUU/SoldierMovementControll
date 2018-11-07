using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;

public class SceneLoaded : MonoBehaviour {

    private void Start()
    {
        if (NetEngine.IsServer)
        {
            NetEngine.userHandlers[(int)FlowMessageType.LOAD_FINISH].Invoke(0, -1, null, 0);
        }
        NetInterface.BroadCastFlowMessage(FlowMessageType.LOAD_FINISH, null, true);
    }
}
