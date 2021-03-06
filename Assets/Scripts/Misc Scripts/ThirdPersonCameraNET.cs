using UnityEngine;
using System.Collections;


public class ThirdPersonCameraNET : MonoBehaviour
{
	
	private bool menuOn = false;
	public Collider target;
		// The object we're looking at
	new public Camera camera;
	
	public GameObject[] cameras;
	private GameObject[] viewerCams;
	GameManagerVik MoverTest; 
	
	public int defaultCameraIndex; //for a fast reset.
	public static int currCameraIndex; //current game camera view.
		// The camera to control
	public LayerMask obstacleLayers = -1, groundLayers = -1;
		// Which layers should count as obstructing the view? And which are designated ground?
		// NOTICE: Make sure that the target collider is not in any of these layers!
	public float groundedCheckOffset = 0.7f;
		// Tweak so check starts from just within target footing
	public float rotationUpdateSpeed = 60.0f,
		lookUpSpeed = 20.0f,
		distanceUpdateSpeed = 10.0f,
		followUpdateSpeed = 10.0f;
		// Tweak these to adjust camera responsiveness
	public float maxForwardAngle = 40.0f;
		// Tweak to adjust camera clamping angle - specifies the maximum angle between target and clamped camera forward
	public bool
		showGizmos = true,
			// Turn this off to reduce gizmo clutter if needed
		requireLock = true,
			// Turn this off if the camera should be controllable even without cursor lock
		controlLock = false;
			// Turn this off if you want mouse lock controlled elsewhere
	
	public float minDistance = 0.0f,
		maxDistance = 3.0f,
		zoomSpeed = 0.5f;
		// Tweak to adjust scrollwheel zoom	
		
	private const float movementThreshold = 0.1f, rotationThreshold = 0.1f;
		// Tweak these to adjust camera responsiveness
	private const float groundedDistance = 0.5f;
		// Tweak if the camera goes into ground mode too soon or late
		
		
	private Vector3 lastStationaryPosition;
	private float optimalDistance, targetDistance;
	private bool grounded = false;
	
	
	void Reset ()
	// Run setup on component attach, so it is visually more clear which references are used
	{
		Setup ();
		//assuming that there's already a camera on the scene, this ensures that the camera index is the starting camera index. Should trigger when transitioning stages.
		if (MoverTest != null)
			if (MoverTest.selectedClass == "Viewer")
					ResetCamera();
	}
	
	
	void Setup ()
	// If target and/or camera is not set, try using fallbacks
	{
		if (target == null)
		{
			target = GetComponent<Collider> ();
		}
		
		if (camera == null)
		{
			if (Camera.main != null){				
				GameObject[] tempCams = GameObject.FindGameObjectsWithTag("ViewerCamera");
				
				viewerCams = new GameObject[tempCams.Length];
				int currID;
				for(int x = 0; x < tempCams.Length; x++){
					currID = tempCams[x].GetComponent<ViewerCamIndex>().myID;
					viewerCams[currID - 1] = tempCams[x];
				}
				
				cameras = new GameObject[viewerCams.Length + 1];
				cameras[0] = GameObject.Find("Main Camera");
				
				for (int i = 0; i < viewerCams.Length; ++i)
				{
					cameras[i + 1] = viewerCams[i];	
					(cameras[i + 1].GetComponent<Camera>() as Camera).enabled = false;
				}
				currCameraIndex = defaultCameraIndex = FindMainCameraIndex();
				
				camera = cameras[defaultCameraIndex].GetComponent<Camera>() as Camera;
				
				GameObject SpawnManager = GameObject.Find("Code");
				MoverTest = SpawnManager.GetComponent<GameManagerVik>();
			}
		}
	}
	
	
	public void LoadCameras()
	{
			GameObject[] tempCams = GameObject.FindGameObjectsWithTag("ViewerCamera");

				
				viewerCams = new GameObject[tempCams.Length];
				int currID;
				for(int x = 0; x < tempCams.Length; x++){
					currID = tempCams[x].GetComponent<ViewerCamIndex>().myID;
					viewerCams[currID - 1] = tempCams[x];
				}
				
				cameras = new GameObject[viewerCams.Length + 1];
				cameras[0] = GameObject.Find("Main Camera");
				
				for (int i = 0; i < viewerCams.Length; ++i)
				{
					cameras[i + 1] = viewerCams[i];	
					(cameras[i + 1].GetComponent<Camera>() as Camera).enabled = false;
				}
				currCameraIndex = defaultCameraIndex = FindMainCameraIndex();
				
				camera = cameras[defaultCameraIndex].GetComponent<Camera>() as Camera;
				
				GameObject SpawnManager = GameObject.Find("Code");
				MoverTest = SpawnManager.GetComponent<GameManagerVik>();
	}
	
