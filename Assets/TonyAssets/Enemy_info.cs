using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_info : MonoBehaviour {
    public Object_status status;
	// Use this for initialization
	public GameObject explosionprefab;
	void Start () {
        status = new Object_status(this.gameObject, 100);
	}
	
	// Update is called once per frame
	void Update () {
		if (status.healthPoint == 0) {
			Destroy(this.gameObject);
			GameObject exp = Instantiate(explosionprefab, transform.position, Quaternion.identity);
			Destroy(exp, 4);
		}
	}
}
