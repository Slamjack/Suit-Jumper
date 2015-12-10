using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float speed = 10F;
	public float jumpspeed = 100F;

	private Rigidbody rb;
	
	private bool Grounded = false;
	private bool HasDoubleJump = false;

	void OnCollisionEnter(Collision col)
	{
		ContactPoint contact = col.contacts[0];
		if (Vector3.Dot (contact.normal, Vector3.up) > 0.1) {
			Grounded = true;
			HasDoubleJump = true;
		}
	}

	void OnCollisionStay(Collision col)
	{
		ContactPoint contact = col.contacts[0];
		if (Vector3.Dot (contact.normal, Vector3.up) > 0.1)
			Grounded = true;
	}

	void OnCollisionExit(Collision col)
	{
		Grounded = false;
	}

	void Jump() 
	{ 
		if (Grounded)
			rb.velocity = new Vector3(0,jumpspeed,0);

		if (!Grounded && HasDoubleJump) {
			rb.velocity = new Vector3(0,jumpspeed,0);
			HasDoubleJump = false;
		}

	}

	void moveForward() 
	{
		transform.localPosition += transform.forward * speed * Time.deltaTime;
	}
	
	void moveBack() 
	{
		transform.localPosition -= transform.forward * speed * Time.deltaTime;
	}
	
	void moveRight() 
	{
		transform.localPosition += transform.right * speed * Time.deltaTime;
	}
	
	void moveLeft() 
	{
		transform.localPosition -= transform.right * speed * Time.deltaTime;
	}
	
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate ()
	{
		//Forward trigger
		if(Input.GetButton("Up"))
			moveForward();
		//Back trigger
		if(Input.GetButton("Down"))
			moveBack();
		//Right trigger
		if(Input.GetButton("Right"))
			moveRight();
		//Left trigger
		if(Input.GetButton("Left"))
			moveLeft();

		// Jump trigger
		if (Input.GetButtonDown("Jump"))
			Jump();
	}
}