	//finds a camera called "MAIN CAMERA" in the scene. This will be used to move the 
	int FindMainCameraIndex()
	{
		int targetIndex = 0;
		
		foreach (GameObject c in cameras)
		{
			if (c.name.Contains("Main Camera"))
				return targetIndex;
			targetIndex++;
		}
		
		return targetIndex;
	
	}
	
	
	int SwitchCamera(int i)
	{
		(cameras[currCameraIndex].GetComponent<Camera>() as Camera).enabled = false;
			currCameraIndex = i-1;
		
		if(currCameraIndex >= cameras.Length || currCameraIndex < 0)
		{
			currCameraIndex = 0;
		}
		
		//print("currCameraIndex = " + currCameraIndex);
		Debug.Log("Length of array is" +cameras.Length);
		Debug.Log("current index is"+currCameraIndex);
		if(currCameraIndex > 0 )
		{			
			GameObject buttonsForDoors = GameObject.FindGameObjectWithTag("SwitchForDoor");
			if (buttonsForDoors)
			{
			DoorTriggerScript thisScript = buttonsForDoors.GetComponent<DoorTriggerScript>();
			
			thisScript.showColours();
			}		
			GameObject buttonsForLifts = GameObject.FindGameObjectWithTag("SwitchForLift");
			if (buttonsForLifts)
			{
			triggerCsScript thatScript = buttonsForLifts.GetComponent<triggerCsScript>();
			
			thatScript.showColours();
			}
		}
		else
		{
			GameObject buttonsForDoors = GameObject.FindGameObjectWithTag("SwitchForDoor");
			if (buttonsForDoors)
			{
			DoorTriggerScript thisScript = buttonsForDoors.GetComponent<DoorTriggerScript>();
			thisScript.hideColours();
			}		
			GameObject buttonsForLifts = GameObject.FindGameObjectWithTag("SwitchForLift");
			if (buttonsForLifts)
			{
			triggerCsScript thatScript = buttonsForLifts.GetComponent<triggerCsScript>();
			thatScript.hideColours();
			}
		}
		(cameras[currCameraIndex].GetComponent<Camera>() as Camera).enabled = true;
		
		return currCameraIndex;
	}
	
	void ResetCamera()
	{
		/*
		currCameraIndex = defaultCameraIndex;
		(cameras[defaultCameraIndex].GetComponent<Camera>() as Camera).enabled = true;
		for (int i = 0; i < cameras.Length; ++i)
		{
			if (i != defaultCameraIndex)
				(cameras[i].GetComponent<Camera>() as Camera).enabled = false;
			
		}*/
		
		int curCam = SwitchCamera(1);
					
		
	}

