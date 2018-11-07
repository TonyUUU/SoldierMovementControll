using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;
public class General : StateSynchronizableMonoBehaviour {

    private float hval;
    public GameObject CamPivot;

    private Vector3 remotePos;
    private Quaternion remoteRot;
    private int timeStamp = 0;
    public void FixedUpdate()
    {
        float targety = hval;
        float y = transform.position.y + (targety - transform.position.y) * (Time.fixedDeltaTime * 5);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        // interpolate if not local auth
        if (!LocalAuthority)
        {
            // ignore remote velocity for now, captured by input state
            // sync pos, rot
            transform.rotation = Quaternion.Slerp(transform.rotation, remoteRot, Time.fixedDeltaTime * NetEngineConfig.INTERP_COEFF);
            if ((transform.position - remotePos).magnitude >= NetEngineConfig.POSITION_EPSILON)
            {
                transform.position = remotePos;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, remotePos, Time.fixedDeltaTime * NetEngineConfig.INTERP_COEFF);
            }
        }

    }

    public override byte[] GetState(out int size)
    {
        // all packets start with segment header (except for a few exceptions)
        SegmentHeader seghead;
        // the segment header tells the engine what type of packet to expect
        seghead.type = MessageType.STATE_DATA; // It's a state packet
        // all state messages start with a state_header
        StateDataMessage sdm;
        sdm.NetID = NetID;
        sdm.TimeStep = NetEngine.SimStep;
        sdm.SegHead = seghead;
        // This header tells the engine what the id is, timestep and type of state
        sdm.StateSize = 0;
        GeneralState ss;
        ss.pos = transform.position;
        ss.bodrot = transform.rotation;
        byte[] b1, b2, b3;
        b2 = NetworkSerializer.GetBytes<GeneralState>(ss);
        sdm.StateSize = b2.Length;
        b1 = NetworkSerializer.GetBytes<StateDataMessage>(sdm);
        b3 = NetworkSerializer.Combine(b1, b2);
        size = b3.Length;
        return b3;
    }

    public override void Init()
    {
    }

    public override void OnDespawn()
    {
        throw new System.NotImplementedException();
    }

    public override void Synchronize(byte[] state, int stamp)
    {
        if (LocalAuthority) { return; }
        SoldierState ss = NetworkSerializer.GetStruct<SoldierState>(state);
        if (stamp > timeStamp)
        {
            this.remotePos = ss.pos;
            this.timeStamp = stamp;
        }
    }

    public void AddHval(float val) {
        hval += val;
    }

    public void Move(Vector3 dpos)
    {
        dpos = transform.rotation * dpos;
        transform.position += dpos * Time.deltaTime * 5;
    }
}
