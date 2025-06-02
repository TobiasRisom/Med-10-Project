using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuNavigation : MonoBehaviour
{
    private readonly float pos = -1080;
    private readonly int code = 9827;

    public TMP_InputField codeInput;
    public TMP_InputField beboerInput;
    public TMP_InputField ansatInput;

    public GameObject startNoUser;
    public GameObject startUserExists;
    public TextMeshProUGUI welcomeWithName;

    public TextMeshProUGUI nameDisplay1;
    public TextMeshProUGUI nameDisplay2;
    
    private FirestoreHandler fish;
    
    private bool isUserReady = false;
    
    private float tweenSpeed = 0.5f;
    private Ease tweenEase = Ease.OutQuad;

    private string userName;
    private string userID;
    
    public List<Toggle> toggles;
    public List<GameObject> hearts;

    public TMP_InputField petName;

    async void Start()
    {
	    var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();

	    if (dependencyStatus == Firebase.DependencyStatus.Available)
	    {
		    fish = GameObject.FindWithTag("dataManager").GetComponent<FirestoreHandler>();

		    await fish.InitializeFirestore();  // Make sure FirestoreHandler is ready before calling any Firebase functions

		    userName = PlayerPrefs.GetString("Name", "NoN");

		    if (userName == "NoN")
		    {
			    startNoUser.SetActive(true);
			    startUserExists.SetActive(false);

			    userID = await fish.UserAuthentication();

			    if (string.IsNullOrEmpty(userID))
			    {
				    Debug.LogError("UserAuthentication failed or returned null/empty userID!");
			    }

			    PlayerPrefs.SetString("UserID", userID);
			    PlayerPrefs.Save();
		    }
		    else
		    {
			    startNoUser.SetActive(false);
			    startUserExists.SetActive(true);
			    welcomeWithName.text = "Velkommen tilbage" + "\n" + userName + "!";
		    }

		    isUserReady = true;

		    foreach (Toggle toggle in toggles)
		    {
			    toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle, isOn));
		    }
	    }
	    else
	    {
		    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
	    }
    }


    public void changeWindow(int windowIndex)
	    {
		    if (isUserReady)
		    {
		    float targetX = pos * windowIndex;
		    transform.DOLocalMoveX(targetX, tweenSpeed).SetEase(tweenEase);
		    }
	    }

        public void setName(bool beboer)
        {
	        string n = "";
	        if (beboer)
	        {
		        n = beboerInput.text.Trim();
	        }
	        else
	        {
		        n = ansatInput.text.Trim();
	        }
	        
			PlayerPrefs.SetString("Name", n);

			nameDisplay1.text = n + "!";
			nameDisplay2.text = n + "!";
        }

        public void setPet()
        {
	        PlayerPrefs.SetString("PetName", petName.text.Trim());
	        for (int i = 0; i < toggles.Count; i++)
	        {
		        if (toggles[i].isOn)
		        {
			        PlayerPrefs.SetInt("Pet", i);
			        changeWindow(4);
			        return;
		        }
	        }
        }

        public void checkCode()
        {
	        int userCode = int.Parse(codeInput.text);
	        

	        if (userCode == code)
	        {
		        changeWindow(6);
	        }
	        else
	        {
		        changeWindow(8);
		        codeInput.text = "";
	        }
        }

        public void setRole(string role)
        {
	        PlayerPrefs.SetString("Role", role);
        }

        public async void goToMainScreen()
        {
	        Debug.Log($"goToMainScreen called; isUserReady={isUserReady}; UserID={PlayerPrefs.GetString("UserID")}");

	        // Set Start Time and Date
	        DateTime tomorrow = DateTime.Now.Date.AddDays(1);
	        DateTime nextUpdate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 0, 0);
	        PlayerPrefs.SetString("UpdateTime", nextUpdate.ToString("yyyy-MM-dd"));
	        PlayerPrefs.SetInt("Dollars", 0);
	        PlayerPrefs.SetInt("DaysActive", 1);

	        string userId = PlayerPrefs.GetString("UserID", "");
	        string name = PlayerPrefs.GetString("Name", "NoN");

	        if (!isUserReady)
	        {
		        Debug.LogError("UserID not ready yet — aborting navigation.");
		        return;
	        }

	        bool added = await fish.AddNewUser(userId, name);

	        fish.RegisterDeviceTokenForUser(userId);
	        fish.TokenSave(userId, false);
	        
	        if (!added)
	        {
		        Debug.LogWarning("User creation failed or user already exists.");
	        }
	        
			SceneManager.LoadScene("MainScreen");

        }


        public async void goToAnsatMainScreen()
        {
	        Debug.Log($"goToMainScreen called; isUserReady={isUserReady}; UserID={PlayerPrefs.GetString("UserID")}");

	        if (!isUserReady)
	        {
		        Debug.LogError("UserID not ready yet — aborting navigation.");
		        return;
	        }

	        string userId = PlayerPrefs.GetString("UserID", "");
	        string name = PlayerPrefs.GetString("Name", "NoN");

	        if (string.IsNullOrEmpty(userId))
	        {
		        Debug.LogError("Cannot proceed: UserID is empty or not yet set.");
		        return;
	        }

	        bool added = await fish.AddStaffMember(userId, name);

	        fish.RegisterDeviceTokenForUser(userId);
	        fish.TokenSave(userId, true);

	        if (!added)
	        {
		        Debug.LogWarning("Staff creation failed or staff already exists.");
	        }

	        SceneManager.LoadScene("ANSAT_MainScreen");
        }


        public void goToEitherMain()
        {
	        string role = PlayerPrefs.GetString("Role");

	        if (role == "Beboer")
	        {
		        SceneManager.LoadScene("MainScreen");
	        }
	        else
	        {
		        SceneManager.LoadScene("ANSAT_MainScreen");
	        }
        }
        
        void OnToggleChanged(Toggle changedToggle, bool isOn)
        {
	        if (!isOn)
		        return; 

	        // Turn off all others
	        foreach (var toggle in toggles)
	        {
		        if (toggle != changedToggle)
		        {
			        toggle.isOn = false;
		        }
	        }
        }
}
