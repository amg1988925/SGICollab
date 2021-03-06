using UnityEngine;
using System.Collections;

public class collabAnalytics : MonoBehaviour {
	
	static string url = "http://sgicollab1.herokuapp.com";
	public static string token, level, gameID, xPos, yPos, zPos;
	public static bool sendEnabled;
	
	// Use this for initialization
	void Start () {
		sendEnabled = GetComponent<GameManagerVik>().sendAnalytics;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
			
	public static bool sendAnalytics(Transform player, string dataToAnalyse, bool reliable){
		
		if(sendEnabled){
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
			if(reliable){
				while (!r.isDone) {
						if (r.exception != null) {
							Debug.Log (r.exception.ToString ());
					}
				}
				
				if (r.exception != null) {
					Debug.Log (r.exception.ToString ());
				} else {
					Debug.Log(r.response.Text);	
					Debug.Log("Analytics sent online.");
					return true;
				}
			}
		}
		
		return false;
	}
	
	public static bool sendScoreFactorData(int clearTime, string completeStatus, int starsGrade){
		if(sendEnabled){
			level = GameManagerVik.nextLevel.ToString();
			token = UserDatabase.token;
			gameID = GameManagerVik.gameID;
			
			string urlconcat = "/game" + 
								"/" + gameID +
								"?auth_token=" + token +
								"&game[level]=" + level +
								"&game[completed]=" + completeStatus +
								"&game[cleartime]=" + clearTime +
								"&game[stars]=" + starsGrade;
											
			var r = new HTTP.Request ("PUT", url + urlconcat);
			r.Send ();	
			while (!r.isDone) {
					if (r.exception != null) {
						Debug.Log (r.exception.ToString ());
				}
			}
			
			if (r.exception != null) {
				Debug.Log (r.exception.ToString ());
			} else {
				Debug.Log(r.response.Text);	
				Debug.Log("Clear time analytics sent online.");
				return true;
			}
		}
		return false;
	}
	
}
