using System;
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

    private string userName;

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    
		PlayerPrefs.SetString("Name", "NoN");
	    userName = PlayerPrefs.GetString("Name", "NoN");

	    if (userName == "NoN")
	    {
		    // No user
		    startNoUser.SetActive(true);
		    startUserExists.SetActive(false);
	    }
	    else
	    {
		    // User exists
		    startNoUser.SetActive(false);
		    startUserExists.SetActive(true);
		    welcomeWithName.text = "Velkommen tilbage" + "\n" + userName + "!";
	    }
	    
	    if (isItUpdateTime())
	    {
		    UpdateTasks();
	    }
    }
    
		public void changeWindow(int windowIndex)
        {
            transform.localPosition = new Vector3(pos * windowIndex, transform.localPosition.y, transform.localPosition.z);
        }

        public void setName(string n)
        {
			PlayerPrefs.SetString("Name", n);

	        nameDisplay1.name = n;
	        nameDisplay2.name = n;
        }

        public void checkCode()
        {
	        int userCode = int.Parse(codeInput.text);
	        

	        if (userCode == code)
	        {
		        changeWindow(4);
	        }
	        else
	        {
		        changeWindow(5);
		        codeInput.text = "";
	        }
        }

        public void setRole(string role)
        {
	        PlayerPrefs.SetString("Role", role);
        }

        public void goToMainScreen()
        {
	        // Set Start Time and Date
	        DateTime tomorrow = DateTime.Now.Date.AddDays(1);
	        DateTime nextUpdate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 0, 0);
	        PlayerPrefs.SetString("UpdateTime", nextUpdate.ToString("yyyy-MM-dd"));
	        
	        
	        //fish.AddNewUser("Leif");
	        SceneManager.LoadScene("MainScreen");
        }

        public void goToAnsatMainScreen()
        {
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
        
        private Boolean isItUpdateTime()
        {
	        DateTime now = DateTime.Now;
	        string updateTime = PlayerPrefs.GetString("UpdateTime");
	    
	        if (DateTime.TryParse(updateTime, out DateTime storedTime))
	        {
		        if (now >= storedTime)
		        {
			        // Update Time!
			        return true;
		        }
	        }
	        return false;
        }

        private void UpdateTasks()
        {
	        fish.UpdateDailyAndWeeklyTasks((((int)DateTime.Now.DayOfWeek + 6) % 7) + 2); // Monday = 2, Tuesday = 3, Wednesday = 4...
	        
	        // Change next update date to tomorrow
	        DateTime tomorrow = DateTime.Now.Date.AddDays(1);
	        DateTime nextUpdate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 0, 0);
	        PlayerPrefs.SetString("UpdateTime", nextUpdate.ToString("yyyy-MM-dd"));
        }
}
