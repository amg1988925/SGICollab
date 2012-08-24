using UnityEngine;
using System.Collections;

public class collabAnalytics : MonoBehaviour {
	
	static string url = "http://sgicollab.herokuapp.com";
	static string token, level, gameID, xPos, yPos, zPos;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
			
	public static void sendAnalytics(Transform player, string dataToAnalyse){
		
		//blockmake
		//plankmake
		//death
		//buttonpress
		
		//jump
		//Viewer Cam
		//Move Obj
		
		token = UserDatabase.token;
		level = GameManagerVik.nextLevel.ToString();
		gameID = GameManagerVik.gameID;
		xPos = player.position.x.ToString();
		yPos = player.position.y.ToString();
		zPos = player.position.z.ToString();		
		
		string urlconcat = "/" + dataToAnalyse +
							"?" + dataToAnalyse + "[game_id]=" + gameID +
							"&" + dataToAnalyse + "[level]=" + level +
							"&" + dataToAnalyse + "[x]=" + xPos +
							"&" + dataToAnalyse + "[y]=" + yPos +
							"&" + dataToAnalyse + "[z]=" + zPos +
							"&auth_token=" + token;
		
		var r = new HTTP.Request ("POST", url + urlconcat);
		r.Send ();		
		Debug.Log("Analytics sent online.");
		
	}
}
