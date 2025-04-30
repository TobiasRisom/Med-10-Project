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
	public GameObject moneyGraphic;

	public bool onUserScreen = false;
	
	
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
                           
                           	Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                           	Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
				           
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
	
	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
		UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
	}

	public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
		UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
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
						     {"Name", newUserName},
						     {"Points", 0},
						     {"TasksNotDone", 0},
						     {"DaysWithAllTasksCleared", 0},
						     {"ScheduleAccessedAmount", 0},
						     {"PetsBought", 0},
						     {"MoneySpentOnPets", 0}
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

	public async void GetUserStats(string user)
	{
		DocumentReference userRef = firestore.Collection("Users")
		                                     .Document(user);
		DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

		if (snapshot.Exists)
		{
			int points = snapshot.GetValue<int>("Points");
			int tasksNotDone = snapshot.GetValue<int>("TasksNotDone");
			int daysCleared = snapshot.GetValue<int>("DaysWithAllTasksCleared");

			GameObject stats = GameObject.FindWithTag("Stats");

			stats.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Opgaver klaret i alt: " + (points / 100);
			stats.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Opgaver ikke klaret: " + tasksNotDone;
			stats.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Dage hvor alle opgaver blev gjort: " + daysCleared;
		}
	}

	public void GetTasksAndCount(string user, System.Action<int, List<Task>, List<DocumentSnapshot>> callback)
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
                callback(snapshot.Documents.Count(), taskList, snapshot.Documents.ToList());
            }
            else
            {
                Debug.LogError("Error fetching tasks: " + task.Exception);
                callback(0, new List<Task>(), new List<DocumentSnapshot>()); // Return 0 and an empty list on error
            }
        });
    }
    
    public void spawnTasks(string user)
    {
	    Debug.Log("Running SpawnTasks");
        tasks.Clear(); // Reset list so we don't spawn missing objects
        GameObject mainScreen = GameObject.FindWithTag("mainScreen");
        
        GameObject content = GameObject.FindWithTag("content");
        ContentHandler ch = content.GetComponent<ContentHandler>();

        GetTasksAndCount(user, (taskAmount, taskList, documentSnapshots) =>
        {
	        TaskData = taskList;

	        // Instantiate a GameObject for each task and add it to the screen
            for (int i = 0; i < TaskData.Count; i++)
            {
	            if (TaskData[i].Status == 2)
	            {
		            TaskData[i].Description = "Du har klaret opgaven!\nTryk for at tjene ØG Dollars!";
	            }
	            
	            if (TaskData[i].Status == 3)
		            continue; 
	            
                // Instantiate the task GameObject
                GameObject newTask = Instantiate(taskTemplate, content.transform, false);
                
                Button taskButton = newTask.GetComponent<Button>();

                
                newTask.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TaskData[i].Titel;
                newTask.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = TaskData[i].Emoji;
                newTask.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = TaskData[i].Description;
                newTask.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = TaskData[i].ImageFormat ? "\ud83d\udcf8" : "\ud83d\udcdd";

                switch (TaskData[i].Status)
                {
	                case 0:
		                newTask.transform.GetChild(4).gameObject.SetActive(true);
		                newTask.transform.GetChild(5).gameObject.SetActive(false);
		                newTask.transform.GetChild(6).gameObject.SetActive(false);
		                break;
	                case 1:
		                newTask.transform.GetChild(5).gameObject.SetActive(true);
		                newTask.transform.GetChild(4).gameObject.SetActive(false);
		                newTask.transform.GetChild(6).gameObject.SetActive(false);
		                
		                taskButton.GetComponent<Image>()
		                          .color = new Color(0.70f, 0.94f, 1f);
		                break;
	                case 2:
		                newTask.transform.GetChild(6).gameObject.SetActive(true);
		                newTask.transform.GetChild(4).gameObject.SetActive(false);
		                newTask.transform.GetChild(5).gameObject.SetActive(false);

		                taskButton.GetComponent<Image>()
		                          .color = new Color(0.47f, 0.78f, 0.49f);

		                newTask.transform.GetChild(3)
		                       .GetComponent<TextMeshProUGUI>()
		                       .text = "\ud83d\udcb8";
		                
		                // 🎯 Attach claim button logic
		                int taskIndex = i;
		                taskButton.onClick.AddListener(() => {
			                ClaimTaskReward(user, taskIndex, documentSnapshots[taskIndex], taskButton.gameObject);
		                });
		                break;
                }

                // Add the newly created task object to the tasks list
                tasks.Add(newTask);
                
                if (TaskData[i].Status != 2)
                {
	                Button btn = tasks[i]
		                .GetComponent<Button>();

	                int index = i; // looks stupid but necessary due to scope
	                btn.onClick.AddListener(delegate { goToTask(index); });
                }
                ch.AddItem(tasks[i]);
            }

            Debug.Log("Tasks spawned");
        });
    }
    
    private void ClaimTaskReward(string user, int taskIndex, DocumentSnapshot documentSnapshot, GameObject button)
    {
	    
	    GameObject instance = Instantiate(moneyGraphic);
	    instance.GetComponent<MoneyAnimation>().Play(button.transform.position, GameObject.FindWithTag("MainCanvas").transform);
	    
	    // 🎁 Give the dollars
	    int currentDollars = PlayerPrefs.GetInt("Dollars", 0);
	    PlayerPrefs.SetInt("Dollars", currentDollars + 100);
	    PlayerPrefs.Save();

	    // 🧠 Check Repeat field
	    if (TaskData[taskIndex].Repeat == 0)
	    {
		    // 🗑️ Delete task if no repeats
		    firestore.Collection("Users")
		             .Document(user)
		             .Collection("Tasks")
		             .Document(documentSnapshot.Id)
		             .DeleteAsync()
		             .ContinueWithOnMainThread(task =>
		             {
			             if (task.IsCompletedSuccessfully)
				             Debug.Log("Task deleted successfully.");
			             else
				             Debug.LogError("Error deleting task: " + task.Exception);
		             });
	    }
	    else
	    {
		    // ♻️ Otherwise, set status to 3 (hidden)
		    firestore.Collection("Users")
		             .Document(user)
		             .Collection("Tasks")
		             .Document(documentSnapshot.Id)
		             .UpdateAsync(new Dictionary<string, object> {
			             { "Status", 3 }
		             });
	    }
	    
	    tasks.RemoveAt(taskIndex);
	    TaskData.RemoveAt(taskIndex);
	    GameObject.FindWithTag("content").GetComponent<ContentHandler>().RemoveItem(taskIndex);

	    // Optionally refresh UI (destroy or reload)
	    Debug.Log("Reward claimed for task index: " + taskIndex);
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
		ContentHandler ch = content.GetComponent<ContentHandler>();

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
					newTask.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = "\ud83d\udcf8";
					
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
					newTask.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = "\ud83d\udcdd";
					
					TextMeshProUGUI answer = newTask.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>();
					answer.text = V_TaskData[i].Answer;
				}
				V_tasks.Add(newTask);
			}
			
			for (int i = 0; i < V_tasks.Count; i++)
			{
				Debug.Log(V_Users[i]);

				Button accept = V_tasks[i].transform.GetChild(6).GetComponent<Button>();
				
				Button reject = V_tasks[i].transform.GetChild(7).GetComponent<Button>();

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
		ContentHandler ch = content.GetComponent<ContentHandler>();
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
		
		if (status == 2)
		{
			DocumentReference userRef = firestore.Collection("Users").Document(user);
			DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();

			if (userSnapshot.Exists && userSnapshot.TryGetValue("Points", out int currentPoints))
			{
				int newPoints = currentPoints + 100;
				await userRef.UpdateAsync(new Dictionary<string, object>
				{
					{ "Points", newPoints }
				});
			}
		}

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

    public async System.Threading.Tasks.Task submitTask(string user, string answer)
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

	 int yesterday;
	 if (today == 2)
	 {
		 yesterday = 8;
	 }
	 else
	 {
		 yesterday = today - 1;
	 }

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

                                              bool allTasksCompleted = true;
                                              
                                              foreach (DocumentSnapshot taskDoc in taskSnapshot.Documents)
                                              {
	                                              if (taskDoc.TryGetValue("Status", out int status))
	                                              {
		                                              if (status == 0)
		                                              {
			                                              allTasksCompleted = false;
			                                              Dictionary<string, object> updates = new Dictionary<string, object>
			                                              {
				                                              { "TasksNotDone", FieldValue.Increment(1) }
			                                              };
                                                          
			                                              firestore.Collection("Users")
			                                                       .Document(userId).UpdateAsync(updates);
		                                              }
	                                              }
	                                              
                                                  if (taskDoc.TryGetValue("Repeat", out int repetition))
                                                  {
                                                      if (repetition == 1 || repetition == today)
                                                      {
                                                          // Make daily and weekly tasks accessible
                                                          Dictionary<string, object> updates = new Dictionary<string, object>
                                                          {
                                                              { "Status", 0 }
                                                          };
                                                          
                                                          firestore.Collection("Users")
                                                                   .Document(userId)
                                                                   .Collection("Tasks")
                                                                   .Document(taskDoc.Id)
                                                                   .UpdateAsync(updates);
                                                      }

                                                      if (repetition == yesterday)
                                                      {
	                                                      // Make weekly tasks not done inaccessible
	                                                      Dictionary<string, object> updates = new Dictionary<string, object>
	                                                      {
		                                                      { "Status", 3 },
	                                                      };
	                                                      
	                                                      firestore.Collection("Users")
	                                                               .Document(userId)
	                                                               .Collection("Tasks")
	                                                               .Document(taskDoc.Id)
	                                                               .UpdateAsync(updates);
                                                      }
                                                  }
                                                  else
                                                  {
                                                      Debug.LogWarning($"Task {taskDoc.Id} for user {userId} has no 'Status' field.");
                                                  }
                                              }

                                              if (allTasksCompleted)
                                              {
	                                              Dictionary<string, object> updates = new Dictionary<string, object>
	                                              {
		                                              { "DaysWithAllTasksCleared", FieldValue.Increment(1) }
	                                              };
                                                          
	                                              firestore.Collection("Users")
	                                                       .Document(userId).UpdateAsync(updates);
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
    
    public void UpdateStats(string userName, string statToUpdate, int amountToIncrease)
    {
	    DocumentReference docRef = firestore.Collection("Users")
	                      .Document(userName);
	    docRef.GetSnapshotAsync().ContinueWithOnMainThread(task => {
		      
		                   if (!task.IsCompletedSuccessfully)
		                   {
			                   Debug.LogError("Failed to get users");
			                   return;
		                   }

		                   DocumentSnapshot snapshot = task.Result;

		                   if (snapshot.Exists)
		                   {
			                   Dictionary<string, object> updates = new Dictionary<string, object>
			                   {
				                   {statToUpdate, FieldValue.Increment(amountToIncrease)} // Example: add a timestamp for last login
			                   };
			                   
			                   docRef.UpdateAsync(updates);
		                   }
	    });
    }

    public void SetUserToggles()
    {
	    GameObject content = GameObject.FindWithTag("content");
	    ContentHandler ch = content.GetComponent<ContentHandler>();
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
			    GameObject toggleObj = Instantiate(userToggle, content.transform, false);
			    TextMeshProUGUI label = toggleObj.GetComponentInChildren<TextMeshProUGUI>();

			    if (label != null)
			    {
				    label.text = userId;
			    }
			    else
			    {
				    Debug.LogWarning("Toggle prefab missing Text component in children.");
			    }
			    ch.AddItem(toggleObj);
		    }
	    });
    }
}