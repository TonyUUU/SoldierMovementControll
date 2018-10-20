using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarbaricCode.Networking;

public class LobbyMenuContextController : MonoBehaviour {
    // yeah some sort of data structure

	public GameObject PlayButton;
	public string sceneName;
    public Dropdown dropdown;

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

    public void OnRoleChange() {
        if (!NetEngine.IsServer) {
            switch (dropdown.value) {
                case 0:
                    NetInterface.SendFlowMessage(flow.ROLE_CHANGE_SOLDIER);
                    break;
                case 1:
                    NetInterface.SendFlowMessage(flow.ROLE_CHANGE_GENERAL);
                    break;
            }
        } else {
            switch (dropdown.value) {
                case 0:
                    GameState.players[0].role = GameRole.SOLDIER;
                    break;
                case 1:
                    GameState.players[0].role = GameRole.GENERAL;
                    break;
            }
        }
    }

}
