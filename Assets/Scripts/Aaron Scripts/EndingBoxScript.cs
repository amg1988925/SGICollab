using UnityEngine;
using System.Collections;

public class EndingBoxScript : Photon.MonoBehaviour {
	private bool isBuilderAtEnd;
	private bool isMoverAtEnd;
	private bool isJumperAtEnd;
	private bool isViewerAtEnd;
	
	private int nextLevel;
	private bool alreadyLoading = false;
	
	public Texture completedTexture;
	public Texture incompleteTexture;
	
	private int lastFrameTime;
	private int thisFrameTime;
	private int delta;
	
//	private string statusText = "";
	private bool started = false;
	
	//This boolean tracks whether a player has reached the end.
	[HideInInspector]	
	public bool PlayersHaveReachedEnd = false;
	public bool isWaitingForNextStage = false;
	public bool finalAnalyticsSent = false;
	
	private GameManagerVik currGameManager = null;
	//private ThirdPersonControllerNET currController = null;
	
	private int ReadyCount = 0;
	private int TargetReadyCount = 0;
	
	public int levelTimeInMinutes = 1;
	
	[HideInInspector]
	public int timeLeft = -1;
	public static int clearTime;
	public static string completeStatus;
	
	public GUISkin endGameSkin;
	
	public static Hashtable clearLevelScore = new Hashtable();
	int numberOfPlayersNotUpdated = 3;
	
	[RPC]
	void callReady()
	{	
		++ReadyCount;
//		Debug.Log(ReadyCount);
	}
	
	
	[RPC]
	void SyncTimer(int currTime)
	{
		//if I have more time left than someone, then someone else's time must be correct. 
		//if (currTime < timeLeft)
		Debug.Log("Time received " + currTime);		
		
		if (timeLeft > currTime)
		{
			Debug.Log("Syncing Timer");
			timeLeft = currTime;
		}
		
//		started = true; //doesn't matter who calls it.
	}
	
	[RPC]
	void SyncOnJoin()
	{		
		//send my time left to everyone.
		//Debug.Log("RPC SyncOnJoin called");
		Debug.Log(currGameManager.selectedClass + " chosen");
	//	if (photonView.isMine)
		photonView.RPC("SyncTimer",PhotonTargets.OthersBuffered, timeLeft);		
	}
	
	
	
	void Awake()
	{
		//60 seconds in a minute, assume 30 fps.
		//timeLeft = levelTimeInMinutes * 60 * 30;	
		started = false;
	}
	
	void Start()
	{
//		clearLevelScore.Add("Clear Time", 0);
//		clearLevelScore.Add("Total Deaths", 0);
		clearLevelScore["Clear Time"] = 0;
		clearLevelScore["Total Deaths"] = 0;
	}
	
	// Use this for initialization
	void OnLevelWasLoaded () {
		ReadyCount = 0;
		isWaitingForNextStage = PlayersHaveReachedEnd = false;
		
		lastFrameTime = thisFrameTime = (int)Time.time;
		delta = 0;
		isBuilderAtEnd = false; 
		isMoverAtEnd = false;
		isJumperAtEnd = false;
		isViewerAtEnd = false;
		
		finalAnalyticsSent = false;
		
//		statusText = "Press [Spacebar] to Go To Next Stage";
				
		//Now I'll handle it here. 
		GameManagerVik.setNextLevel(Application.loadedLevel);
		currGameManager = GameObject.Find("Code").GetComponent<GameManagerVik>();
		
		if (currGameManager.level_tester_mode)
			TargetReadyCount = 1;		
		else
			TargetReadyCount = 4;
		
		timeLeft = levelTimeInMinutes * 60;
		
		//Initialise hashtable for each level ending scores
		LevelCompleteGUI.showWindow = false;
		clearLevelScore["Clear Time"] = 0;
		clearLevelScore["Total Deaths"] = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
	//	if (currGameManager.level_tester_mode) return;
	
		if (!started) 
		{
			if (currGameManager.gameStarted)
			{
				timeLeft = levelTimeInMinutes * 60;
				started = true;
				
				Debug.Log(currGameManager.selectedClass +" Calling Sync on Join");
				photonView.RPC("SyncOnJoin",PhotonTargets.Others);	
			}
		
			return;
			
		}
		if (alreadyLoading) return;
		if (PlayersHaveReachedEnd){
			clearLevelScore["Total Deaths"] = GameManagerVik.deathCount;
			
			if(ReadyCount >=TargetReadyCount)
			{
				
				//Now done in a call to GameManagerVik
				/*
				nextLevel += 1; 			
								//last level check
								if (nextLevel > (Application.levelCount - 1)) 
									nextLevel = -1;
								GameManagerVik.nextLevel = nextLevel;
									Debug.Log("nextLevel updated = "+nextLevel);
				*/
				GameManagerVik.checkNextLevel(); //automatic
				nextLevel = GameManagerVik.nextLevel;
				
				
				
				
				alreadyLoading = true;
								
								
						
					//		Playtomic.Log.LevelAverageMetric("Time", 0, Time.timeSinceLevelLoad);
					
					
						
				if (nextLevel > -1)
				{
						Application.LoadLevel(nextLevel);
						//timeLeft = levelTimeInMinutes * 60;
				}
				else
				{
						PhotonNetwork.LeaveRoom();
				}
			}
		}
		
		if (timeLeft <= 0)
		{
			timeLeft = 0;
			PlayersHaveReachedEnd = true;	
			
			//I need to do this here to prevent repeatedly sending an incomplete statement.
			if (!finalAnalyticsSent)
			{
				finalAnalyticsSent = true;
				
				clearTime = (int)((float)PhotonNetwork.time - GameManagerVik.startTime);
				completeStatus = "incomplete";
				
				clearLevelScore["Clear Time"] = "Failed";
				LevelCompleteGUI.showWindow = true;
				GetComponent<LevelCompleteGUI>().countStarsGrade(timeLeft, GameManagerVik.deathCount, GameManagerVik.gemsCollected);
			}
		}
		
		if (Input.GetKeyUp(KeyCode.Space) && PlayersHaveReachedEnd)
		{
			isWaitingForNextStage = true;
			LevelCompleteGUI.statusText = "Waiting for other players";

			photonView.RPC("callReady",PhotonTargets.AllBuffered);			
		}
	
	}
	
