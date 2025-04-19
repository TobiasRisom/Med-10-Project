using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirestoreHandler : MonoBehaviour
{
	private FirebaseFirestore firestore;
	public string username = "Leif";
	
	public int currentTask;
	public string currentUserInfo;
	
	public class Task
	{
		public string Titel { get; set; }
		public string Emoji { get; set; }
		public string Description { get; set; }
		public bool ImageFormat { get; set; }
		public int Status { get; set; }
		public int Repeat { get; set; }
		public string Answer { get; set; }
	}
	public List<Task> TaskData = new List<Task>(); // Stores data locally so we don't need to keep reading the database
	public List<GameObject> tasks; // Stores the tasks game objects that are spawned in the Unity scene
	public GameObject taskTemplate; // Template for all tasks shown on the main screen

	// Same variables but for verifying tasks
	public List<Task> V_TaskData = new List<Task>();
	public List<GameObject> V_tasks;
	public GameObject v_taskTemplate;
	public List<String> V_Users = new List<String>();

	public GameObject userToggle;
	
	
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
	
	public async Task<List<string>> GetUsers()
	{
		List<string> userNames = new List<string>();

		CollectionReference colRef = firestore.Collection("Users");
		
		QuerySnapshot snapshot = await colRef.GetSnapshotAsync();

		Debug.Log("List of users accessed");

		foreach (DocumentSnapshot document in snapshot.Documents)
		{
			if (document.ContainsField("Name"))
			{
				string userName = document.GetValue<string>("Name");
				userNames.Add(userName);
			}
		}

		return userNames;
	}
	
    public void GetTasksAndCount(string user, System.Action<int, List<Task>> callback)
    {
        var tasksReference = firestore.Collection("Users").Document(user).Collection("Tasks").OrderBy(FieldPath.DocumentId);

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
                        Repeat = document.GetValue<int>("Repeat"),
                        Answer = document.GetValue<string>("Answer")
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
    
    public void spawnTasks(string user)
    {
	    Debug.Log("Running SpawnTasks");
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
            
            for (int i = 0; i < tasks.Count; i++)
            {
	            Button btn = tasks[i]
		            .GetComponent<Button>();

	            int index = i; // looks stupid but necessary due to scope
	            btn.onClick.AddListener(delegate { goToTask(index); });
            }

            Debug.Log("Tasks spawned");
        });
    }

	public void GetTasksAwaitingVerification(System.Action<int, List<Task>> callback)
{
	V_Users.Clear();
    List<Task> verifiedTasks = new List<Task>();

    // Get the list of users from Firestore
    firestore.Collection("Users").GetSnapshotAsync().ContinueWithOnMainThread(usersTask =>
    {
        if (!usersTask.IsCompletedSuccessfully)
        {
            Debug.LogError("Failed to get users: " + usersTask.Exception);
            callback(0, verifiedTasks);
            return;
        }

        QuerySnapshot usersSnapshot = usersTask.Result;

        if (usersSnapshot.Count == 0)
        {
            callback(0, verifiedTasks);
            return;
        }

        List<System.Threading.Tasks.Task> asyncTasks = new List<System.Threading.Tasks.Task>();
        
        foreach (DocumentSnapshot userDoc in usersSnapshot.Documents)
        {
            string userId = userDoc.Id;
            Debug.Log($"Querying tasks for user: {userId}");

            // Fetch tasks for this user (async operation)
            System.Threading.Tasks.Task t = firestore.Collection("Users").Document(userId).Collection("Tasks")
                .WhereEqualTo("Status", 1)
                .GetSnapshotAsync()
                .ContinueWithOnMainThread(tasksTask =>
                {
                    if (tasksTask.IsCompletedSuccessfully)
                    {
                        QuerySnapshot taskSnapshot = tasksTask.Result;
                        Debug.Log($"Found {taskSnapshot.Count} tasks with Status == 1 for user: {userId}");
                        foreach (DocumentSnapshot document in taskSnapshot.Documents)
                        { 
	                        Task newTask = new Task
                            {
                                Titel = document.GetValue<string>("Titel"),
                                Emoji = document.GetValue<string>("Emoji"),
                                Description = document.GetValue<string>("Description"),
                                ImageFormat = document.GetValue<bool>("ImageFormat"),
                                Status = document.GetValue<int>("Status"),
                                Repeat = document.GetValue<int>("Repeat"),
                                Answer = document.GetValue<string>("Answer")
                            };

                            verifiedTasks.Add(newTask); // Add custom task to the list
                            V_Users.Add(userId);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Error getting tasks for user {userId}: " + tasksTask.Exception);
                    }
                });

            asyncTasks.Add(t); // Add the async task (System.Threading.Tasks.Task) to the list
        }

        // Wait for all async tasks to complete before triggering the callback
        System.Threading.Tasks.Task.WhenAll(asyncTasks).ContinueWithOnMainThread(_ =>
        {
            callback(verifiedTasks.Count, verifiedTasks); // All tasks completed, call the callback
        });
    });
}
	public void spawnVerifiedTasks()
	{
		V_tasks.Clear();
		GameObject content = GameObject.FindWithTag("content");
		ANSATContentHandler ch = content.GetComponent<ANSATContentHandler>();

		GetTasksAwaitingVerification((taskAmount, taskList) =>
		{

			V_TaskData = taskList;
			for (int i = 0; i < V_TaskData.Count; i++)
			{
				GameObject newTask = Instantiate(v_taskTemplate, content.transform);
				
				newTask.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = V_TaskData[i].Titel;
				newTask.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = V_Users[i];

				if (V_TaskData[i].ImageFormat)
				{
					newTask.transform.GetChild(2).gameObject.SetActive(true);
					newTask.transform.GetChild(3).gameObject.SetActive(false);
					newTask.transform.GetChild(4).gameObject.SetActive(false);
					
					// Convert base64 to byte array
					byte[] imageBytes = Convert.FromBase64String(V_TaskData[i].Answer);

					// Create a texture from the bytes
					Texture2D texture = new Texture2D(2, 2); // The size will be replaced by LoadImage
					if (!texture.LoadImage(imageBytes))
					{
						Debug.LogError("Failed to load image from base64 string.");
						continue;
					}
					
					// Create a Sprite from the texture
					Sprite sprite = Sprite.Create(
						texture,
						new Rect(0, 0, texture.width, texture.height),
						new Vector2(0.5f, 0.5f)
					);
					
					// Assign the sprite to the Image component
					Image img = newTask.transform.GetChild(2).GetComponent<Image>();
					img.sprite = sprite;
				}
				else
				{
					newTask.transform.GetChild(2).gameObject.SetActive(false);
					newTask.transform.GetChild(3).gameObject.SetActive(true);
					newTask.transform.GetChild(4).gameObject.SetActive(true);
					
					TextMeshProUGUI answer = newTask.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>();
					answer.text = V_TaskData[i].Answer;
				}
				V_tasks.Add(newTask);
			}
			
			for (int i = 0; i < V_tasks.Count; i++)
			{
				Debug.Log(V_Users[i]);

				Button accept = V_tasks[i].transform.GetChild(5).GetComponent<Button>();
				
				Button reject = V_tasks[i].transform.GetChild(6).GetComponent<Button>();

				int index = i; // looks stupid but necessary due to scope
				string storedUser = V_Users[index];
				string storedTitle = V_TaskData[index].Titel;
				accept.onClick.AddListener(delegate { acceptOrRejectTask(storedUser, storedTitle, 2); });
				reject.onClick.AddListener(delegate { acceptOrRejectTask(storedUser, storedTitle, 0); });
				
				Debug.Log("Spawned task with parameters: " + storedUser + " & " + storedTitle);
				
				ch.AddItem(V_tasks[i]);
			}

			Debug.Log("Verified tasks spawned.");
		});
	}

	public async void acceptOrRejectTask(string user, string taskTitle, int status)
	{
		GameObject content = GameObject.FindWithTag("content");
		ANSATContentHandler ch = content.GetComponent<ANSATContentHandler>();
		CollectionReference colRef = firestore.Collection("Users").Document(user).Collection("Tasks");
		
		Query query = colRef.WhereEqualTo("Titel", taskTitle).Limit(1);
		QuerySnapshot snapshot = await query.GetSnapshotAsync();
		
		var docs = snapshot.Documents.ToList();
		
		DocumentReference newRef = docs[0].Reference;
	    
		Dictionary<string, object> updates = new Dictionary<string, object>
		{
			{ "Status", status } 
		};
	    
		await newRef.UpdateAsync(updates);

		for (int i = 0; i < V_TaskData.Count(); i++)
		{
			if (V_Users[i] == user && V_TaskData[i].Titel == taskTitle)
			{
				V_Users.RemoveAt(i);
				V_tasks.RemoveAt(i);
				V_TaskData.RemoveAt(i);
				ch.RemoveItem(i);
				break;
			}
		}
		Debug.Log("Task updated successfully!");
	}

    
    public void goToTask(int taskIndex)
    {
	    currentTask = taskIndex;
	    SceneManager.LoadScene("TaskScreen");
    }

    public async void submitTask(string user, string answer)
    {
	    DocumentReference docRef = firestore.Collection("Users").Document(user);

	    Query query = docRef.Collection("Tasks")
	                        .OrderBy(FieldPath.DocumentId);

	    QuerySnapshot snapshot = await query.GetSnapshotAsync();

	    var docs = snapshot.Documents.ToList();
		Debug.Log("Current task: "+currentTask);
	    DocumentReference newRef = docs[currentTask].Reference;
	    
	    Dictionary<string, object> updates = new Dictionary<string, object>
	    {
		    { "Answer", answer },
		    { "Status", 1 } 
	    };
	    
	    await newRef.UpdateAsync(updates);
	    Debug.Log("Task updated successfully!");
    }

    public void addTaskToUsers(Task newTask, List<string> users)
    {
	    firestore.Collection("Users").GetSnapshotAsync().ContinueWithOnMainThread(userTask =>
	    {
		    if (!userTask.IsCompletedSuccessfully)
		    {
			    Debug.LogError("Task not completed");
			    return;
		    }

		    QuerySnapshot userSnapshots = userTask.Result;

		    foreach (DocumentSnapshot userDoc in userSnapshots.Documents)
		    {
			    string userId = userDoc.Id;

			    // Only add task if userId is in the provided list
			    if (!users.Contains(userId))
				    continue;

			    // Convert Task object to dictionary
			    Dictionary<string, object> taskData = new Dictionary<string, object>
			    {
				    { "Titel", newTask.Titel },
				    { "Emoji", newTask.Emoji },
				    { "Description", newTask.Description },
				    { "ImageFormat", newTask.ImageFormat },
				    { "Status", newTask.Status },
				    { "Repeat", newTask.Repeat },
				    { "Answer", newTask.Answer }
			    };

			    // Add task to selected user's Tasks subcollection
			    string taskId = $"Task_{DateTime.Now.ToString("dd-MM-yy-HH-mm-ss")}";
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

 public void UpdateDailyAndWeeklyTasks(int today)
{
    V_Users.Clear();
    List<Task> verifiedTasks = new List<Task>();

    firestore.Collection("Users")
             .GetSnapshotAsync()
             .ContinueWithOnMainThread(usersTask =>
             {
                 if (!usersTask.IsCompletedSuccessfully)
                 {
                     Debug.LogError("Failed to get users: " + usersTask.Exception);
                     return;
                 }

                 QuerySnapshot usersSnapshot = usersTask.Result;
                 foreach (DocumentSnapshot userDoc in usersSnapshot.Documents)
                 {
                     string userId = userDoc.Id;
                     Debug.Log($"Querying tasks for user: {userId}");

                     // Fetch all tasks for this user
                     var t = firestore.Collection("Users")
                                      .Document(userId)
                                      .Collection("Tasks")
                                      .GetSnapshotAsync()
                                      .ContinueWithOnMainThread(tasksTask =>
                                      {
                                          if (tasksTask.IsCompletedSuccessfully)
                                          {
                                              QuerySnapshot taskSnapshot = tasksTask.Result;

                                              foreach (DocumentSnapshot taskDoc in taskSnapshot.Documents)
                                              {
                                                  if (taskDoc.TryGetValue("Status", out int status))
                                                  {
                                                      if (status == 2 || status == today)
                                                      {
                                                          // Update status to 0
                                                          Dictionary<string, object> updates = new Dictionary<string, object>
                                                          {
                                                              { "Status", 0 }
                                                          };

                                                          firestore.Collection("Users")
                                                                   .Document(userId)
                                                                   .Collection("Tasks")
                                                                   .Document(taskDoc.Id)
                                                                   .UpdateAsync(updates)
                                                                   .ContinueWithOnMainThread(updateTask =>
                                                                   {
                                                                       if (updateTask.IsCompletedSuccessfully)
                                                                       {
                                                                           Debug.Log($"Task {taskDoc.Id} for user {userId} updated to Status 0.");
                                                                       }
                                                                       else
                                                                       {
                                                                           Debug.LogError($"Failed to update task {taskDoc.Id} for user {userId}: {updateTask.Exception}");
                                                                       }
                                                                   });
                                                      }
                                                  }
                                                  else
                                                  {
                                                      Debug.LogWarning($"Task {taskDoc.Id} for user {userId} has no 'Status' field.");
                                                  }
                                              }
                                          }
                                          else
                                          {
                                              Debug.LogError($"Error getting tasks for user {userId}: " + tasksTask.Exception);
                                          }
                                      });
                 }
             });
}
    
    public async Task<Dictionary<string, string>> GetLeaderboard()
    {
	    Dictionary<string, string> leaderboard = new Dictionary<string, string>();
	    
	    await firestore.Collection("Users")
	                   .OrderByDescending("Points")
	                   .GetSnapshotAsync()
	                   .ContinueWithOnMainThread(task => {
		      
		                   if (!task.IsCompletedSuccessfully)
		                   {
			                   Debug.LogError("Failed to get users");
			                   return;
		                   }

		                   QuerySnapshot snapshot = task.Result;

		                   foreach (DocumentSnapshot document in snapshot.Documents)
		                   {
			                   Dictionary<string, object> userData = document.ToDictionary();
			                   leaderboard[document.Id] = userData["Points"].ToString();
		                   }
	                   });
	    return leaderboard;
    }

    public void SetUserToggles()
    {
	    GameObject layout = GameObject.FindWithTag("UserToggles");
	    firestore.Collection("Users").GetSnapshotAsync().ContinueWithOnMainThread(task =>
	    {
		    if (task.IsFaulted || task.IsCanceled)
		    {
			    Debug.LogError("Failed to retrieve users from Firestore.");
			    return;
		    }

		    QuerySnapshot snapshot = task.Result;
		    foreach (DocumentSnapshot document in snapshot.Documents)
		    {
			    string userId = document.Id;
			    GameObject toggleObj = Instantiate(userToggle, layout.transform);
			    TextMeshProUGUI label = toggleObj.GetComponentInChildren<TextMeshProUGUI>();

			    if (label != null)
			    {
				    label.text = userId;
			    }
			    else
			    {
				    Debug.LogWarning("Toggle prefab missing Text component in children.");
			    }
		    }
	    });
    }
}