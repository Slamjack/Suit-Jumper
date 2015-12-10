using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour {

	//Objects declaration
	private BoxCollider bc;
	private GameObject player;
	private Animator anim;

	//Internally modified variables
	private bool mustcrouch;

	/* Initialize */
	void Start () 
	{
		//Objects initialization
		bc = GetComponent<BoxCollider> ();
		player = GameObject.Find("Ethan");
		anim = player.GetComponent<Animator> ();
	}

	void OnTriggerEnter()
	{
		mustcrouch = true;
	}

	void OnTriggerStay()
	{
		mustcrouch = true;
	}

	void OnTriggerExit()
	{
		mustcrouch = false;
	}

	/* Update - Once per Frame */
	void Update () 
	{
		if (Input.GetButton ("Crouch")) {
			anim.SetBool("isCrouching", true);
			bc.size = new Vector3 (0.8f, 0.8f, 0.8f);
			bc.center = new Vector3 (0.0f,-0.6f,0.0f); 
		} else {
			if(!mustcrouch){
				anim.SetBool("isCrouching", false);
				bc.center = new Vector3 (0.0f,-0.2f,0.0f); 
				bc.size = new Vector3 (0.8f, 1.6f, 0.8f);
			}
		}
	}
}