	void Start ()
	// Verify setup, initialise bookkeeping
	{
		Setup ();
			// Retry setup if references were cleared post-add
				
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		
		if (camera == null)
		{
			Debug.LogError ("No camera assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		lastStationaryPosition = target.transform.position;
		targetDistance = optimalDistance = (camera.transform.position - target.transform.position).magnitude;
	}
	
	float ViewRadius
	// The minimum clear radius between the camera and the target
	{
		get
		{
			//THIS sets the minimum camera distance to something far enough from the character to see the character. We don't need something like that right now. 
			/*
			float fieldOfViewRadius = (optimalDistance / Mathf.Sin (90.0f - camera.fieldOfView / 2.0f)) * Mathf.Sin (camera.fieldOfView / 2.0f);
				// Half the width of the field of view of the camera at the position of the target
			float doubleCharacterRadius = Mathf.Max (target.bounds.extents.x, target.bounds.extents.z) * 2.0f;
			
			return Mathf.Min (doubleCharacterRadius, fieldOfViewRadius);
			*/
			return 0; 
		}
	}
	
	
	Vector3 SnappedCameraForward
	// The camera forward vector, clamped to the target forward vector so only horizontal rotation is kept
	{
		get
		{
			Vector2 planeForward = new Vector2 (camera.transform.forward.x, camera.transform.forward.z);
			planeForward = new Vector2 (target.transform.forward.x, target.transform.forward.z).normalized *
				planeForward.magnitude;
			return new Vector3 (planeForward.x, camera.transform.forward.y, planeForward.y);
		}
	}
	
	
	void FixedUpdate ()
	// See if the camera touches the ground and adjust the camera distance if an object blocks the view
	{
		grounded = Physics.Raycast (
			camera.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
		);
			// Shoot a ray downward to see if we're touching the ground
		
		Vector3 inverseLineOfSight = camera.transform.position - target.transform.position;
		RaycastHit hit;
		Vector3 rayStartPoint = target.transform.position;// +target.transform.forward;

		Debug.DrawRay(rayStartPoint, inverseLineOfSight);

		if (Physics.SphereCast (rayStartPoint, ViewRadius, inverseLineOfSight, out hit, optimalDistance, obstacleLayers))
		// Cast a sphere from the target towards the camera - using the view radius - checking against the obstacle layers
		{
			targetDistance = Mathf.Min ((hit.point - rayStartPoint).magnitude, optimalDistance);
			
				// If something is hit, set the target distance to the hit position
		}
		else
		{
			targetDistance = optimalDistance;
				// If nothing is hit, target the optimal distance
		}
	}
	
	
	void Update ()
	// Update optimal distance based on scroll wheel input
	{
		if (MoverTest)
		{
			
			if (MoverTest.selectedClass == "Viewer")
			{
				
				if (Input.GetKeyDown("1"))
				{
					SwitchCamera(2);
				/*
				if(curCam > 0 ){			
					GameObject buttonsForDoors = GameObject.FindGameObjectWithTag("SwitchForDoor");
					DoorTriggerScript thisScript = buttonsForDoors.GetComponent<DoorTriggerScript>();
					thisScript.showColours();
					
					GameObject buttonsForLifts = GameObject.FindGameObjectWithTag("SwitchForLift");
					triggerCsScript thatScript = buttonsForLifts.GetComponent<triggerCsScript>();
					thatScript.showColours();
				}else{
					GameObject buttonsForDoors = GameObject.FindGameObjectWithTag("SwitchForDoor");
					DoorTriggerScript thisScript = buttonsForDoors.GetComponent<DoorTriggerScript>();
					thisScript.hideColours();
					
					GameObject buttonsForLifts = GameObject.FindGameObjectWithTag("SwitchForLift");
					triggerCsScript thatScript = buttonsForLifts.GetComponent<triggerCsScript>();
					thatScript.hideColours();
				}*/
				}
				if (Input.GetKeyUp("1"))
				{
					ResetCamera();	
				}

				
				if (Input.GetKeyDown("2"))
				{
					SwitchCamera(3);
				}
				if (Input.GetKeyUp("2"))
				{
					ResetCamera();	
				}
			
				if (Input.GetKeyDown("3"))
				{
					SwitchCamera(4);		
				}
				if (Input.GetKeyUp("3"))
				{
					ResetCamera();	
				}

				
				if (Input.GetKeyDown("4"))
				{
					SwitchCamera(5);		
				}
				if (Input.GetKeyUp("4"))
				{
					ResetCamera();	
				}

			
				if (Input.GetKeyDown("5"))
				{
					SwitchCamera(6);
				}
				if (Input.GetKeyUp("5"))
				{
					ResetCamera();	
				}

				if (Input.GetKeyDown("6"))
				{
					SwitchCamera(7);
				}
				if (Input.GetKeyUp("6"))
				{
					ResetCamera();	
				}

				if (Input.GetKeyDown("7"))
				{
					SwitchCamera(8);
				}
				if (Input.GetKeyUp("7"))
				{
					ResetCamera();	
				}

				if (Input.GetKeyDown("8"))
				{
					SwitchCamera(9);
				}
				if (Input.GetKeyUp("8"))
				{
					ResetCamera();	
				}

				
			}
			
		
		}
		
		
		optimalDistance = Mathf.Clamp (
			optimalDistance,// + Input.GetAxis ("Mouse ScrollWheel") * -zoomSpeed * Time.deltaTime,
			minDistance,
			maxDistance
		);
		
		/*
		RaycastHit hit;
		if (Physics.SphereCast (target.transform.position, ViewRadius, inverseLineOfSight, out hit, optimalDistance, obstacleLayers))
		// Cast a sphere from the target towards the camera - using the view radius - checking against the obstacle layers
		{
			optimalDistance = Mathf.Min ((hit.point - target.transform.position).magnitude, optimalDistance);
			
			
				// If something is hit, set the target distance to the hit position
		}*/
				//turn off an on menu
		if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.Escape))
		{
			menuOn = !menuOn;
		}

	}
	

