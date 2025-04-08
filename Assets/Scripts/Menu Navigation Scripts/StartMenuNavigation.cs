using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuNavigation : MonoBehaviour
{
    private float pos = -1080;
    private int code = 9827;
    
    public TMP_InputField inputField;
    
		public void changeWindow(int windowIndex)
        {
            transform.localPosition = new Vector3(pos * windowIndex, transform.localPosition.y, transform.localPosition.z);
        }

        public void checkCode()
        {
	        int userCode = int.Parse(inputField.text);
	        

	        if (userCode == code)
	        {
		        changeWindow(4);
	        }
	        else
	        {
		        changeWindow(5);
		        inputField.text = "";
	        }
        }

        public void goToMainScreen()
        {
	        SceneManager.LoadScene("MainScreen");
        }
}
