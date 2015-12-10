using UnityEngine;
using System.Collections;

public class PlayerMoveScript: MonoBehaviour {

	//Externally modified variables
	public float groundspeed = 10.0f;		//Normal moving speed multiplier
	public float crouchslow = 0.5f;			//Crouch slow multiplier
	public float smoothfactorbasic = 10.0f;	//Normal moving speed multiplier
	public float turnspeed = 10.0f;			//Mouse turn sensivity
	public float jumpheight = 1.2f;			//Jump height
	public float airspeed = 1.0f;			//Air acceleration multiplier
	public float maxairacceleration = 1.0f;	//Maximal acceleration in the air
	public float sizeratio = 1.0f;			//Size multiplier of player
	public float debugtime = 0.2f;			//Life time of debug rays
	public int PossibleJumps = 1;			//Number of jumps
	public bool RawInputs = false;			//Raw or smooth movements
	public bool PermaJump = false;			//Jump when holding key

	//Internally modified variables
	private float speedmodifier = 1.0f;	//Default player speed modifier
    private float smoothfactor;         //Stores smoothfactor
	private float vel;                  //Velocity meter
    private int jumpsleft;				//Stores jumps left
	private int incrouchtrigger = 0;	//Stores number of collider in crouch trigger
	private bool mustcrouch;			//Forces crouching
	private bool isCrouching;			//Stores crouching state
	private bool isGrounded;			//Stores grounded state

	//Objects declaration
	private Rigidbody rb;	//Player rigidbody
	private GameObject ec;	//Player collider

	//Subobjects declaration
	private BoxCollider bc;		//Player crouching trigger
	private GameObject player;	//Player model (to acces player components)
	private GameObject ct;		//Camera target
	private Animator anim;		//Animator

	//Layers
	public LayerMask defaultlayer;

	/* Initialize */
	void Start () 
	{
		//Objects initialization
		rb = GetComponent<Rigidbody> ();
		ec = GameObject.Find ("PlayerEmptyCollider");

		//Subobjects initialization
		bc = ec.GetComponent<BoxCollider> ();
		player = GameObject.Find("Ethan");
		ct = GameObject.Find ("CameraTarget");
		anim = player.GetComponent<Animator> ();
	}

	/* TRIGGER ENTER */
	void OnTriggerEnter()
	{
		incrouchtrigger++;	//Add one element to crouch trigger
	}

	/* TRIGGER EXIT */
	void OnTriggerExit()
	{
		incrouchtrigger--;	//Remove one element from crouch trigger
	}

	/* COLLISION ENTER */
	void OnCollisionEnter(Collision col) 
	{
		ContactPoint contact = col.contacts[0];					//Get collision points
		if (Vector3.Dot (contact.normal, Vector3.up) > 0.1f) {	//Detect points positon according to normal
			jumpsleft = PossibleJumps;							//Gives jumps back
			isGrounded = true;									//Ground player
		}
	}

	/* COLLISION STAY */
	void OnCollisionStay(Collision col) 
	{
		ContactPoint contact = col.contacts[0];					//Get collision points
		if (Vector3.Dot (contact.normal, Vector3.up) > 0.1f) {	//Detect points positon according to normal
			isGrounded = true;									//Ground player
		}
	}

	/* COLLISION EXIT */
	void OnCollisionExit(Collision col) 
	{
		isGrounded = false;	//Unground player
	}

