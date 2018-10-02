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
            Debug.Log("Fire!");
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitedObj, firingRange))
            {
				if (hitedObj.collider.tag == "Damageable" || hitedObj.collider.tag == "Player" )
                {
					Debug.Log("Hit the target!");
                    Soldier s = hitedObj.collider.gameObject.GetComponent<Soldier>();
                    NetInterface.Fire(s.NetID, damage);
                }
            }
            else {
                Debug.Log("Missing target!");
            }
        }

	}
}