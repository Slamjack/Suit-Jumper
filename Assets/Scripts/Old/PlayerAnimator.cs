using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour {

	private Animator anim;

	/* Initialize */
	void Start () 
	{
		anim = GetComponent<Animator>();	
	}
	
	/* Update - Once per Frame */
	void Update () 
	{
		//Change MoveSpeed value according to inputs
		anim.SetFloat ("MoveSpeed", Input.GetAxis ("Vertical"));
	}
}
