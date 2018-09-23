using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_info : MonoBehaviour {
    public Object_status status;
	// Use this for initialization
	void Start () {
        status = new Object_status(this.gameObject, 100);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
