using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarbaricCode.Networking;
using System.Runtime.InteropServices;

// we can use this to enforce rigidbodies on the gameobject
[RequireComponent(typeof(Rigidbody))]
// The class now extents StateSychronizableMonobehaviour which extends MonoBehaviour
public class Soldier : StateSynchronizableMonoBehaviour, Damageable {

    static int ForwardBackMask = 3; // 0011
    static int LeftRightMask = 3 << 2; // 1100
    static int FireMask = 1 << 4; // 10000
    // speed
    public float speed = 5;
    // hp
    public int hp = 100;
    // we can probably move this out of the class
    public GameObject explosionprefab;
    // We cache the reference to rigidbody for better performance
    private Rigidbody rb;

    public GameObject CameraPoint, AimPoint, HeadJoint;

    private Vector3 remotePos;
    private Vector3 remoteVel;
    private Quaternion remoteBodRot;
    private Quaternion remoteHeadRot;
    private int timeStamp = 0;
    
    // a bitfield that determines movement
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SoldierControlState {
        //          left/right 00/11 - none, 01 - left, 10 - right
        //          ||fwd/back 00/11 - none, 01 - up, 10 - down
        //          vvvv
        // ....0000 0000
        public int MoveState; 
    }

    private SoldierControlState inputState;

    void Start () {
        // get the rigidbody reference
        rb = GetComponent<Rigidbody>();
	}

    void FixedUpdate() {
        // parse local input
        Vector3 vel = Vector3.zero;
        if ((inputState.MoveState & ForwardBackMask) == 1)
        {
            vel.z = 1;
        }
        else if ((inputState.MoveState & ForwardBackMask) == 2)
        {
            vel.z = -1;
        }

        if (((inputState.MoveState & LeftRightMask) >> 2) == 1)
        {
            vel.x = -1;
        }
        else if (((inputState.MoveState & LeftRightMask) >> 2) == 2)
        {
            vel.x = 1;
        }

        if ((inputState.MoveState & FireMask) >> 4 == 1) {
            Fire();
        }

        vel *= speed;
        vel = transform.rotation * vel;
        rb.velocity = new Vector3(vel.x, rb.velocity.y, vel.z);

        // interpolate if not local auth
        if (!LocalAuthority)
        {
            // ignore remote velocity for now, captured by input state
            // sync pos, rot
            transform.rotation = Quaternion.Slerp(transform.rotation, remoteBodRot, Time.fixedDeltaTime * NetEngineConfig.INTERP_COEFF);
            HeadJoint.transform.rotation = Quaternion.Slerp(HeadJoint.transform.rotation, remoteHeadRot, Time.fixedDeltaTime * NetEngineConfig.INTERP_COEFF);
            if ((transform.position - remotePos).magnitude >= NetEngineConfig.POSITION_EPSILON)
            {
                transform.position = remotePos;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, remotePos, Time.fixedDeltaTime * NetEngineConfig.INTERP_COEFF);
            }
        }
    }

	public void getHit(int damage){
        hp -= damage;
		if (hp < 0) {
			hp = 0;
            OnDie();
		}

		Debug.Log(string.Format("Soldier Got a shot! MEDIC!. Damage Value {0}, Current HP{1}", damage, hp));
	}

    private void OnDie() {
        // send a flow message that player has died.
        NetEngine.Despawn(NetID);
    }

    private void SpawnExplosion() {
       Instantiate(explosionprefab, transform.position, Quaternion.identity);
    }

    // The important network methods are here

    // Called when the state is recieved
    public override void Synchronize(byte[] state)
    {

        if (LocalAuthority) { return; }
        SoldierState ss = NetworkSerializer.ByteArrayToStructure<SoldierState>(state);
        if (ss.statehead.TimeStep > timeStamp) {
            this.hp = ss.health;
            this.remotePos = ss.pos;
            this.remoteBodRot = ss.bodrot;
            this.remoteHeadRot = ss.headrot;
            this.inputState = ss.inputState;
            this.remoteVel = ss.vel;
            this.timeStamp = ss.statehead.TimeStep;
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
        ss.health = hp;
        ss.pos = transform.position;
        ss.bodrot = transform.rotation;
        ss.headrot = HeadJoint.transform.rotation;
        ss.vel = rb.velocity;
        ss.inputState = inputState;

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
    public void SetInputState(SoldierControlState scs) {
        if (!LocalAuthority) {
            return;
        }

        this.inputState = scs;
    }

    public override void Init()
    {
        Start();
    }

    public void FireDown() {
        inputState.MoveState = inputState.MoveState | (1 << 4);
    }

    public void FireUp() {
        inputState.MoveState = inputState.MoveState & ~(1 << 4);
    }

    private void Fire() {
        Debug.Log("Fire!");
        RaycastHit hit;
        // wrap in a weapon class
        if (Physics.Raycast(AimPoint.transform.position, AimPoint.transform.forward, out hit, int.MaxValue))
        {
            if (hit.collider.tag == "Damageable" || hit.collider.tag == "Player")
            {
                Debug.Log("Hit the target!");
                Damageable d = hit.collider.gameObject.GetComponent<Damageable>();
                d.getHit(34);
            }
        }
        else
        {
            Debug.Log("Missing target!");
        }
    }

    public void Rotate(Quaternion bodrot, Quaternion headrot) {
        transform.rotation = bodrot;
        HeadJoint.transform.localRotation = headrot;
        Transform t = HeadJoint.transform;
        float xrot = t.localRotation.eulerAngles.x;

        if (xrot > 360 - 90)
        {
            xrot = Mathf.Max(xrot, 360 - 80);
            xrot = Mathf.Min(xrot, 360);
        }
        else {
            xrot = Mathf.Min(xrot, 80);
        }

        t.localRotation = Quaternion.Euler(new Vector3(xrot, 0, 0));
    }

    public override void OnDespawn()
    {
        // there is some problem with asynchronous calling here.
        // net command to despawn this object

        // net command to spawn this explosion prefab
        // also might be good to pool these explosion prefabs
        // ehh... we can actually have this happen when the client recieves the message to despawn
        // GameObject exp = Instantiate(explosionprefab, transform.position, Quaternion.identity);

        if (!LocalAuthority)
        {
            return;
        }

        if (NetEngine.IsServer)
        {
            FlowControl.FlowHandlerMapping[flow.PLAYER_DIED].Invoke(0);
        }
        else
        {
            NetInterface.SendFlowMessage(flow.PLAYER_DIED);
        }

    }
}
