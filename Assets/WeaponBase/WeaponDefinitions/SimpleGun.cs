using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// very important that shooting is fun
public class SimpleGun : MonoBehaviour, WeaponBase
{
	public int clipSize = 20;
	private int currentAmmo = 20;
	public double damagePerAmmo = 2.0;
	public double weaponRange = 20.0;
	public double secondaryWeaponDamage = 40.0;
	public double missileVelocity = 20.0f; //debatable, havent tested it yet, tune it up later
	public double missileDisappearDistance = 200.0f; //ditto
	public float recoilFactor = 0.1f; //ditto
	private enum fireMode {
		SINGLE,
		BURST
	}
	private fireMode curMode = fireMode.SINGLE;

    void WeaponBase.PrimaryFire()
    {

        RaycastHit hit;
        Vector3 direction = transform.forward + recoilFactor * transform.up;
        if (Physics.Raycast (transform.position, direction, out hit, (float)(weaponRange * 3.0f))) {
            Debug.DrawRay(transform.position, direction * (float)(weaponRange * 3.0f), Color.yellow, 0.001f);
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
        else
        {
            Debug.DrawRay(transform.position, direction * (float) (weaponRange * 3.0f), Color.white, 0.001f);
        }

    }

	void WeaponBase.SecondaryFire()
	{
		//unlike primaryFire, secondary attack usually can only occur once in a while
		//we can spawn a gameobject everytime secondaryFire() triggerred, and destroy it if it hit something
		//or if it doesnt hit anyting, after a amount of distance we destroy it
		GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule); // using capsule to simulate a rocket missile for now
		capsule.transform.position = transform.position;
		CapsuleCollider cc = capsule.AddComponent<CapsuleCollider> () as CapsuleCollider;
		cc.attachedRigidbody.useGravity = false; // maybe?
		capsule.AddComponent(System.Type.GetType("DelegateCollision")); //dynamically add OnCollisionEnter function
		do {
			capsule.transform.position += capsule.transform.forward * (float) (Time.deltaTime * missileVelocity);
		} // check if max distance reached or missile hit something 
		while(Vector3.Distance (capsule.transform.position, transform.position) < missileDisappearDistance && capsule != null);
	}

	void WeaponBase.SwapMode()
	{
		if (curMode == fireMode.SINGLE)
        {
            curMode = fireMode.BURST;
        }
        else
        {
            curMode = fireMode.SINGLE;
        }
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
