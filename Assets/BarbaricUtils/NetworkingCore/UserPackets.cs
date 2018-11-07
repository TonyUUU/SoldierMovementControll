using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;

namespace BarbaricCode
{
    namespace Networking
    {

        public class UserDataHandler : Attribute
        {
            public int type;
            public UserDataHandler(int type)
            {
                this.type = type;
            }
        }
        public static partial class UserHandlers
        {
            public enum NetEngineUserDataTypes : int {
                HELLO_WORLD = 0,
                GOODBYE_WORLD = 1,
            }
            // user packets should start after 99

            [UserDataHandler((int)NetEngineUserDataTypes.HELLO_WORLD)]
            public static void HelloWorld(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                HelloWorldPacket hwp = NetworkSerializer.GetStruct<HelloWorldPacket>(buffer);
                Debug.Log("Hello World the secret is :" + hwp.secret);
            }

            [UserDataHandler((int)NetEngineUserDataTypes.GOODBYE_WORLD)]
            public static void GoodbyeWorld(int nodeID, int connectionID, byte[] buffer, int recieveSize)
            {
                Debug.Log("Goodbye World");
            }
        }

        public struct HelloWorldPacket {
            public int secret;
        }

    }
}
