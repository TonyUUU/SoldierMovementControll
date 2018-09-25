using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Soldier_controller : MonoBehaviour {

    public float speed = 10.0F;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        
	}
	
	// Update is called once per frame
	void Update () {
        float z_change = Input.GetAxis("Vertical") * speed;
        float x_change = Input.GetAxis("Horizontal") * speed;
        transform.Translate(x_change * Time.deltaTime, 0, z_change * Time.deltaTime);

        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;
	}
}
