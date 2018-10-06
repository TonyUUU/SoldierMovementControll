using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Soldier_controller : MonoBehaviour {

    public float speed = 10.0F;
    public Rigidbody rb;

    public GameObject misslePrefab;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float z_change = Input.GetAxis("Vertical") * speed;
        float x_change = Input.GetAxis("Horizontal") * speed;
        // transform.Translate(x_change * Time.deltaTime, 0, z_change * Time.deltaTime);
        
		if (Input.GetKeyDown("f")) {
			Instantiate(misslePrefab, new Vector3(transform.position.x, transform.position.y + 100, transform.position.z), Quaternion.identity);
		}

        rb.velocity = new Vector3(x_change, 0, z_change);

        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;
	}
}
