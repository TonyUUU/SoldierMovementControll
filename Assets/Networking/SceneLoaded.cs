using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaded : MonoBehaviour {

    private void Start()
    {
        NetInterface.SendFlowMessage(flow.LOAD_FINISH);
    }
}
