using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;
public class HitBox : MonoBehaviour, Damageable {
    public float HitMultiplier = 1f;
    public StateSynchronizableMonoBehaviour source;
    public void getHit(int damage)
    {
        NetInterface.Fire(source.NetID, ((int)(damage * HitMultiplier)));
    }
}
