using UnityEngine;
using System.Collections;


/// <summary>
/// This script is attached to each player and ensures that each player's movements are updated across the network
/// </summary>



public class LiftUpdate : Photon.MonoBehaviour {
	
//	public Texture viewerTexture;
//	private GameManagerVik manager;
	
	// Use this for initialization
	void Awake () 
	{
			
//		GameObject SpawnManager = GameObject.Find("Code");
//		manager = SpawnManager.GetComponent<GameManagerVik>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	/*
	void FixedUpdate()
	{
			//If the player has moved at all then fire an RPC to update across the network
		liftLastPosition = liftThisPosition;
		liftThisPosition = transform.position.y;
	}
	
	

	void OnTriggerStay(Collider other)
	{
		//Debug.Log("Stay Triggered");
		
		other.transform.position = new Vector3 (other.transform.position.x,
												other.transform.position.y + (liftThisPosition - liftLastPosition),
												other.transform.position.z);
	
	}
	
	*/
}
