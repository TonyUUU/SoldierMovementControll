using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarbaricCode.Networking;


public class LobbyMenuContextController : MonoBehaviour {
    // yeah some sort of data structure

    public void DisconnectClicked() {
        NetEngine.CloseSocket();
        MenuContextController.instance.SwitchToMainMenu();
    }

    public void PlayClicked() {

    }
}
