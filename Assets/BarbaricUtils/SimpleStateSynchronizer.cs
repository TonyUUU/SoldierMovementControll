using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BarbaricCode.Networking;
public class SimpleStateSynchronizer : StateSynchronizableMonoBehaviour
{
    public override byte[] GetState(out int size)
    {
        SegmentHeader head;
        head.type = MessageType.STATE_DATA;
        StateDataMessage sd;
        sd.SegHead = head;
        sd.NetID = NetID;
        sd.TimeStep = NetEngine.SimStep;
        sd.StateSize = 0;
        SimpleState ss;
        ss.StateHeader = sd;
        ss.Position = transform.position;
        ss.Rotation = transform.rotation;

        byte[] b2 = NetworkSerializer.GetBytes<SimpleState>(ss);
        sd.StateSize = b2.Length;
        byte[] b1 = NetworkSerializer.GetBytes<StateDataMessage>(sd);
        byte[] b3 = NetworkSerializer.Combine(b1, b2);
        size = b3.Length;
        return b3;
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    public override void OnDespawn()
    {
        throw new System.NotImplementedException();
    }

    public override void Synchronize(byte[] state, int stamp)
    {

        if (LocalAuthority) {
            return;
        }

        SimpleState ss = NetworkSerializer.GetStruct<SimpleState>(state);
        transform.position = ss.Position;
        transform.rotation = ss.Rotation;
    }
    private void Update()
    {
        if (!LocalAuthority)
        {
            return;
        }

        Vector3 moveVec = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveVec.y++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveVec.y--;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveVec.x--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveVec.x++;
        }
        transform.position += moveVec * Time.fixedDeltaTime;
    }
}
