using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open_fire : MonoBehaviour {
    public float firingRange = 10000F;
    public int damage = 10;
    public RaycastHit hitedObj;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * firingRange, Color.red, 0.001f);
        if (Input.GetMouseButtonDown(0) == true) {
            Debug.Log("Fire!");
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitedObj, firingRange))
            {
                Debug.Log("Hit the target!");
                if (hitedObj.collider.tag != "Unbreakable")
                {
                    Enemy_info target_info = hitedObj.transform.GetComponent<Enemy_info>();
                    target_info.status.getHit(damage);
                }
            }
            else {
                Debug.Log("Missing target!");
            }
        }
	}
}
