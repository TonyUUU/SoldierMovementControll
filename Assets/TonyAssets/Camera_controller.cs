using System.Collections;
using UnityEngine;

public class Camera_controller : MonoBehaviour {

    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensityivity = 1.0f;
    public float smoothing = 2.0f;

    GameObject character;

	// Use this for initialization
	void Start () {
        character = this.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 mvmt = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mvmt = Vector2.Scale(mvmt, new Vector2(sensityivity * smoothing, sensityivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, mvmt.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, mvmt.y, 1f / smoothing);
        mouseLook += smoothV;

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
