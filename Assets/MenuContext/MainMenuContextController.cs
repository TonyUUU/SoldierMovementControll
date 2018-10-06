using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarbaricCode.Networking;
public class MainMenuContextController : MonoBehaviour {
    public GameObject MainMenuContext;
    public GameObject MenuPanel;
    public GameObject PlaySubPanel;
    public GameObject IPPanel;
    public GameObject MaskPanel;
    public InputField IPInput;

    public static MainMenuContextController instance;

    private void Start()
    {
        PlaySubPanel.SetActive(false);
        IPPanel.SetActive(false);
        MaskPanel.SetActive(false);
        instance = this;
    }

    public void PlayClicked() {
        if (PlaySubPanel.activeInHierarchy)
        {
            PlaySubPanel.SetActive(false);
        }
        else {
            PlaySubPanel.SetActive(true);
        }
    }

    public void JoinClicked() {
        IPPanel.SetActive(true);
    }

    public void HostClicked()
    {
        // some network actions
        NetEngine.StartServer(NetworkConfig.DEFAULT_HOST_PORT);
    }

    public void JoinCancelClicked() {
        IPPanel.SetActive(false);
    }

    public void JoinJoinClicked() {
        MaskPanel.SetActive(true);
        NetEngine.StartSocket(NetworkConfig.DEFAULT_CLIENT_PORT);
        NetEngine.Connect(IPInput.text, NetworkConfig.DEFAULT_HOST_PORT);
        // net engine stuff
    }

    public void RemoveMask() {
        MaskPanel.SetActive(false);
    }
}
