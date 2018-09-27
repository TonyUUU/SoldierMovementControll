using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class PlayerSoldierController : MonoBehaviour {

    public float speed = 10.0F;
    public GameObject misslePrefab;
    public Soldier soldier;

    private void Start()
    {
        soldier = GetComponent<Soldier>();
        // if the soldier is not a local authority then
        // there is no reason to keep this script
        // i.e. it's the controller for another player
        if (!soldier.LocalAuthority) {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update () {
        // should change these to Input.GetKey(KeyCode.key)
        // So the player can change bindings, but that's for later
        // @TODO
        float z_change = Input.GetAxis("Vertical") * speed;
        float x_change = Input.GetAxis("Horizontal") * speed;

        soldier.Move(x_change, z_change);
        
        // there's probably a better way to do this
        if (Input.GetKeyDown("f")) {
			Instantiate(misslePrefab, new Vector3(transform.position.x, transform.position.y + 100, transform.position.z), Quaternion.identity);
		}

        if (Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
