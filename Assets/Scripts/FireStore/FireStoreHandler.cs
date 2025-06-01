using System;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Messaging;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

// The main Firestore Class. Handles all communication with Google Firestore.

public class FirestoreHandler : MonoBehaviour
{
	private FirebaseFirestore firestore;
	
	[FormerlySerializedAs("currentTask")]
	public DocumentSnapshot currentSnapshot;
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
		public string TimeStamp { get; set; }
	}

	public Task currentTask;
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
	
	private bool initialized = false;
	
	
	private void Awake()
	{
		DontDestroyOnLoad(this);
	}
	
	public async System.Threading.Tasks.Task InitializeFirestore()
	{
		// Assumes dependency check done outside already
		FirebaseApp app = FirebaseApp.DefaultInstance;
		firestore = FirebaseFirestore.GetInstance(app);

		Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
		Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

		initialized = true;
		Debug.Log("FireStore Initialized!");
		await System.Threading.Tasks.Task.CompletedTask;  // Just to match async signature
	}
	
	public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
		UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
	}

	public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
		UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
	}
	
	// Gets the data for the weekly dinner schedule.
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
				// The snapshot has our intended data!
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

	// Puts the weekly schedule data into the weekly schedule
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

	// Updates the schedule when it's been edited by ØG staff
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

	// When a new user is created, inserts them into the database
	public Task<bool> AddNewUser(string userId, string newUserName)
	{
		CollectionReference users = firestore.Collection("Users");
		var tcs = new TaskCompletionSource<bool>();

		DocumentReference userDocRef = users.Document(userId);

		userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
		{
			if (task.IsCompletedSuccessfully)
			{
				var snapshot = task.Result;

				if (!snapshot.Exists)
				{
					var data = new Dictionary<string, object>
					{
						{"UserId", userId},
						{"Name", newUserName},
						{"Points", 0},
						{"TasksNotDone", 0},
						{"DaysWithAllTasksCleared", 0},
						{"ScheduleAccessedAmount", 0},
						{"PetsBought", 0},
						{"MoneySpentOnPets", 0},
						{"DaysActive", 1},
						{"TimeOfCreation", Timestamp.GetCurrentTimestamp()}
					};

					userDocRef.SetAsync(data).ContinueWithOnMainThread(setTask =>
					{
						if (setTask.IsCompleted)
						{
							Debug.Log($"User {newUserName} added successfully!");
							tcs.SetResult(true);
						}
						else
						{
							Debug.LogError($"Error adding user {newUserName}: " + setTask.Exception);
							tcs.SetResult(false);
						}
					});
				}
				else
				{
					Debug.Log($"User {newUserName} already exists.");
					tcs.SetResult(false);
				}
			}
			else
			{
				Debug.LogError("Error checking user existence: " + task.Exception);
				tcs.SetResult(false);
			}
		});

		return tcs.Task;
	}


	
	public Task<bool> AddStaffMember(string staffUserId, string staffName)
	{
		CollectionReference staffCollection = firestore.Collection("Staff");
		var tcs = new TaskCompletionSource<bool>();

		// Check if staff user already exists by userId (assuming userId is the doc ID)
		DocumentReference staffDocRef = staffCollection.Document(staffUserId);

		staffDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
		{
			if (task.IsCompletedSuccessfully)
			{
				var snapshot = task.Result;

				if (!snapshot.Exists)
				{
					// Staff data - only necessary fields
					var data = new Dictionary<string, object>
					{
						{"Name", staffName},
						{"UserId", staffUserId},
						{"TimeOfCreation", Timestamp.GetCurrentTimestamp()}
					};

					staffDocRef.SetAsync(data).ContinueWithOnMainThread(setTask =>
					{
						if (setTask.IsCompleted)
						{
							Debug.Log($"Staff member {staffName} added successfully!");
							tcs.SetResult(true);
						}
						else
						{
							Debug.LogError($"Error adding staff member {staffName}: " + setTask.Exception);
							tcs.SetResult(false);
						}
					});
				}
				else
				{
					Debug.Log($"Staff member {staffName} already exists.");
					tcs.SetResult(false);
				}
			}
			else
			{
				Debug.LogError("Error checking staff existence: " + task.Exception);
				tcs.SetResult(false);
			}
		});

		return tcs.Task;
	}
	
	
	
	// Gets the list of current users
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

	// Get the statistics for the user shown in the User Overview
	public async void GetUserStats(string user)
	{
		GameObject stats = GameObject.FindWithTag("Stats");

		if (user == "Alle Beboere")
		{
			QuerySnapshot allUsersSnapshot = await firestore.Collection("Users").GetSnapshotAsync();

			int totalPoints = 0;
			int totalTasksNotDone = 0;
			int totalDaysCleared = 0;

			foreach (DocumentSnapshot doc in allUsersSnapshot.Documents)
			{
				if (doc.Exists)
				{
					totalPoints += doc.GetValue<int>("Points");
					totalTasksNotDone += doc.GetValue<int>("TasksNotDone");
					totalDaysCleared += doc.GetValue<int>("DaysWithAllTasksCleared");
				}
			}

			stats.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Opgaver klaret i alt: " + (totalPoints / 100);
			stats.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Ignorerede Opgaver: " + totalTasksNotDone;
			stats.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Dage hvor alle opgaver blev gjort: " + totalDaysCleared;
		}
		else
		{
			DocumentReference userRef = firestore.Collection("Users").Document(user);
			DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

			if (snapshot.Exists)
			{
				int points = snapshot.GetValue<int>("Points");
				int tasksNotDone = snapshot.GetValue<int>("TasksNotDone");
				int daysCleared = snapshot.GetValue<int>("DaysWithAllTasksCleared");

				stats.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Opgaver klaret i alt: " + (points / 100);
				stats.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Ignorerede Opgaver: " + tasksNotDone;
				stats.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Dage hvor alle opgaver blev gjort: " + daysCleared;
			}
		}
	}

	// Class that pairs tasks with their associated snapshot, used to ensure that tasks in the app point to the same task in the database
	public class TaskWithSnapshot
	{
		public Task TaskData { get; set; }
		public DocumentSnapshot Snapshot { get; set; }
	}

	public void GetTasksAndCount(string user, bool includeStatus3Tasks, System.Action<int, List<TaskWithSnapshot>> callback)
	{
		var tasksReference = firestore.Collection("Users")
		                              .Document(user)
		                              .Collection("Tasks")
		                              .OrderBy(FieldPath.DocumentId);

		tasksReference.GetSnapshotAsync().ContinueWithOnMainThread(task =>
		{
			if (task.IsCompletedSuccessfully)
			{
				QuerySnapshot snapshot = task.Result;

				List<TaskWithSnapshot> visibleTasks = new List<TaskWithSnapshot>();

				foreach (DocumentSnapshot document in snapshot.Documents)
				{
					int status = document.GetValue<int>("Status");

					Task newTask = new Task
					{
						Titel = document.GetValue<string>("Titel"),
						Emoji = document.GetValue<string>("Emoji"),
						Description = document.GetValue<string>("Description"),
						ImageFormat = document.GetValue<bool>("ImageFormat"),
						Status = status,
						Repeat = document.GetValue<int>("Repeat"),
						Answer = document.GetValue<string>("Answer"),
						TimeStamp = document.GetValue<string>("TimeStamp")
					};

					if (!includeStatus3Tasks)
					{
						if (status != 3)
						{
							visibleTasks.Add(new TaskWithSnapshot { TaskData = newTask, Snapshot = document });
						}
					}
					else
					{
						visibleTasks.Add(new TaskWithSnapshot { TaskData = newTask, Snapshot = document });
					}
				}

				callback(visibleTasks.Count, visibleTasks);
			}
			else
			{
				Debug.LogError("Error fetching tasks: " + task.Exception);
				callback(0, new List<TaskWithSnapshot>());
			}
		});
	}
	
	// Class which pairs tasks with their associated user
	public class TaskWithUser
	{
		public string UserId { get; set; }
		public TaskWithSnapshot TaskData { get; set; }
	}
	
	// Returns the list of all tasks among all users, and the total amount of tasks
	public void GetAllUsersTasksAndCount(bool includeStatus3Tasks, System.Action<int, List<TaskWithUser>> callback)
	{
		firestore.Collection("Users").GetSnapshotAsync().ContinueWithOnMainThread(userTask =>
		{
			if (!userTask.IsCompletedSuccessfully)
			{
				Debug.LogError("Error fetching user list: " + userTask.Exception);
				callback(0, new List<TaskWithUser>());
				return;
			}

			var userDocs = userTask.Result.Documents;
			int totalUsers = userDocs.Count();
			if (totalUsers == 0)
			{
				callback(0, new List<TaskWithUser>());
				return;
			}

			int usersProcessed = 0;
			List<TaskWithUser> allTasks = new List<TaskWithUser>();

			foreach (var userDoc in userDocs)
			{
				string userId = userDoc.Id;

				GetTasksAndCount(userId, includeStatus3Tasks, (count, tasks) =>
				{
					foreach (var t in tasks)
					{
						allTasks.Add(new TaskWithUser
						{
							UserId = userId,
							TaskData = t
						});
					}

					usersProcessed++;

					if (usersProcessed == totalUsers)
					{
						callback(allTasks.Count, allTasks);
					}
				});
			}
		});
	}



