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
        // tells the netengine to iterate through this class to grab handlers
        [HandlerClass]
        public static partial class Handlers
        {

            // Segment Handlers

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

            // Network Handlers
            // User space custom handlers
            [NetEngineHandler(NetEngineEvent.NewConnection)]
            public static void OnPlayerConnected(int nodeid, int connectionID, byte[] buffer, int recieveSize) {
                // enable the ui
                // do other things
                // asdf

                // rq spawn command
                // how to do loading/unloading of levels?



            }
        }
    }
}
