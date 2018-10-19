using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;

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
            return;
        }

        soldier.CameraPoint.AddComponent<PlayerCameraController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update () {
        // should change these to Input.GetKey(KeyCode.key)
        // So the player can change bindings, but that's for later
        // @TODO
        float z_change = Input.GetAxis("Vertical") * speed;
        float x_change = Input.GetAxis("Horizontal") * speed;
        Soldier.SoldierControlState scs;
        int state = 0;

        if (Input.GetKey(KeyCode.W)) {
            state = state | 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            state = state | (1 << 2);
        }

        if (Input.GetKey(KeyCode.S))
        {
            state = state | 2;
        }

        if (Input.GetKey(KeyCode.D))
        {
            state = state | (2 << 2);
        }

        scs.MoveState = state;
        soldier.SetInputState(scs);

        // there's probably a better way to do this
        if (Input.GetKeyDown("f")) {
			Instantiate(misslePrefab, new Vector3(transform.position.x, transform.position.y + 100, transform.position.z), Quaternion.identity);
		}

        if (Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            soldier.FireDown();
        }
        else {
            soldier.FireUp();
        }

        float dy = Input.GetAxis("Mouse Y");
        float dx = Input.GetAxis("Mouse X");
        Quaternion bodrot = transform.rotation * Quaternion.Euler(0, dx, 0);
        Quaternion headrot = soldier.HeadJoint.transform.localRotation * Quaternion.Euler(-dy, 0, 0);
        soldier.Rotate(bodrot, headrot);
    }
}
