using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarbaricCode.Networking;


public class UIController : MonoBehaviour {

    public InputField IP;
    public InputField Port;

    public void StarClient() {
        NetEngine.StartSocket(int.Parse(Port.text));
    } 

    public void StartServer() {
        NetEngine.StartServer(int.Parse(Port.text));
    }

    public void Connect() {
        NetEngine.Connect(IP.text, int.Parse(Port.text));
    }

    public void Spawn() {
        NetEngine.Spawn(0);
    }
}
