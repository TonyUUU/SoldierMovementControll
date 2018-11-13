using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// very important that shooting is fun
public class SimpleGun : MonoBehaviour, WeaponBase
{
	public int clipSize = 20;
	private int currentAmmo = clipSize;
	public double damagePerAmmo = 2.0;
	public double weaponRange = 20.0;
	private enum fireMode {
		SINGLE,
		BURST
	}
	private fireMode curMode = fireMode.SINGLE;

    void WeaponBase.Fire()
    {
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, fwd, out hit, distance)) {
			double distance = Vector3.Distance (hit.collider.transform.position, transform.position);
			//damage fall off bases on range
			if (distance > weaponRange && distance <= (2 * weaponRange)) {
				damagePerAmmo = 1.0;
			} else if (distance > (2 * weaponRange)) {
				damagePerAmmo = 0.0;
			}
			switch (curMode)
			{
				case fireMode.SINGLE:
					// damamgeable got hit only once, call getHit() once
					break;	
				case fireMode.BURST:
					// create a timer, for every 0.5 secs, call getHit() once
					break;	
			}
		}

    }

	void WeaponBase.SwapMode()
	{
		curMode = (curMode + 1) % 2;
	}

    int WeaponBase.GetAmmo()
    {
		return currentAmmo;
    }

    float WeaponBase.GetAmmoPercentage()
    {
		return (float) currentAmmo / clipSize;
    }

    void WeaponBase.Reload()
    {
		currentAmmo = clipSize;
    }

}
