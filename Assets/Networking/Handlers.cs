using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the partial handler lets you define parts of a class separately
// we use this here to separate core NetEngine functions
// from user specified
namespace BarbaricCode
{
    namespace Networking
    {
        public static partial class Handlers
        {
            // in order for your handler to get registered
            // it must have this attribute
            // [SegHandle(type of message)]
            // And it must have this method signature
            // (int nodeID, int connectionID, byte[] buffer, int recieveSize)
            // After that it will be automatically registered at runtime
            [SegHandle(MessageType.GOT_HIT)]
            public static void Hit(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                HIT hit = NetworkSerializer.ByteArrayToStructure<HIT>(buffer);
                NetEngine.NetworkObjects[hit.NetID].gameObject.GetComponent<Damageable>().getHit(hit.Damage);
            }
        }
    }
}
