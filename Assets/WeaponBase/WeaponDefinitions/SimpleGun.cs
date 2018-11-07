using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// very important that shooting is fun
public class SimpleGun : MonoBehaviour, WeaponBase
{

    void WeaponBase.Fire()
    {
        // fire fire fire
    }

    int WeaponBase.GetAmmo()
    {
        return -1;
    }

    float WeaponBase.GetAmmoPercentage()
    {
        return 1f;
    }

    void WeaponBase.Reload()
    {
        // this gun has infinite ammo
    }

}
