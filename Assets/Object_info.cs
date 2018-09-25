using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_status {
    public GameObject obj;
    public int healthPoint;
    public Vector3 position;
    //public Vector3 rotation;

    public Object_status(GameObject obj, int hp) {
        this.obj = obj;
        this.healthPoint = hp;
        this.position = obj.transform.position;
        //this.rotation = obj.transform.rotation;
    }

    public void getHit(int damage) {
        healthPoint -= damage;
        if (healthPoint < 0) {
            healthPoint = 0;
        }

        Debug.Log(string.Format("Got a shot! MEDIC!. Damage Value {0}, Current HP{1}", damage, healthPoint));
    }
}
