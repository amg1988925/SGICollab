using UnityEngine;
using System.Collections;
//using System.Text;
//using System.Net;
//using System.IO;
using System.Collections.Generic;

public class UserDatabase : MonoBehaviour {
	
	static string url = "http://sgicollab1.herokuapp.com/users";
	public static string token;
	public int adminID = 2;
	public static string failedResponse = "Failed";
	
	float lastTime;
	float intervalForUserCheck = 300; //seconds
	
    void Start() {
		lastTime = Time.time;
    }
	
	void Update(){
		if(Time.time - lastTime > intervalForUserCheck){
			StartCoroutine(verifyUser());
			lastTime = Time.time;
		}
	}
	
	public bool signUp(string username, string password){
		print("Signing up...");
		
		string urlconcat ="?user[name]=" + username + 
							"&user[password]=" + password + 
							"&user[password_confirmation]=" + password + 
							"&user[maxStageReached]=5" + 
							"&user[admin_id]=" + adminID;
	
		var r = new HTTP.Request ("POST", url + urlconcat);
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
			Hashtable json = (Hashtable)JsonSerializer.Decode(r.response.Bytes);
			
			if (json.ContainsKey ("auth_token")) {
			 	token = json["auth_token"].ToString();
				return true;
			}
			else if(json.ContainsKey("error")){
				string errorMessage = "";
				if(((Hashtable)json["error"]).ContainsKey("name")){
					errorMessage = "Username has been taken!";
				}
				else if(((Hashtable)json["error"]).ContainsKey("password")){
					errorMessage = "Password too short!";
				}
				NecroGUI.showMessage(errorMessage, 2);
			}
		}	
		return false;
	}
	
	//User log in
	public bool login(string username, string password){
		print("Logging in...");
				
		string urlconcat ="/sign_in" + 
							"?user[name]=" + username + 
							"&user[password]=" + password;
		
		var r = new HTTP.Request ("POST", url + urlconcat);
		r.Send ();
		while (!r.isDone) {
				if (r.exception != null) {
					Debug.Log (r.exception.ToString ());
				return false;
			}
		}
		
		if (r.exception != null) {
			Debug.Log (r.exception.ToString ());
			return false;
		} 
		else {
			Debug.Log(r.response.Text);	
			
			Hashtable json = (Hashtable)JsonSerializer.Decode(r.response.Bytes);
			 if (json.ContainsKey ("auth_token")) {
			 	token = json["auth_token"].ToString();
			 	GameManagerVik.maxStageReached = (int)json["maxStageReached"];
				return true;
			}
		}
		return false;	
	}
	
//	public static void setData(string username, string password, string data){
//		
//		string urlconcat = "?user[name]=" + username + 
//							"&user[password]=" + password +
//							"&auth_token=" + token +
//							"&user["+ data +"]=" + data;
//		
//		var r = new HTTP.Request ("PUT", url + urlconcat);
//		r.Send ();
//		while (!r.isDone) {
//				if (r.exception != null) {
//					Debug.Log (r.exception.ToString ());
//			}
//		}
//		
//		if (r.exception != null) {
//			Debug.Log (r.exception.ToString ());
//		} else {
//			Debug.Log(r.response.Text);
//			
//		}
//	}
	
	
	public static IEnumerator verifyUser(){
		print("Verifying...");
				
		string urlconcat ="/signedin" +
							"?auth_token=" + token;
		
		var r = new HTTP.Request ("POST", url + urlconcat);
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
			
			if(r.response.Text == "not signed in")
			{
				//destroy the code object here! Otherwise when we go back to main we'll actually *duplicate* it, which is what we don't want because
				//it messes up the main menu.
				PhotonNetwork.LeaveRoom();
				Destroy(GameObject.Find("Code"));
				Application.LoadLevel(0);
			}
		}
		
		yield return null;
	}
	
	
	public string getGameID(){
		print("Getting game ID...");
				
		string level = GameManagerVik.nextLevel.ToString();
		
		string urlconcat ="http://sgicollab1.herokuapp.com/game" +
							"?auth_token=" + token +
							"&game[room_name]=" +  WWW.EscapeURL(PhotonNetwork.room.name) +
							"&game[level]=" + level + 
							"&game[admin_id]=" + adminID;
		
		print(urlconcat);
		var r = new HTTP.Request ("POST", urlconcat);
		r.Send ();
		while (!r.isDone) {
				if (r.exception != null) {
					Debug.Log (r.exception.ToString ());
			}
		}
		
		if (r.exception != null) {
			Debug.Log (r.exception.ToString ());
			return null;
		} else {
			Debug.Log(r.response.Text);	
			
			//Must inform us if there is any errors here
			if(r.response.Text.Length > 10){				
				return failedResponse;
			}
			
			return r.response.Text;
		}
		
	}
	
	public static void verifyGameID(string gameID){
		print("Setting game ID..." + gameID);
		
		string urlconcat ="http://sgicollab1.herokuapp.com/add_user_to_game" +
							"?game_id=" + gameID +
							"&auth_token=" + token;
		
		var r = new HTTP.Request ("GET", urlconcat);
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
			
			if(r.response.Text == "user not signed in"){
				PhotonNetwork.LeaveRoom();
			}
		}
	}
}