using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraScript : MonoBehaviour
{

	// Variables
	//player
	public Transform player;
	//sensitivity
	public float mouseSensitivity;
	//matematika za kasnije
	float cameraVerticalRotation = 0f;
	// mod u kojem je kamera, first ili third ili second person
	int CameraMode = 1;
	void Start()
	{

	}
	//NE OBRAČAJ PAŽNJU NA OVO SAD!!!!!!!!
	/*public void ChangeCameraPers() 
	{
		CameraMode++;
		if (CameraMode > 3) 
		{
			CameraMode = 1;
			//player.Rotate(0, 180f, 0);
			LookSphere.transform.Rotate(0, 180f, 0);
		}
		if (CameraMode == 3) 
		{
			player.Rotate(0, 180f, 0);
			LookSphere.transform.Rotate(0, 180f, 0);
		}
	}*/
	void FixedUpdate()
	{		
		if (Input.GetKeyDown(KeyCode.E)) 
		{
			//ne treba za sad
			//ChangeCameraPers();
		}
		
		
			//LOKA MIŠ U SREDINI SCREENA I STAVLJA DA JE INVISIBLE
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			// Collect Mouse Input
			float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
			float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;
			
					
			//DOBIVA INPUT OD MIŠA ZA ROTACIJU U I STAVLJU JU U VARIABLU
			cameraVerticalRotation -= inputY;
			//OGRANIČAVA ROTACIJU ZA 90 STUPNJEVA GORE I DOLJE
			cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
			
			
			if (CameraMode == 1) 
			{
				player.transform.eulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
				//rotira kameru
				transform.eulerAngles = new Vector3(cameraVerticalRotation, transform.eulerAngles.y + inputX, 0);
				//rotira playera kao kameru
				
				//smješta kameru
				transform.localPosition = new Vector3(0, 1f, 0);
			}
			
			
			//OVO TU JE AKO ČEMO MIJENJATI IZ FIRST U THIRD PERSON I TAKO DALJE...
			/*
			if (CameraMode == 2) 
			{
				transform.LookAt(player);
				transform.position = EndOfCube.transform.position;
			}
			if (CameraMode == 3) 
			{
				transform.LookAt(player);
				transform.position = EndOfCube.transform.position;
			}*/
	}
}
