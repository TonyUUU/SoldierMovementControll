using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponBase {
    // generally server only implementation?
    // nah. We'll trust the client for now
    void PrimaryFire();
    void Reload();
    int GetAmmo();
    float GetAmmoPercentage();
	void SwapMode();
	void SecondaryFire ();
}