private List<TaskWithSnapshot> taskWithSnapshots = new List<TaskWithSnapshot>();

// Spawn current tasks for the resident
public void spawnTasks(string user)
{
    Debug.Log("Running SpawnTasks");
    tasks.Clear(); // Reset list so we don't spawn missing objects
    GameObject mainScreen = GameObject.FindWithTag("mainScreen");
    GameObject content = GameObject.FindWithTag("content");
    ContentHandler ch = content.GetComponent<ContentHandler>();

    GameObject loadingText = GameObject.FindWithTag("loadingText");
    loadingText.SetActive(false);

    GetTasksAndCount(user, false, (taskAmount, taskWithSnapshots) =>
    {
        TaskData = taskWithSnapshots.Select(entry => entry.TaskData).ToList(); // Update TaskData list to only include TaskData

        // Iterate through the task entries (TaskWithSnapshot)
        for (int i = 0; i < TaskData.Count; i++)
        {
            var taskEntry = taskWithSnapshots[i]; // Capture TaskWithSnapshot entry
            Task thisTask = taskEntry.TaskData;

            if (thisTask.Status == 2)
            {
	            thisTask.Description = "Du har klaret opgaven!\nTryk for at tjene ØG Dollars!";
            }
            
            // We do not want to show tasks that are not active
            if (thisTask.Status == 3)
                continue;
            
            GameObject newTask = Instantiate(taskTemplate, content.transform, false);
            Task taskCopy = thisTask;
            DocumentSnapshot snapshotCopy = taskEntry.Snapshot;
            Button taskButton = newTask.GetComponent<Button>();
            
            newTask.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = thisTask.Titel;
            newTask.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = thisTask.Emoji;
            newTask.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = thisTask.Description;
            newTask.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = thisTask.ImageFormat ? "\ud83d\udcf8" : "\ud83d\udcdd";

            switch (thisTask.Status)
            {
                case 0:
                    newTask.transform.GetChild(4).gameObject.SetActive(true);
                    newTask.transform.GetChild(5).gameObject.SetActive(false);
                    newTask.transform.GetChild(6).gameObject.SetActive(false);
                    taskButton.onClick.AddListener(() => goToTask(taskCopy, snapshotCopy));
                    break;
                case 1:
                    newTask.transform.GetChild(5).gameObject.SetActive(true);
                    newTask.transform.GetChild(4).gameObject.SetActive(false);
                    newTask.transform.GetChild(6).gameObject.SetActive(false);
                    taskButton.GetComponent<Image>().color = new Color(0.70f, 0.94f, 1f);
                    taskButton.onClick.AddListener(() => goToTask(taskCopy, snapshotCopy));
                    break;
                case 2:
                    newTask.transform.GetChild(6).gameObject.SetActive(true);
                    newTask.transform.GetChild(4).gameObject.SetActive(false);
                    newTask.transform.GetChild(5).gameObject.SetActive(false);
                    taskButton.GetComponent<Image>().color = new Color(0.47f, 0.78f, 0.49f);
                    newTask.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "\ud83d\udcb8"; // 💸 Emoji
                    taskButton.onClick.AddListener(() => {
                        ClaimTaskReward(user, taskEntry, newTask, ch); 
                    });
                    break;
            }
            
            tasks.Add(newTask);
            ch.AddItem(newTask);
        }

        Debug.Log("Tasks spawned");
    });
}

