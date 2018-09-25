using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController : MonoBehaviour {

	[SerializeField]
	private float speed = 10.0f;
	// Use this for initialization

	public GameObject misslePrefab;
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		
		Move();
		// call missile attack when space is pressed
		if (Input.GetKeyDown("f")) {
			Instantiate(misslePrefab, new Vector3(transform.position.x, transform.position.y + 100, transform.position.z), Quaternion.identity);
		}
		if (Input.GetKeyDown("escape"))
			Cursor.lockState = CursorLockMode.None;
	}

	private void Move() {
		float translation = Input.GetAxis("Vertical") * speed;	
		float straffle = Input.GetAxis("Horizontal") * speed;
		float jump = Input.GetAxis("Jump") * speed;
		translation *= Time.deltaTime;
		straffle *= Time.deltaTime;
		jump *= Time.deltaTime; 

		transform.Translate(straffle, jump, translation);
		if (transform.position.y < 0.5) {
			transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
		}
	}

}
