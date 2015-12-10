using UnityEngine;
using System.Collections;

public class FlyControl : MonoBehaviour {
	
	public float pitchspeed = 10f;
	public float rollspeed = 10f;
	public float yawspeed = 10f;
	public float movespeed = 10f;

	private float pitch;
	private float yaw;
	private float roll;

	// Use this for initialization
	void Start ()
	{
		//Empty
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Little push forward
		transform.position += transform.forward * Time.deltaTime * movespeed;
		
		
		//Inputs to pitch/yaw/roll
		transform.Rotate(-pitchspeed * Input.GetAxis("Mouse Y"),
		                 yawspeed * Input.GetAxis("Mouse X"),
		                 -rollspeed * Input.GetAxis("Horizontal"));

		//Calculate the position of the camera relative to the object
		Vector3 campos = transform.position - transform.forward * 2.0f + transform.up;

		//Bias equation for camera position
		float chasebias = 0.9f;
		Camera.main.transform.position = chasebias * Camera.main.transform.position + (1.0f - chasebias) * campos;

		//Aim camera to lead the object and copy z rotation
		Camera.main.transform.LookAt(transform.position + transform.forward * 20.0f, transform.up);

		Debug.DrawLine (transform.position,transform.position + transform.forward * 10f,Color.green,0.1f, false);

		/*
		//Get Inputs
		pitch = 0;
		yaw = 0;
		roll = 0;
		if(Input.GetAxis ("Mouse Y") > 0)
			pitch = -pitchspeed;
		if(Input.GetAxis ("Mouse Y") < 0)
			pitch = pitchspeed;
		if(Input.GetAxis ("Mouse X") > 0)
			yaw = yawspeed;
		if(Input.GetAxis ("Mouse X") < 0)
			yaw = -yawspeed;
		if(Input.GetAxis ("Horizontal") > 0)
			roll = -rollspeed;
		if(Input.GetAxis ("Horizontal") < 0)
			roll = rollspeed;

		//Establish target rotation
		Quaternion targetrot = Quaternion.Euler (pitch, yaw, roll);

		//Slerp to target rotation
		transform.localRotation = Quaternion.Slerp (transform.rotation, temoin.rotation * targetrot,0.1f);
*/
		

	}
}
