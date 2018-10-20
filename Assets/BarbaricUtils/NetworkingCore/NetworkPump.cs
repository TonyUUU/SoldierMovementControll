using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;

public class NetworkPump : MonoBehaviour {
    // Update is called once per frame
    public List<GameObject> SpawnablePrefabs;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        int count = 0;
        foreach (GameObject go in SpawnablePrefabs) {
            NetEngine.SpawnablePrefabs.Add(count, go);
            count++;
        }
    }

    void FixedUpdate () {
        NetEngine.Step();
	}
}