	/* SOURCE ENGINE MOVEMENT */
	void SourceMove()
	{
		Vector3 targetvelocity;			//Velocity the player will try to reach on the ground
		Vector3 acceleration;			//Acceleration the player is trying to add in the air
		Vector3 velocity = rb.velocity;	//Current elocity of the player
		Vector3 velocitychange;			//Acceleration needed to reach targetvelocity
		Vector3 projection;				//Projection needed to rule air movement

		//INPUTS
		targetvelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));	//Get inputs as vector

		targetvelocity = transform.TransformDirection(targetvelocity); 	//Change orientation of targetvelocity to local
		acceleration = targetvelocity;									//Save orientation and inputs for air movement

		//SPEED MODIFIERS
		targetvelocity *= groundspeed * speedmodifier;											//Ground speed modifiers applied
		targetvelocity = Vector3.ClampMagnitude (targetvelocity, groundspeed * speedmodifier);	//Clamp to prevent "Crossing the square" on ground
		acceleration *= airspeed;																//Air speed modifiers applied
		acceleration = Vector3.ClampMagnitude (acceleration, airspeed);							//Clamp to prevent "Crossing the square" in air

		//GROUND MOVEMENT
		if (isGrounded) {

			velocitychange = (targetvelocity - velocity) / smoothfactor;	//Calculate change needed and smoothes it
			velocitychange.y = 0;											//No vertical changge needed (Jumping does it)	
			rb.AddForce (velocitychange, ForceMode.VelocityChange);			//Move according to change needed

			Debug.DrawLine (transform.position, transform.position + targetvelocity, Color.green, debugtime, true);
			Debug.DrawLine (transform.position + velocity ,transform.position + velocity +  velocitychange, Color.blue, debugtime, true);

		//AIR MOVEMENT
		} else {

			projection = Vector3.Project(velocity,acceleration);				//Get projection of velocity/acceleration
			float proangle = Vector3.Angle (velocity,acceleration);				//Get angle beetween velocity & acceleration
			if(projection.magnitude <= maxairacceleration || proangle > 90.0f)	//If normal is not over the limit or backwards
				rb.AddForce (acceleration, ForceMode.VelocityChange);			//Move according to acceleration

			Debug.DrawLine (transform.position ,transform.position + acceleration, Color.red, debugtime, true);

		}

		Debug.DrawLine (transform.position, transform.position + velocity, Color.magenta, debugtime, true);
		Debug.DrawLine (transform.position, transform.position + transform.forward, Color.yellow, debugtime, true);
	}

	/* FREESTRAFE ENGINE MOVEMENT */
	void FreeStrafeMove()
	{
		Vector3 targetvelocity;			//Velocity the player will try to reach on the ground
		Vector3 velocity = rb.velocity;	//Current elocity of the player
		Vector3 velocitychange;			//Acceleration needed to reach targetvelocity
		
		//INPUTS
		targetvelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));	//Get inputs as vector
		
		targetvelocity = transform.TransformDirection(targetvelocity); 	//Change orientation of targetvelocity to local

		//SPEED MODIFIERS
		targetvelocity *= groundspeed * speedmodifier;											//Ground speed modifiers applied
		targetvelocity = Vector3.ClampMagnitude (targetvelocity, groundspeed * speedmodifier);	//Clamp to prevent "Crossing the square" on ground
			
		velocitychange = (targetvelocity - velocity) / smoothfactor;	//Calculate  change needed and smoothes it
		velocitychange.y = 0;											//No vertical changge needed (Jumping does it)	
		rb.AddForce (velocitychange, ForceMode.VelocityChange);			//Move according to change needed
			
		Debug.DrawLine (transform.position, transform.position + targetvelocity, Color.green, debugtime, true);
		Debug.DrawLine (transform.position + velocity ,transform.position + velocity +  velocitychange, Color.red, debugtime, true);
		Debug.DrawLine (transform.position, transform.position + velocity, Color.magenta, debugtime, true);
		Debug.DrawLine (transform.position, transform.position + transform.forward, Color.yellow, debugtime, true);
	}

	/* JUMP */
	void Jump() 
	{ 
		if (jumpsleft != 0 && !isCrouching) {
			Vector3 jumpdirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));	//Get inputs as vector
			jumpdirection = transform.TransformDirection(jumpdirection);	//Change orientation of jumpdirection to local
			jumpdirection.y = Mathf.Sqrt(2f * jumpheight * 9.81f);			//Calculate jump power needed to reach height
			jumpdirection.x *= groundspeed * speedmodifier;					//Default ground speed
			jumpdirection.z *= groundspeed * speedmodifier;					//Default ground speed
			rb.velocity = jumpdirection;									//Jump according to inputs
			jumpsleft--;													//Remove one jump from stock
		}
	}

	/* CROUCH */
	void Crouch()
	{
		isCrouching = true;																	//Crouch player
		bc.size = new Vector3 (0.8f * sizeratio, 0.8f * sizeratio, 0.8f * sizeratio);		//Resize player collider
		bc.center = new Vector3 (0.0f * sizeratio, -0.6f * sizeratio, 0.0f * sizeratio);	//Reposition player collider
		ct.transform.position = Vector3.Lerp (ct.transform.position, transform.position - transform.up * 0.5f, 0.1f);	//Translate cameratarget
	}

	/* UNCROUCH */
	void noCrouch()
	{
		//Uncrouch if possible
		if(!mustcrouch){
			isCrouching = false;																	//Uncrouch player
			bc.size = new Vector3 (0.8f * sizeratio, 1.6f * sizeratio, 0.8f * sizeratio);			//Resize player collider to full size
			bc.center = new Vector3 (0.0f * sizeratio,-0.2f * sizeratio,0.0f * sizeratio);			//Reposition player collider to center
			ct.transform.position = Vector3.Lerp (ct.transform.position, transform.position, 0.1f);	//Translate cameratarget
		}
	}

	/* MAIN UPDATE (INPUTS) */
	void Update()
	{
		//Jump trigger
		if (Input.GetButtonDown ("Jump") && !PermaJump)
			Jump ();
		if (Input.GetButton ("Jump") && PermaJump)
			Jump ();
		
		//Crouch trigger
		if (Input.GetButton ("Crouch"))
			Crouch ();
		else
			noCrouch();
	}

	/* FIXED UPDATE (PHYSICS) */
	void FixedUpdate () 
	{
		//Force crouching if collider in crouch trigger
		if (incrouchtrigger > 0)
			mustcrouch = true;
		else
			mustcrouch = false;

        //RawInputs

        //Initialisation
        if (RawInputs)
            smoothfactor = 1.0f;
        else
            smoothfactor = smoothfactorbasic;

        //Reset speed to default and reapply modifiers
        speedmodifier = 1.0f;
		if(isGrounded && isCrouching)
			speedmodifier *= crouchslow;	//Apply crouch slow

		//Rotate player according to mouse X
		rb.transform.Rotate(0.0f,turnspeed * Input.GetAxis("Mouse X"),0.0f);
			
		//Moves like Jagger
		SourceMove();
	}

	/* LATE UPDATE */
	void LateUpdate()
	{
		RaycastHit hit;	//Used to store camera raycast data

		Vector3 campos = ct.transform.position - ct.transform.forward * 3.0f;	//Get new camera position

		if(Physics.Raycast(ct.transform.position, - ct.transform.forward , out hit, 3f, defaultlayer))	//Detects camera collision
			campos =  hit.point + ct.transform.forward * 0.5f;											//Change position to collision point
		Camera.main.transform.position = campos;														//Moves camera to posiion
		Camera.main.transform.LookAt (ct.transform.position);											//Look at camera target

		ec.transform.rotation = Quaternion.identity;	//Fix player collider in space

		//Update animation parameters according to inputs
		anim.SetFloat ("MoveSpeed", Input.GetAxis ("Vertical"));
		anim.SetBool ("isGrounded", isGrounded);
		anim.SetBool("isCrouching", isCrouching);
		anim.SetFloat("Jumpvelocity", rb.velocity.y);

		//Debug
		Debug.DrawLine (ct.transform.position,ct.transform.position - ct.transform.forward*3f, Color.blue, 0f, true);
		vel = (int)Mathf.Pow (Mathf.Pow (rb.velocity.x * 100, 2f) + Mathf.Pow (rb.velocity.y * 100, 2.0f) + Mathf.Pow (rb.velocity.z * 100, 2f), 0.5f);
	}

}
