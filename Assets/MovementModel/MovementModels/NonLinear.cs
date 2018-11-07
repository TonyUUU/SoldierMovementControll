using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonLinear : MovementModelBase
{

    /// <summary>
    /// consider the idea of virtual velocity vs real velocity
    /// </summary>

    // some temp vars
    static float edge_down_threshold;
    static float mid_down_threshold;

    private static int terrain_layer_mask;

    public TerrainDetector terrdet;

    public float MaxSpeed = 10;
    public float Accel = 1.0f;
    public float JumpVelocity = 10;
    public float StrafeDamp = 0.45f;
    public float SpeedDecayFactor = 8f;


    public GameObject[] CastPoints;
    private Vector3[] _cOffsets = new Vector3[9];

    bool Airborne = false;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public override void AbsoluteRotate(Quaternion q)
    {
        throw new System.NotImplementedException();
    }

    public override void Step(uint inputstate)
    {
        Vector3 vel = rb.velocity;

        bool airborne = terrdet.Airborne();

        // apply accel
        int zAxis = InputStateUtils.GetZAxis(inputstate);
        int xAxis = InputStateUtils.GetXAxis(inputstate);

        float accel_rate = 1f;

        if (airborne) {
            accel_rate = 0.25f;
        }

        vel = vel - transform.right   * xAxis * Time.fixedDeltaTime * Accel * accel_rate;
        vel = vel + transform.forward * zAxis * Time.fixedDeltaTime * Accel * accel_rate;
        Vector2 xy = new Vector2(vel.x, vel.z);

        if (xy.magnitude > MaxSpeed) {
            xy = xy.normalized * MaxSpeed;
        }

        vel.x = xy.x;
        vel.z = xy.y;

        // gotta split the shit
        if (!airborne)
        {
            if (zAxis == 0)
            {
                vel = vel - transform.forward * Vector3.Dot(vel, transform.forward) * Time.fixedDeltaTime * SpeedDecayFactor;
            }
            else
            {
                if (Vector3.Dot(vel, transform.forward * zAxis) < 0)
                {
                    vel = vel - zAxis * transform.forward * Vector3.Dot(vel, transform.forward * zAxis) * StrafeDamp;
                }
            }

            if (xAxis == 0)
            {
                vel = vel - transform.right * Vector3.Dot(vel, transform.right) * Time.fixedDeltaTime * SpeedDecayFactor;
            }
            else
            {
                if (Vector3.Dot(vel, -transform.right * xAxis) < 0)
                {
                    vel = vel + transform.right * xAxis * Vector3.Dot(vel, -transform.right * xAxis) * StrafeDamp;
                }
            }
        }

        if (!airborne && InputStateUtils.GetJumpState(inputstate)) {
            vel.y = JumpVelocity;
            airborne = true;
        }   

        rb.velocity = vel;

        // apply clamp
    }

    public override void Init()
    {
        Start();
    }
}