	void FixedUpdate()
	{
		if (!started) return;
		if (isWaitingForNextStage || PlayersHaveReachedEnd) return;
		
		
		if (currGameManager.isPaused() || !currGameManager.gameStarted) return;
		
		lastFrameTime = thisFrameTime;
		thisFrameTime = (int)Time.time;		
		delta = thisFrameTime - lastFrameTime;
	//		Debug.Log(thisFrameTime +" " + lastFrameTime + " " +photonDelta + " " + timeLeft);
		timeLeft -= delta;			
	//	Debug.Log(timeLeft);
	}

	
	 void OnTriggerEnter(Collider other) 
	{			
		if (currGameManager.level_tester_mode)
		{
			PlayersHaveReachedEnd = true;
		}
		
		else
		{
 	   	 if(other.attachedRigidbody.name.Contains("Builder"))
			{
				isBuilderAtEnd =true;	
//				int objbuilt = GameManagerVik.objectsBuilt;
//				photonView.RPC("updateObjectsBuilt", PhotonTargets.AllBuffered, objbuilt);
			}
		
			if(other.attachedRigidbody.name.Contains("Jumper"))
			{
				isJumperAtEnd =true;
			}	
	
			if(other.attachedRigidbody.name.Contains("Viewer"))
			{
				isViewerAtEnd =true;		
			}
		
			if(other.attachedRigidbody.name.Contains("Mover"))
			{
				isMoverAtEnd =true;
			}
	
			//this can remain as-is.
			if (isMoverAtEnd && isViewerAtEnd && isJumperAtEnd && isBuilderAtEnd){
				PlayersHaveReachedEnd = true;
				clearTime = (int)((float)PhotonNetwork.time - GameManagerVik.startTime);
				completeStatus = "complete";
								
				//Tally total deaths
				if(!PhotonNetwork.isMasterClient)
					photonView.RPC("tallyScoresFactors", PhotonTargets.MasterClient, GameManagerVik.deathCount);
								
				clearLevelScore["Clear Time"] = clearTime + " secs";
				LevelCompleteGUI.showWindow = true;
				
//				other.transform.GetChild(0).audio.PlayOneShot(other.GetComponentInChildren<MusicScript>().clearlevelSFX);
			}
		}
	}
		
//	[RPC]
//	void updateObjectsBuilt(int objBuilt){
//		GameManagerVik.objectsBuilt = objBuilt;			
//	}
		
	[RPC]
	void tallyScoresFactors(int deathCount, PhotonMessageInfo info){
		GameManagerVik.deathCount += deathCount;	
		numberOfPlayersNotUpdated--;
		
		if(numberOfPlayersNotUpdated == 0){
			photonView.RPC("updateTotalScoresFactors", PhotonTargets.AllBuffered, GameManagerVik.deathCount);	
		}
	}
	
	[RPC]
	void updateTotalScoresFactors(int totalDeaths, PhotonMessageInfo info){
		GameManagerVik.deathCount = totalDeaths;
		GetComponent<LevelCompleteGUI>().countStarsGrade(timeLeft, GameManagerVik.deathCount, GameManagerVik.gemsCollected);
	}
	
	void OnTriggerExit(Collider other) 
	{	 
       	//we do not disable localPlayerAtEnd here. 
		if(other.attachedRigidbody.name.Contains("Builder"))
			isBuilderAtEnd =false;
	   	if(other.attachedRigidbody.name.Contains("Jumper"))
			isJumperAtEnd =false;
		if(other.attachedRigidbody.name.Contains("Viewer"))
			isViewerAtEnd =false;
		if(other.attachedRigidbody.name.Contains("Mover"))
			isMoverAtEnd =false;
    }
	
	void OnGUI()
	{
//		if (PlayersHaveReachedEnd)
//		{	
//			GUI.skin = endGameSkin;		
//			
//				
//			//Do not send analytics here, onGUI will keep doing so anyway
//			if (timeLeft > 0)	
//			{
//				GUI.DrawTexture(new Rect (Screen.width *0.125f, Screen.height *0.125f, Screen.width * 0.75f, Screen.height * 0.75f), completedTexture, ScaleMode.StretchToFill);
//			}
//			else
//			{
//				GUI.DrawTexture(new Rect (Screen.width *0.125f, Screen.height *0.125f, Screen.width * 0.75f, Screen.height * 0.75f), incompleteTexture, ScaleMode.StretchToFill);
//
//			}
//			
//			//This will be offset to one side to avoid overlapping the chatbox. 
//			//Stats here. Note: you might want to stop stat collecting for a given stage when a player first reaches the end point.	
//			GUILayout.BeginArea(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.65f, 200, Screen.height * 0.2f));			
//	        	GUILayout.Label("Clear Time: " + GameManagerVik.startTime);			
//	        	GUILayout.Label("Deaths: " + GameManagerVik.deathCount);			
//	        	GUILayout.Label("Total Objects Built: " + GameManagerVik.objectsBuilt);	
//			GUILayout.EndArea();
//			
//			GUI.Label(	new Rect (Screen.width *0.5f - 150, Screen.height *0.8f, 400, Screen.height * 0.1f), statusText);
//				
//		}	
	
	}

}
