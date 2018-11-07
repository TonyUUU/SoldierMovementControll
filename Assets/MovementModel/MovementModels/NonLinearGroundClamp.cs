using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonLinearGroundClamp : MovementModelBase
{

    // some temp vars
    static float edge_down_threshold;
    static float mid_down_threshold;

    private static int terrain_layer_mask;

    public float MaxSpeed = 10;
    public float Accel = 9999999;

    public GameObject[] CastPoints;
    private Vector3[] _cOffsets = new Vector3[9];

    bool Airborne = false;

    private void Start()
    {
        for (int i = 0; i < CastPoints.Length; i++) {
            _cOffsets[i] = CastPoints[i].transform.position - transform.position;
        }
        rb = GetComponent<Rigidbody>();
        terrain_layer_mask = LayerMask.NameToLayer("terrain");

        edge_down_threshold = CastPoints[0].transform.position.y - CastPoints[8].transform.position.y;

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public override void AbsoluteRotate(Quaternion q)
    {
        throw new System.NotImplementedException();
    }

    public override void Step(uint inputstate)
    {
        Airborne = true;
        // should scale the middown by velocity.
        mid_down_threshold = -rb.velocity.y * Time.fixedDeltaTime + 0.05f;
        mid_down_threshold = Mathf.Max(mid_down_threshold, 0);
        // Check 9 ground points
        // Set y to max of valid set
        // Check airborne

        float y_next = Mathf.NegativeInfinity;
        bool hit;
        Vector3 v;
        RaycastHit rh;
        for (int i = 0; i < 8; i++) {
            v = transform.rotation * _cOffsets[i];
            hit = false;
            hit = Physics.Raycast(transform.position + v, Vector3.down, out rh, edge_down_threshold+0.05f, terrain_layer_mask);

            if (hit)
            {
                y_next = Mathf.Max(transform.position.y - rh.distance + edge_down_threshold, y_next);
                Debug.DrawLine(transform.position + v, transform.position + v + Vector3.down * edge_down_threshold, Color.blue);
                if (rb.velocity.y <= 0)
                {
                    Airborne = false;
                }
            }
            else {
                Debug.DrawLine(transform.position + v, transform.position + v + Vector3.down * edge_down_threshold, Color.red);
            }
        }

        if (!Airborne) {
            mid_down_threshold = 0.5f;
        }

        v = transform.rotation * _cOffsets[8];
        hit = false;
        hit = Physics.Raycast(transform.position + v, Vector3.down, out rh, mid_down_threshold+0.05f, terrain_layer_mask);
        if (hit)
        {
            if (rb.velocity.y <= 0.05)
            {
                Airborne = false;
                y_next = Mathf.Max(transform.position.y - rh.distance, y_next);
                Debug.DrawLine(transform.position + v, transform.position + v + Vector3.down * mid_down_threshold, Color.green);
            }
            else {
                Debug.DrawLine(transform.position + v, transform.position + v + Vector3.down * mid_down_threshold, Color.red);
            }
        }
        // no valid set? Pin to ground if within a threshold.
        // Otherwise we airborne
        if (y_next == Mathf.NegativeInfinity || Airborne) {
            y_next = transform.position.y;
        }

        // apply friction
        float yvel = rb.velocity.y;

        Vector3 vel = rb.velocity;
        if (InputStateUtils.GetXAxis(inputstate) == 0) {
            vel = vel - transform.rotation*Vector3.right * Vector3.Dot(vel, transform.rotation * Vector3.right) * (8*Time.fixedDeltaTime);
        }



        // apply gravity
        if (Airborne)
        {
            yvel -= Time.fixedDeltaTime * 9.8f;
            yvel = Mathf.Max(-5, yvel);
        }
        else {
            yvel = 0;
        }
        vel.y = yvel;
        rb.velocity = vel;
        // apply acceleration (Also based on airbone or not)

        float accelerationRate = 1;
        if (Airborne)
        {
            // half acceleration
            // accelerationRate = 0.5f;
        }

        vel = rb.velocity;

        vel += transform.forward * InputStateUtils.GetZAxis(inputstate) * Accel * accelerationRate * Time.fixedDeltaTime;
        vel -= transform.right * InputStateUtils.GetXAxis(inputstate) * Accel * accelerationRate * Time.fixedDeltaTime;
        // apply clamping

        vel.y = yvel;
        
        // mutate rigidbody

        // we don't freeze y but we maybe keep close tabs on it.
        transform.position = new Vector3(transform.position.x, y_next, transform.position.z);
        rb.velocity = vel;

        // maybe use an interpolant.
    }

    public override void Init()
    {
        Start();
    }
}
