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
        sd.StateType = MessageType.SIMPLE_STATE;
        SimpleState ss;
        ss.StateHeader = sd;
        ss.Position = transform.position;
        ss.Rotation = transform.rotation;

        size = PacketUtils.MessageToStructSize[MessageType.SIMPLE_STATE];
        return NetworkSerializer.GetBytes<SimpleState>(ss);
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    public override void Synchronize(byte[] state)
    {

        if (LocalAuthority) {
            return;
        }

        SimpleState ss = NetworkSerializer.ByteArrayToStructure<SimpleState>(state);
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
