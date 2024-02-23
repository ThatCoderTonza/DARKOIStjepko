using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMentScript : MonoBehaviour
{
	
	
	//-----------------PLAYER VARIABLES---------------------------
	//BRZINA KOJOM SE PLAYER KREĆE NEKA BUDE PLAYERSPEED
	//MORA BITI FLOAT JER UNITY UVIJEK RADI S FLOAT KADA MNOŽI S VEKTOROM
	public float PlayerSpeed = 5;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		//kada player klikne jedan od četri gumba ide napred nazad lijevo desno 
		if  (Input.GetKey(KeyCode.W)) 
		{
			transform.Translate(Vector3.forward / PlayerSpeed);
		}
		if  (Input.GetKey(KeyCode.S)) 
		{
			transform.Translate(Vector3.back / PlayerSpeed);
		}
		if  (Input.GetKey(KeyCode.A)) 
		{
			transform.Translate(Vector3.left / PlayerSpeed);
		}
		if  (Input.GetKey(KeyCode.D)) 
		{
			transform.Translate(Vector3.right / PlayerSpeed);
		}
	}
}
