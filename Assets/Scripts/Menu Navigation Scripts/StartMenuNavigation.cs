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
    private string userID;
    
    private float tweenSpeed = 0.5f;
    private Ease tweenEase = Ease.OutQuad;

    private string userName;
    
    public List<Toggle> toggles;
    public List<GameObject> hearts;

    public TMP_InputField petName;

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    
		//PlayerPrefs.SetString("Name", "NoN");
		userName = PlayerPrefs.GetString("Name", "NoN");

	    if (userName == "NoN")
	    {
		    // No user
		    startNoUser.SetActive(true);
		    startUserExists.SetActive(false);
		    userID = fish.UserAuthentication();
	    }
	    else
	    {
		    // User exists
		    startNoUser.SetActive(false);
		    startUserExists.SetActive(true);
		    welcomeWithName.text = "Velkommen tilbage" + "\n" + userName + "!";
	    }
	    
	    foreach (Toggle toggle in toggles)
	    {
		    toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle, isOn));
	    }
    }
    
	    public void changeWindow(int windowIndex)
	    {
		    float targetX = pos * windowIndex;
		    transform.DOLocalMoveX(targetX, tweenSpeed).SetEase(tweenEase);
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
	        // Set Start Time and Date
	        DateTime tomorrow = DateTime.Now.Date.AddDays(1);
	        DateTime nextUpdate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 0, 0);
	        PlayerPrefs.SetString("UpdateTime", nextUpdate.ToString("yyyy-MM-dd"));
	        PlayerPrefs.SetInt("Dollars", 0);
	        PlayerPrefs.SetInt("DaysActive", 1);
	        
	        bool added = await fish.AddNewUser(userID, PlayerPrefs.GetString("Name", "NoN"));

	        if (added)
	        {
		        fish.RegisterDeviceTokenForUser(userID);
		        SceneManager.LoadScene("MainScreen");
	        }
	        else
	        {
		        Debug.LogWarning("User creation failed or user already exists.");
		        // Handle accordingly (maybe still register token or just proceed)
		        fish.RegisterDeviceTokenForUser(userID);
		        SceneManager.LoadScene("MainScreen");
	        }
        }

        public async void goToAnsatMainScreen()
        {
	        bool added = await fish.AddStaffMember(userID, PlayerPrefs.GetString("Name", "NoN"));

	        if (added)
	        {
		        fish.RegisterDeviceTokenForUser(userID);
		        SceneManager.LoadScene("ANSAT_MainScreen");
	        }
	        else
	        {
		        Debug.LogWarning("Staff creation failed or staff already exists.");
		        // Optionally still register token & load scene if desired
		        fish.RegisterDeviceTokenForUser(userID);
		        SceneManager.LoadScene("ANSAT_MainScreen");
	        }
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
