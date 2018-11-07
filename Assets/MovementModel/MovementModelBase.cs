using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// modifies the rigid body with the step function given an input state.
public abstract class MovementModelBase : MonoBehaviour {

    public Rigidbody rb;
    // steps
    public abstract void Step(uint inputstate);
    // in case rotations are absolute also pass in a quaternion q for rotation
    public abstract void AbsoluteRotate(Quaternion q);

    public abstract void Init();
}
