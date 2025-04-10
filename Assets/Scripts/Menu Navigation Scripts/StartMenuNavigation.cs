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

    private FirestoreHandler fish;

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
    }
    
		public void changeWindow(int windowIndex)
        {
            transform.localPosition = new Vector3(pos * windowIndex, transform.localPosition.y, transform.localPosition.z);
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

        public void goToMainScreen()
        {
	        //fish.AddNewUser("Leif");
	        SceneManager.LoadScene("MainScreen");
        }

        public void goToAnsatMainScreen()
        {
	        SceneManager.LoadScene("ANSAT_MainScreen");
        }
}
