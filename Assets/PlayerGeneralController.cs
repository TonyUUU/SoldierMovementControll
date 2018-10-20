using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(General))]
public class PlayerGeneralController : MonoBehaviour {
    General general;

    private void Start()
    {
        general = GetComponent<General>();
        if (!general.LocalAuthority) {
            Destroy(this);
            return;
        }
        general.CamPivot.AddComponent<PlayerCameraController>();
    }

    void Update () {
        Vector3 dpos = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) {
            dpos.z += 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            dpos.z -= 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            dpos.x -= 1;
        }
        if (Input.GetKey(KeyCode.D)) {
            dpos.x += 1;
        }

        general.Move(dpos);

        general.AddHval(-Input.mouseScrollDelta.y);
    }
}
