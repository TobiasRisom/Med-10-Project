using System;
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
    
    private float tweenSpeed = 0.5f;
    private Ease tweenEase = Ease.OutQuad;

    private string userName;

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
	    }
	    else
	    {
		    // User exists
		    startNoUser.SetActive(false);
		    startUserExists.SetActive(true);
		    welcomeWithName.text = "Velkommen tilbage" + "\n" + userName + "!";
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
		        n = beboerInput.text;
	        }
	        else
	        {
		        n = ansatInput.text;
	        }
	        
			PlayerPrefs.SetString("Name", n);

			nameDisplay1.text = n + "!";
			nameDisplay2.text = n + "!";
        }

        public void checkCode()
        {
	        int userCode = int.Parse(codeInput.text);
	        

	        if (userCode == code)
	        {
		        changeWindow(5);
	        }
	        else
	        {
		        changeWindow(6);
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
	        
	        
	        fish.AddNewUser("Leif");
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
}