// Navigating to the task screen, while keeping track of which task the user has tapped
public void goToTask(Task task, DocumentSnapshot snapshot)
{
	currentTask = task;
	currentSnapshot = snapshot; // store the snapshot too
	SceneManager.LoadScene("TaskScreen");
}



// Get a reward from a completed task and set it as inactive
private void ClaimTaskReward(string user, TaskWithSnapshot taskEntry, GameObject button, ContentHandler ch)
{
	GameObject instance = Instantiate(moneyGraphic);
	instance.GetComponent<MoneyAnimation>().Play(button.transform.position, GameObject.FindWithTag("MainCanvas").transform);
	
	int currentDollars = PlayerPrefs.GetInt("Dollars", 0);
	PlayerPrefs.SetInt("Dollars", currentDollars + 100);
	PlayerPrefs.Save();
	GameObject.FindWithTag("PanelHolder").GetComponent<MainScreenNavigation>().setDollarsText();
	
	if (taskEntry.TaskData.Repeat == 0)
	{
		firestore.Collection("Users")
		         .Document(user)
		         .Collection("Tasks")
		         .Document(taskEntry.Snapshot.Id)
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
		firestore.Collection("Users")
		         .Document(user)
		         .Collection("Tasks")
		         .Document(taskEntry.Snapshot.Id)
		         .UpdateAsync(new Dictionary<string, object> {
			         { "Status", 3 }
		         });
	}
	
	ch.RemoveItem(button); 
	Destroy(button);
	taskWithSnapshots.Remove(taskEntry);
}

