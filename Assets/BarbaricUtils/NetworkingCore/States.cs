using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BarbaricCode {
    namespace Networking {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SimpleState
        {
            public StateDataMessage StateHeader;
            public Vector3 Position;
            public Quaternion Rotation;
        }

    }
}