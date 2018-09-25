using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier_info : MonoBehaviour {
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

	void gotHit(int damage){
		status.healthPoint -= damage;
		if (status.healthPoint < 0) {
			status.healthPoint = 0;
		}

		Debug.Log(string.Format("Soldier Got a shot! MEDIC!. Damage Value {0}, Current HP{1}", damage, status.healthPoint));
	}
}
