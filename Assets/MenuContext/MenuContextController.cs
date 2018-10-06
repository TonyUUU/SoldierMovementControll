using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuContextController : MonoBehaviour {
    public static MenuContextController instance;
    public GameObject MainMenu;
    public GameObject NetworkLobby;

    public GameObject AlertPopup;

    // Use this for initialization
	void Start () {
        instance = this;
        MainMenu.SetActive(true);
        NetworkLobby.SetActive(false);
	}

    public void SwitchToMainMenu() {
        MainMenu.SetActive(true);
        NetworkLobby.SetActive(false);
    }

    public void SwitchToNetworkLobby() {
        NetworkLobby.SetActive(true);
        MainMenu.SetActive(false);
    }

    public void CreateAlert(string message, GameObject container) {
        GameObject alert = GameObject.Instantiate(AlertPopup);
        AlertPopup als = alert.GetComponent<AlertPopup>();
        als.SetText(message);
        als.transform.SetParent(container.transform);
        RectTransform rt = (RectTransform)als.transform;
        rt.localPosition = Vector3.zero;
        rt.sizeDelta = new Vector2(0, 0);
    }

}
