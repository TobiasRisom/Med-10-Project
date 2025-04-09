using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class FirestoreHandler : MonoBehaviour
{
	private FirebaseFirestore firestore;
	private GameObject nameList;
	
	private void Awake()
	{
		DontDestroyOnLoad(this);
		InitializeFirestore();
	}

	void InitializeFirestore()
	{
		
		// Firstly, Firebase checks and attempts to fix any asynchronous dependencies, then continues
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		           {
			           if (task.IsCompleted && task.Result == DependencyStatus.Available)
			           {
				           // Get the instance of Firebase associated with this project
				           FirebaseApp app = FirebaseApp.DefaultInstance;
				           firestore = FirebaseFirestore.GetInstance(app);
				           
				           // Successful initialization
				           Debug.Log("FireStore Initialized!");
			           }
			           else
			           {
				           // Something went wrong
				           Debug.LogError("Firebase was not successfully initialized.");
			           }
		           });
	}


	public void ScheduleManager()
	{
		
		// Reference to the document containing the weekly schedule
		DocumentReference docRef = firestore.Collection("Schedule").Document("Ugeskema");


		// Snapshot of the current items in the defined document
		docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
		{
			// "IsCompleted" on its own just means the task is done, but may have messed up and not returned anything. So we use "Result.Exists" as a backup.
			if (task.IsCompleted && task.Result.Exists)
			{
				// The snapshot contains the results of our query!
				DocumentSnapshot snapshot = task.Result;
				
				// Convert snapshot to a dictionary
				Dictionary<string, object> documentFields = snapshot.ToDictionary();

				SetSchedule(documentFields);

			}
			else
			{
				Debug.LogError("Document does not exist or error fetching document.");
			}
		});
	}

	public void SetSchedule(Dictionary<string, object> schedule)
	{
		nameList = GameObject.FindWithTag("ScheduleNames");
		List<TextMeshProUGUI> nameFields = new List<TextMeshProUGUI>();

		foreach (TextMeshProUGUI t in nameList.GetComponentsInChildren<TextMeshProUGUI>())
		{
			nameFields.Add(t);
		}

		nameFields[0].text = schedule["Mandag"].ToString();
		nameFields[1].text = schedule["Tirsdag"].ToString();
		nameFields[2].text = schedule["Onsdag"].ToString();
		nameFields[3].text = schedule["Torsdag"].ToString();
		nameFields[4].text = schedule["Fredag"].ToString();
		nameFields[5].text = schedule["Lørdag"].ToString();
		nameFields[6].text = schedule["Søndag"].ToString();
	}
}