using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileExplosion : MonoBehaviour {

	// Use this for initialization
	public GameObject explosion;
	void Start () {	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < 1) {
			// Destroy(this.gameObject, 1);
			Explode();
		}
	}
	void Explode() {
		GameObject exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
		Destroy(exp, 3);
    }
}
