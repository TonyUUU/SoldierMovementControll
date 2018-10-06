using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;


public static class NetInterface {

    // consider caching soldiers directly for performance
    public static void Fire(int net_id, int damage)
    {

        if (!NetEngine.NetworkObjects.ContainsKey(net_id)) {
            Debug.LogWarning("Net id does not exist on this node: " + net_id);
        } else {
            NetEngine.NetworkObjects[net_id].GetComponent<Soldier>().getHit(damage);
        }

        SegmentHeader seghead;
        seghead.type = MessageType.GOT_HIT;
        HIT hit;
        hit.seghead = seghead;
        hit.NetID = net_id;
        hit.Damage = damage;

        NetEngine.BroadcastUDP(NetworkSerializer.GetBytes<HIT>(hit), PacketUtils.MessageToStructSize[MessageType.GOT_HIT]);
    }

    public static void SendFlowMessage(flow type) {
        Debug.Log("flow type, " + type);
        if (!NetEngine.IsServer) {
            return;
        }
        SegmentHeader seghead;
        seghead.type = MessageType.FLOW_CONTROL;
        FlowMessage fm;
        fm.SegHead = seghead;
        fm.Message = (int) type;

        NetEngine.BroadcastTCP(NetworkSerializer.GetBytes<FlowMessage>(fm), PacketUtils.MessageToStructSize[MessageType.FLOW_CONTROL]);
    }
    

}