// Get the data for tasks awaiting verification for staff
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
                                Answer = document.GetValue<string>("Answer"),
                                TimeStamp = document.GetValue<string>("TimeStamp")
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
	// Spawn the tasks awaiting verification in the staff app
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
				
				newTask.transform.GetChild(9).GetComponent<TextMeshProUGUI>().text = V_TaskData[i].Description;
				newTask.transform.GetChild(10).GetComponent<TextMeshProUGUI>().text = V_TaskData[i].Emoji;
				newTask.transform.GetChild(11)
				       .GetComponent<TextMeshProUGUI>()
				       .text = V_TaskData[i].TimeStamp;

				if (V_TaskData[i].ImageFormat)
				{
					newTask.transform.GetChild(2).gameObject.SetActive(true);
					newTask.transform.GetChild(3).gameObject.SetActive(false);
					newTask.transform.GetChild(4).gameObject.SetActive(false);
					newTask.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text = "\ud83d\udcf8";
					
					byte[] imageBytes = Convert.FromBase64String(V_TaskData[i].Answer);

					Texture2D texture = new Texture2D(2, 2);
					if (!texture.LoadImage(imageBytes))
					{
						Debug.LogError("Failed to load image from base64 string.");
						continue;
					}
					
					Sprite sprite = Sprite.Create(
						texture,
						new Rect(0, 0, texture.width, texture.height),
						new Vector2(0.5f, 0.5f)
					);
					
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

	// Handles staff either accepting or rejecting a completed task
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

	// Handles the submission of task answers
	public async System.Threading.Tasks.Task submitTask(string user, string answer)
	{
		DocumentReference newRef = currentSnapshot.Reference;
		
		string timeStamp = DateTime.Now.ToString("HH:mm\ndd/MM");

		Dictionary<string, object> updates = new Dictionary<string, object>
		{
			{ "Answer", answer },
			{ "Status", 1 },
			{ "TimeStamp", timeStamp }
		};

		await newRef.UpdateAsync(updates);
		Debug.Log("Task updated successfully!");
	}
	
	// Add a new task to users based on the parameters set by the staff creating it
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
				    { "Answer", newTask.Answer },
				    { "TimeStamp", newTask.TimeStamp}
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
    
    // Updates Google Firebase statistics and the status of repeating tasks based on the current day
   public async System.Threading.Tasks.Task UpdateDailyAndWeeklyTasks(int today)
{
    int yesterday = (today == 2) ? 8 : today - 1;

    QuerySnapshot usersSnapshot = await firestore.Collection("Users").GetSnapshotAsync();

    foreach (DocumentSnapshot userDoc in usersSnapshot.Documents)
    {
        string userId = userDoc.Id;
        Debug.Log($"Processing user: {userId}");

        Dictionary<string, object> initialUserUpdates = new Dictionary<string, object>
        {
            { "DaysActive", PlayerPrefs.GetInt("DaysActive") }
        };

        await firestore.Collection("Users").Document(userId).UpdateAsync(initialUserUpdates);

        QuerySnapshot taskSnapshot = await firestore.Collection("Users")
                                                    .Document(userId)
                                                    .Collection("Tasks")
                                                    .GetSnapshotAsync();

        bool allTasksCompleted = true;
        List<System.Threading.Tasks.Task> taskUpdates = new List<System.Threading.Tasks.Task>();
        int tasksNotDone = 0;

        foreach (DocumentSnapshot taskDoc in taskSnapshot.Documents)
        {
            Dictionary<string, object> updates = new Dictionary<string, object>();
            string taskId = taskDoc.Id;

            int status = taskDoc.ContainsField("Status") ? taskDoc.GetValue<int>("Status") : -1;
            int repeat = taskDoc.ContainsField("Repeat") ? taskDoc.GetValue<int>("Repeat") : -1;

            if (status == 0)
            {
                allTasksCompleted = false;
                tasksNotDone++;
            }

            if (repeat == 1 || repeat == today)
            {
                updates["Status"] = 0;
            }

            if (repeat == yesterday)
            {
                updates["Status"] = 3;
            }

            if (updates.Count > 0)
            {
                taskUpdates.Add(
                    firestore.Collection("Users")
                             .Document(userId)
                             .Collection("Tasks")
                             .Document(taskId)
                             .UpdateAsync(updates)
                );
            }
        }

        Dictionary<string, object> postTaskUserUpdates = new Dictionary<string, object>
        {
            { "TasksNotDone", FieldValue.Increment(tasksNotDone) }
        };

        if (allTasksCompleted)
        {
            postTaskUserUpdates["DaysWithAllTasksCleared"] = FieldValue.Increment(1);
        }

        taskUpdates.Add(
            firestore.Collection("Users")
                     .Document(userId)
                     .UpdateAsync(postTaskUserUpdates)
        );

        await System.Threading.Tasks.Task.WhenAll(taskUpdates);

        Debug.Log($"Successfully updated tasks for user: {userId}");
    }
}

   // Get stats for the leaderboard
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
    
    // Function for easily increasing stats in Firestore such as "TasksNotDone"
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
				                   {statToUpdate, FieldValue.Increment(amountToIncrease)}
			                   };
			                   
			                   docRef.UpdateAsync(updates);
		                   }
	    });
    }

    // Handles the toggles that allows staff to select which users get a new task in the task creation screen
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
    
    // Deletes a task for a user
    public async System.Threading.Tasks.Task DeleteTaskForUser(string title, string userName)
    {
	    CollectionReference tasksRef = firestore.Collection("Users").Document(userName).Collection("Tasks");
	    QuerySnapshot snapshot = await tasksRef.WhereEqualTo("Titel", title).GetSnapshotAsync();

	    foreach (DocumentSnapshot doc in snapshot.Documents)
	    {
		    await doc.Reference.DeleteAsync();
		    Debug.Log($"Deleted task '{title}' for user '{userName}'.");
	    }
    }

    // Deletes a task for all users that currently have it
    public async System.Threading.Tasks.Task DeleteTaskForAllUsers(string title)
    {
	    QuerySnapshot userSnapshot = await firestore.Collection("Users").GetSnapshotAsync();

	    foreach (DocumentSnapshot userDoc in userSnapshot.Documents)
	    {
		    string currentUser = userDoc.Id;
		    await DeleteTaskForUser(title, currentUser);
	    }
    }
    
    public async Task<string> UserAuthentication()
    {
	    try
	    {
		    var authResult = await FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
		    var newUser = authResult.User;
		    Debug.Log("Signed in anonymously with UID: " + newUser.UserId);
		    PlayerPrefs.SetString("UserID", newUser.UserId);
		    return newUser.UserId;
	    }
	    catch (Exception ex)
	    {
		    Debug.LogError("Failed to sign in: " + ex);
		    return null;
	    }
    }
    
    public void RegisterDeviceTokenForUser(string userId)
    {
	    FirebaseMessaging.TokenReceived += (sender, token) =>
	    {
		    Debug.Log("FCM Token received: " + token.Token);

		    // Check if user is staff by checking existence in "Staff" collection
		    var staffDoc = firestore.Collection("Staff").Document(userId);
		    staffDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
		    {
			    bool isStaff = false;
			    if (task.IsCompletedSuccessfully && task.Result.Exists)
			    {
				    isStaff = true;
			    }

			    SaveDeviceToken(userId, token.Token, isStaff);
		    });
	    };

	    // Request permission (iOS) and get token (Android/iOS)
	    FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
	    {
	    });
    }

    void SaveDeviceToken(string userId, string token, bool isStaff)
    {
	    var devices = firestore.Collection("devices");
	    var deviceDoc = devices.Document(userId);

	    var deviceData = new Dictionary<string, object>
	    {
		    { "fcmToken", token },
		    { "role", isStaff ? "staff" : "resident" },
		    { "lastUpdated", Timestamp.GetCurrentTimestamp() }
	    };

	    deviceDoc.SetAsync(deviceData).ContinueWithOnMainThread(task =>
	    {
		    if (task.IsCompleted)
			    Debug.Log("Device token saved successfully for user " + userId);
		    else
			    Debug.LogError("Failed to save device token: " + task.Exception);
	    });
    }
    
    void SaveFcmTokenToFirestore(string staffUserId, string fcmToken) {
	    DocumentReference docRef = firestore.Collection("Staff").Document(staffUserId);

	    Dictionary<string, object> updates = new Dictionary<string, object> {
		    { "fcmToken", fcmToken }
	    };

	    docRef.SetAsync(updates, SetOptions.MergeAll).ContinueWithOnMainThread(task => {
		    if (task.IsCompleted) {
			    Debug.Log("FCM token saved to Firestore");
		    } else {
			    Debug.LogError("Error saving FCM token: " + task.Exception);
		    }
	    });
    }

    public void staffTokenSave(string staffUserId)
    {
	    Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWith(task => {
		    if (task.IsCompleted && !task.IsFaulted) {
			    string fcmToken = task.Result;
			    Debug.Log("FCM Token: " + fcmToken);

			    // Now save this to Firestore in the 'Staff' collection
			    SaveFcmTokenToFirestore(staffUserId, fcmToken);
		    }
	    });
    }
}