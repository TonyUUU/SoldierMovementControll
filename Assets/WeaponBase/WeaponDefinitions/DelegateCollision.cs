using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DelegateCollision : MonoBehaviour
{
	void OnCollisionEnter(Collision c)
	{
		var other = c.gameObject;
		if (other.CompareTag("Damageble"))
		{
			//do some damage on damageble 
		}
		Destroy(gameObject);
	}
}

