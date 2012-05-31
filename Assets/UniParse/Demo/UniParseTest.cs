using UnityEngine;
using System.Collections;

public class UniParseTest : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		
		//Register a new user
//		var user = ParseClass.users.New();
//		user.Set("username", "myName");
//		user.Set("password", "myPasss");
//		user.Create();
//		while(!user.isDone) yield return null;
//		//check for error
//		if(user.error != null) {
//			//A message is printed automatically. We can diagnose the issue by examing the HTTP code.
//			Debug.Log(user.code);
//			
//		}
//		
//		//Authenticate an existing user
		var auth_user = ParseClass.Authenticate("myName","myPasss");
		while(!auth_user.isDone) yield return null;
		//check for error
		if(auth_user.error != null) {
			Debug.Log("An error occured, likely a bad password!");
		}else{
		Debug.Log(auth_user.Get<string>("latestStage"));
		}
//		
//		//HOWTO: Create a new class
//		var commentClass = new ParseClass("/classes/Comment");
//		
//		//HOWTO: Add an instance to the class
//		var comment = commentClass.New();
//		
//		//HOWTO: Add fields to the instance.
//		comment.Set("author", "Simon Wittber");
//		comment.Set("text", "This is a wise comment.");
//		comment.Set("DOB", 1979);
//		
//		//HOWTO: Save the new instance.
//		comment.Create();
//		
//		//Wait until it's finished the save.
//		while(!comment.isDone) yield return null;
//		
//		///HOWTO: Get data back out of the instance.
//		Debug.Log(comment.createdAt);
//		Debug.Log(comment.Get<string>("author"));
//		Debug.Log(comment.Get<string>("text"));
//		
//		//HOWTO: Update a field.
//		comment.Set("author", "John Doe");
//		
//		//HOWTO: Save the updated instance.
//		comment.Update();
//		
//		//Wait until it's finished the update.
//		while(!comment.isDone) yield return null;
//		
//		
//		//This is how to add a pointer in the comment instance to a user.
//		comment.Set("user", auth_user);
//		comment.Update();
//		
//		//Wait until it's finished the update.
//		while(!comment.isDone) yield return null;
//		
//		//This is how to get the user class back from a pointer.
//		var record = comment.GetPointer("user");
//		while(!record.isDone) yield return null;
//		Debug.Log(record.Get<string>("username"));
//		
//		
//		//This is how to get all instances in a class which reference this instance in a field.
//		//Eg. Find all comments which point to auth_user via their 'user' field.
//		var comments = auth_user.FindReferencesIn(commentClass, "user");
//		while(!comments.isDone) yield return null;
//		Debug.Log(comments.items.Length);
//		
		
	}
}
