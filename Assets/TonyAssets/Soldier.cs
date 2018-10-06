using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;

// we can use this to enforce rigidbodies on the gameobject
[RequireComponent(typeof(Rigidbody))]
// The class now extents StateSychronizableMonobehaviour which extends MonoBehaviour
public class Soldier : StateSynchronizableMonoBehaviour, Damageable {

    public Object_status status;
    
    // we can probably move this out of the class
    public GameObject explosionprefab;
    // We cache the reference to rigidbody for better performance
    private Rigidbody rb;

	void Start () {
        // get the rigidbody reference
        rb = GetComponent<Rigidbody>();
        status = new Object_status(this.gameObject, 100);
	}
	
	public void getHit(int damage){

		status.healthPoint -= damage;
		if (status.healthPoint < 0) {
			status.healthPoint = 0;
            OnDie();
		}

		Debug.Log(string.Format("Soldier Got a shot! MEDIC!. Damage Value {0}, Current HP{1}", damage, status.healthPoint));
	}

    private void OnDie() {
        // net command to despawn this object
        
        // net command to spawn this explosion prefab
        // also might be good to pool these explosion prefabs
        // ehh... we can actually have this happen when the client recieves the message to despawn
        // GameObject exp = Instantiate(explosionprefab, transform.position, Quaternion.identity);
    }

    private void SpawnExplosion() {
       Instantiate(explosionprefab, transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        // there is some problem with asynchronous calling here.
        OnDie();
    }

    // The important network methods are here

    // Called when the state is recieved
    public override void Synchronize(byte[] state)
    {

        if (LocalAuthority) { return; }
        SoldierState ss = NetworkSerializer.ByteArrayToStructure<SoldierState>(state);
        status.healthPoint = ss.health;
        status.position = ss.pos;
        rb.velocity = ss.vel;
        transform.rotation = ss.rot;
        if ((transform.position - ss.pos).magnitude > NetEngineConfig.POSITION_EPSILON) {
            transform.position = ss.pos;
        }
    }

    public override byte[] GetState(out int size)
    {
        // all packets start with segment header (except for a few exceptions)
        SegmentHeader seghead;
        // the segment header tells the engine what type of packet to expect
        seghead.type = MessageType.STATE_DATA; // It's a state packet
        // all state messages start with a state_header
        StateDataMessage sdm;
        sdm.NetID = NetID;
        sdm.TimeStep = NetEngine.SimStep;
        sdm.SegHead = seghead;
        // This header tells the engine what the id is, timestep and type of state
        sdm.StateType = MessageType.SOLDIER_STATE;  // It's a soldier state!
        SoldierState ss;
        ss.health = status.healthPoint;
        ss.pos = transform.position;
        ss.rot = transform.rotation;
        ss.vel = rb.velocity;

        // The soldierstate packet must contain a seghead.
        ss.statehead = sdm;
        // The Soldierstate now looks like this
        //  Soldier_State : {[SegmentHeader]|[{StateHeader}|{SoldierInformation}]}
        // The net engine will read the segment header first, and from there can
        // Determine what kind of data is in the next segment
        // It will then read the StateHeader and determine what kind of
        // State it is synchronizing
        //  Before Reading Seghead : {[SegmentHeader]|[????]}
        //  After Reading Seghead : {[SegmentHeader]|[{StateHeader}|{???}]}
        // After Reading StateHead : {[SegmentHeader]|[{StateHeader}|{SoldierInfo}]}
        // There are some other processes involved but this is the high level idea

        // we also return as an out variable the sizeof the struct
        // using the message type and we can use the MessageToStructSize dictionary
        // To get the size in bytes of the SoldierState struct
        // The MessageToStructSize dictionary is initialized at runtime using
        // reflection
        size = PacketUtils.MessageToStructSize[MessageType.SOLDIER_STATE];
        return NetworkSerializer.GetBytes<SoldierState>(ss);
        // That should be all. The state will get sent over the network now.
    }

    // this method is required so that only
    // local authorities can give
    // state-input to objects
    public void Move(float x, float z) {
        if (!LocalAuthority) {
            return;
        }

        rb.velocity = new Vector3(x, 0, z);

    }
}