	void LateUpdate ()
	// Update camera position - specifics are delegated to camera mode functions
	{		
		
		
		
		//swapped
		if (
			//(Input.GetMouseButton (1)) &&	// Act if a mouse button is down
			menuOn &&
			(!requireLock || controlLock || Screen.lockCursor)			// ... and we're allowed to
		)
		{
			/*
			if (controlLock)
			{
				Screen.lockCursor = false;
			}*/
			//return;
			/*
			 //do not move if menu is on!
			Vector3 movement = target.transform.position - lastStationaryPosition;
			if (new Vector2 (movement.x, movement.z).magnitude > movementThreshold)
			// Only update follow camera if we moved sufficiently
			{
				FollowUpdate ();
			}
			*/
		
		}
		else
		{
			/*if (controlLock)
			{
				Screen.lockCursor = true;
			}*/
			
			//FreeUpdate ();
			FollowUpdate();
			lastStationaryPosition = target.transform.position;
				// Update the stationary position so we don't get an immediate snap back when releasing the mouse button
	
			
		}
		
		DistanceUpdate ();
	}
	
	
	void FollowUpdate ()
	// Have the camera follow behind the character
	{
		if (menuOn) return;
		
		Vector3 cameraForward = target.transform.position - camera.transform.position;
		
		cameraForward = new Vector3 (cameraForward.x, 0.0f, cameraForward.z);
			// Ignore camera elevation when calculating the angle
		
		float rotationAmount = Vector3.Angle (cameraForward, target.transform.forward);
		
		if (rotationAmount < rotationThreshold)
		// Stop rotating if we're within the threshold
		{
			lastStationaryPosition = target.transform.position;
		}
		
		rotationAmount *= followUpdateSpeed * Time.deltaTime;
		
		if (Vector3.Angle (cameraForward, target.transform.right) < Vector3.Angle (cameraForward, target.transform.right * -1.0f))
		// Rotate to the left if the camera is to the right of target forward
		{
			rotationAmount *= -1.0f;
		}
		
		camera.transform.RotateAround (target.transform.position, Vector3.up, rotationAmount);
	}
	
	
	void FreeUpdate ()
	// Control the camera via the mouse
	{
		float rotationAmount;
		
		// Horizontal rotation:
		//swapped
		
		if (menuOn)
		// If left mouse button it held, do horizontal rotation
		{
			/*
			rotationAmount = Input.GetAxis ("Mouse X") * rotationUpdateSpeed * Time.deltaTime;
			camera.transform.RotateAround (target.transform.position, Vector3.up, rotationAmount);
			*/
			rotationAmount = 0;
		}
		else
		// If right mouse button is held, don't rotate horizontally - the character should do that
		
		{
			FollowUpdate ();
			rotationAmount = Input.GetAxis ("Mouse Y") * -1.0f * lookUpSpeed * Time.deltaTime;
		}
		
		// Vertical rotation:
		
		
			// Calculate vertical rotation
		
		bool lookFromBelow = Vector3.Angle (camera.transform.forward, target.transform.up * -1) >
			Vector3.Angle (camera.transform.forward, target.transform.up);
			// Is the camera looking up at the target?

		//We don't actually need the grounded && lookfrombelow check. Surprise surprise. 
		/*
		if (grounded &&	lookFromBelow)
		// If we're grounded and look up from this position - applying the vertical rotation to the camera pivot point
		{
		
			camera.transform.RotateAround (camera.transform.position, camera.transform.right, rotationAmount);
			//camera.transform.LookAt (target.transform.position);
			
			float forwardAngle = Vector3.Angle (target.transform.forward, SnappedCameraForward);
				// Get the new rotation relative to the target forward vector
			if (forwardAngle > 0)
			// If the new rotation brought the camera over the clamp max, rotate it back by the difference
			{
				camera.transform.RotateAround (
					target.transform.position,
					camera.transform.right,
					lookFromBelow ? forwardAngle - 0 : maxForwardAngle - 0
				);
			}
			
		}
		else
		// If we're not grounded, apply the vertical rotation to the target pivot point
		{*/
			camera.transform.RotateAround (target.transform.position, camera.transform.right, rotationAmount);
			camera.transform.LookAt (target.transform.position);
				// Apply rotation and keep looking at the target
			
			float forwardAngle = Vector3.Angle (target.transform.forward, SnappedCameraForward);
				// Get the new rotation relative to the target forward vector
			if (forwardAngle > maxForwardAngle)
			// If the new rotation brought the camera over the clamp max, rotate it back by the difference
			{
				camera.transform.RotateAround (
					target.transform.position,
					camera.transform.right,
					lookFromBelow ? forwardAngle - maxForwardAngle : maxForwardAngle - forwardAngle
				);
			}
		//}
	}
	
	
	void DistanceUpdate ()
	// Apply any change in camera distance
	{
		Vector3 targetPosition = target.transform.position + (camera.transform.position - target.transform.position).normalized * targetDistance;
		camera.transform.position = Vector3.Lerp (camera.transform.position, targetPosition, Time.deltaTime * distanceUpdateSpeed);
	}
	
	
	void OnDrawGizmosSelected ()
	// Use gizmos to gain information about the state of your setup
	{
		if (!showGizmos || target == null || camera == null)
		{
			return;
		}
		
		Gizmos.color = Color.green;
		Gizmos.DrawLine (target.transform.position, target.transform.position + target.transform.forward);
			// Visualise the target forward vector
		
		Gizmos.color = grounded ? Color.blue : Color.red;
		Gizmos.DrawLine (camera.transform.position + target.transform.up * -groundedCheckOffset,
			camera.transform.position + target.transform.up * -(groundedCheckOffset + groundedDistance));
			// Visualise the camera grounded check and whether or not it is grounded
			
		Gizmos.color = Color.green;
		Gizmos.DrawLine (camera.transform.position, camera.transform.position + camera.transform.forward);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine (camera.transform.position, camera.transform.position + SnappedCameraForward);
			// Visualise the camera forward vector (green) vs. the SnappedCameraForward
	}
}
