using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarbaricCode.Networking;

public class LobbyMenuContextController : MonoBehaviour {
    // yeah some sort of data structure

	public GameObject PlayButton;
	public string sceneName;

	void OnEnable() {
		PlayButton.SetActive (NetEngine.IsServer);
	}

    public void DisconnectClicked() {
        NetEngine.CloseSocket();
		MenuContextController.instance.SwitchToMainMenu();
    }

    public void PlayClicked() {
		if (NetEngine.IsServer) {
			// send play message
			FlowControl.FlowHandlerMapping[flow.PLAY].Invoke(0);
			NetInterface.SendFlowMessage(flow.PLAY);
		}
    }
}
