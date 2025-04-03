using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuNavigation : MonoBehaviour
{
    private float pos = -1080;
    private int code = 9827;
    
    public void changeWindow(int windowIndex)
        {
            transform.localPosition = new Vector3(pos * windowIndex, transform.localPosition.y, transform.localPosition.z);
        }

        public void checkCode()
        {
	        TMP_InputField codeInput = GameObject.FindWithTag("codeInput")
	                                         .GetComponent<TMP_InputField>();

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
}
