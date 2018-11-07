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

    public static void SendFlowMessage(int connectionID, FlowMessageType type, byte[] packet = null, bool TCP = false) {
        packet = packet ?? new byte[0];
        SegmentHeader seghead;
        seghead.type = MessageType.USER_DATA;
        UserDataHeader udh;
        udh.MessageId = (int)type;
        udh.SegHead = seghead;
        udh.MessageSize = packet.Length;

        byte[] b1 = NetworkSerializer.GetBytes<UserDataHeader>(udh);
        byte[] b2 = packet;
        byte[] b3 = NetworkSerializer.Combine(b1, b2);
        if (TCP)
        {
            NetEngine.SendTCP(b3, b3.Length, connectionID);
        }
        else {
            NetEngine.SendUDP(b3, b3.Length, connectionID);
        }
     }

    public static void BroadCastFlowMessage(FlowMessageType type, byte[] packet = null, bool TCP = false) {
        packet = packet ?? new byte[0];
        SegmentHeader seghead;
        seghead.type = MessageType.USER_DATA;
        UserDataHeader udh;
        udh.MessageId = (int)type;
        udh.SegHead = seghead;
        udh.MessageSize = packet.Length;

        byte[] b1 = NetworkSerializer.GetBytes<UserDataHeader>(udh);
        byte[] b2 = packet;
        byte[] b3 = NetworkSerializer.Combine(b1, b2);
        if (TCP)
        {
            NetEngine.BroadcastTCP(b3, b3.Length);
        }
        else
        {
            NetEngine.BroadcastUDP(b3, b3.Length);
        }
    }



}
