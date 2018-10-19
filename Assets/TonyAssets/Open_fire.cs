using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;

[RequireComponent(typeof(Soldier))]
public class Open_fire : MonoBehaviour {
    public float firingRange = 10000F;
    public int damage = 10;
    public RaycastHit hitedObj;
    private Soldier soldier;


	// Use this for initialization
	void Start () {
        soldier = GetComponent<Soldier>();
        if (!soldier.LocalAuthority){
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void Update () {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * firingRange, Color.red, 0.001f);
        if (Input.GetMouseButtonDown(0) == true) {

        }

	}
}