using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;

public class FirestoreHandler : MonoBehaviour
{
	private FirebaseFirestore firestore;
	public string username = "Leif";
	
	public class Task
	{
		public string Titel { get; set; }
		public string Emoji { get; set; }
		public string Description { get; set; }
		public bool ImageFormat { get; set; }
		public int Status { get; set; }
		public int Repeat { get; set; }
	}
	public List<Task> TaskData = new List<Task>(); // Stores data locally so we don't need to keep reading the database
	public List<GameObject> tasks; // Stores the tasks game objects that are spawned in the Unity scene
	public GameObject taskTemplate; // Template for all tasks shown on the main screen
	
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
			           if (task.IsCompletedSuccessfully && task.Result == DependencyStatus.Available)
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
			if (task.IsCompletedSuccessfully)
			{
				
				Debug.Log("Weekly Schedule Accessed");
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
		GameObject nameList = GameObject.FindWithTag("ScheduleNames");
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
		
		Debug.Log("Weekly schedule names added");
	}

	public async void UpdateSchedule(Dictionary<string, object> newNames)
	{
		DocumentReference doc = firestore.Collection("Schedule").Document("Ugeskema");

		var task = doc.UpdateAsync(newNames);
		await task;

		if (task.IsCompletedSuccessfully)
		{
			Debug.Log("Schedule updated!");
		}
		else
		{
			Debug.LogError("Schedule update failed.");
		}
	}

	public void AddNewUser(string newUserName)
	{
		CollectionReference users = firestore.Collection("Users");
		users.WhereEqualTo("Name", newUserName)
		     .GetSnapshotAsync()
		     .ContinueWithOnMainThread(task =>
		     {
			     if (task.IsCompletedSuccessfully)
			     {
				     QuerySnapshot snapshot = task.Result;

				     // Checking if the user's name already exists
				     if (!snapshot.Documents.Any())
				     {
					     DocumentReference newUserDoc = users.Document(newUserName);
					     
					     // Data for new user
					     var data = new Dictionary<string, object>
					     {
						     {"Name", newUserName}
					     };
					     
					     // Add the new user data 

					     newUserDoc.SetAsync(data).ContinueWithOnMainThread(setTask =>
						     {
							     if (setTask.IsCompleted)
							     {
								     Debug.Log($"User {newUserName} added successfully!");
							     }
							     else
							     {
								     Debug.LogError($"Error adding user {newUserName}:" + setTask.Exception);
							     }       
						     });
				     }
				     else
				     {
					     Debug.Log($"User {newUserName} already exists, new user has not been added.");
				     }
			     }
			     else
			     {
				     Debug.LogError("Error during check for user already existing: " + task.Exception);
			     }
		     });
	}
	
// Combined method to get both task count and task data, change name at some point xP
    private void GetTasksAndCount(string user, System.Action<int, List<Task>> callback)
    {
        var tasksReference = firestore.Collection("Users").Document(user).Collection("Tasks");

        tasksReference.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                QuerySnapshot snapshot = task.Result;

                // Create a list to store the tasks
                List<Task> taskList = new List<Task>();

                // Iterate through each document in the snapshot and create Task objects
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    Task newTask = new Task
                    {
                        Titel = document.GetValue<string>("Titel"),
                        Emoji = document.GetValue<string>("Emoji"),
                        Description = document.GetValue<string>("Description"),
                        ImageFormat = document.GetValue<bool>("ImageFormat"),
                        Status = document.GetValue<int>("Status"),
                        Repeat = document.GetValue<int>("Repeat")
                    };

                    // Add the task object to the list
                    taskList.Add(newTask);
                }

                // Pass both the task count and the task list to the callback
                callback(snapshot.Documents.Count(), taskList);
            }
            else
            {
                Debug.LogError("Error fetching tasks: " + task.Exception);
                callback(0, new List<Task>()); // Return 0 and an empty list on error
            }
        });
    }

    // Function to spawn tasks after getting the task count and data
    public void spawnTasks(string user)
    {
        tasks.Clear(); // Reset list so we don't spawn missing objects
        GameObject mainScreen = GameObject.FindWithTag("mainScreen");

        // Get both task count and task data in a single call
        GetTasksAndCount(user, (taskAmount, taskList) =>
        {
            Debug.Log("Task amount: " + taskAmount);

            // Store the tasks in TaskData
            TaskData = taskList;

            int buffer = -300;

            // Instantiate a GameObject for each task and add it to the screen
            for (int i = 0; i < TaskData.Count; i++)
            {
                // Instantiate the task GameObject
                GameObject newTask = Instantiate(taskTemplate, mainScreen.transform, false);

                // Set task's position
                newTask.transform.localPosition = new Vector3(0, buffer * i, 0);
                
                newTask.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TaskData[i].Titel;
                newTask.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = TaskData[i].Emoji;
                newTask.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = TaskData[i].Description;
                newTask.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = TaskData[i].ImageFormat ? "P" : "W";
                newTask.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = TaskData[i].Status.ToString();

                // Add the newly created task object to the tasks list
                tasks.Add(newTask);
            }

            Debug.Log("Tasks spawned");
        });
    }

    public void addTaskToAllUsers(Task newTask)
    {
	    firestore.Collection("Users").GetSnapshotAsync().ContinueWithOnMainThread(userTask =>
	    {
		    if (userTask.IsFaulted || userTask.IsCanceled)
		    {
			    Debug.LogError("Error getting users: " + userTask.Exception);
			    return;
		    }

		    QuerySnapshot userSnapshots = userTask.Result;

		    foreach (DocumentSnapshot userDoc in userSnapshots.Documents)
		    {
			    string userId = userDoc.Id;

			    // Convert Task object to dictionary
			    Dictionary<string, object> taskData = new Dictionary<string, object>
			    {
				    { "Titel",newTask.Titel },
				    { "Emoji",newTask.Emoji },
				    { "Description",newTask.Description },
				    { "ImageFormat",newTask.ImageFormat },
				    { "Status", newTask.Status },
				    {"Repeat", newTask.Repeat}
			    };

			    // Add task to each user's Tasks subcollection
			    string taskId = $"Task_{System.DateTime.Now.ToString("dd-MM-yy-HH-mm-ss")}";
			    DocumentReference taskRef = firestore.Collection("Users").Document(userId).Collection("Tasks").Document(taskId);
			    taskRef.SetAsync(taskData).ContinueWithOnMainThread(t =>
			    {
				    if (t.IsCompleted)
				    {
					    Debug.Log("Task added to user: " + userId);
				    }
				    else
				    {
					    Debug.LogError("Failed to add task for user: " + userId + " | " + t.Exception);
				    }
			    });
		    }
	    });
    }

}