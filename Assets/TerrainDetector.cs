using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDetector : MonoBehaviour {
    int NumTerrainNearby = 0;

    private void OnTriggerEnter(Collider collision)
    {
        NumTerrainNearby++;
    }

    private void OnTriggerExit(Collider collision)
    {
        NumTerrainNearby--;
        if (NumTerrainNearby < 0) {
            Debug.LogError("Uh terrain counter is negative");
        }
    }

    public bool Airborne() {
        return NumTerrainNearby == 0;
    }

}
