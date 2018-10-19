using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
    private void LateUpdate()
    {
        Camera.main.gameObject.transform.position = transform.position;
        Camera.main.gameObject.transform.rotation = transform.rotation;
    }
}
