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
	    
	    
		//PlayerPrefs.SetString("Name", "Thomas");
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
}
