using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TaskScreenNavigation : MonoBehaviour
{
	private DataManager dm;
	
	public TextMeshProUGUI taskTitle;
	public TextMeshProUGUI taskEmoji;
	public TextMeshProUGUI taskDescription;

	public TMP_InputField userAnswer;
	public TextMeshProUGUI sendThisAnswer;
	public TextMeshProUGUI finalUserAnswer;
	public Button finalConfirm;
	public Button finalBack;

	public Button confirmWriting;
	public Button confirmPicture;
    void Start()
    {
	    dm = GameObject.FindWithTag("dataManager")
	                   .GetComponent<DataManager>();

	    int taskIndex = dm.currentTask;

	    taskTitle.text = dm.TaskData[taskIndex].Titel;
	    taskEmoji.text = dm.TaskData[taskIndex].Emoji;
	    taskDescription.text = dm.TaskData[taskIndex].Description;

	    if (dm.TaskData[taskIndex].AnswerFormat == "P")
	    {
		    transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
	    }
    }

    // Update is called once per frame
    void Update()
    {
        checkInputFieldContents();
    }

    void checkInputFieldContents()
    {
	    if (userAnswer.text.Length > 0)
	    {
		    confirmWriting.gameObject.SetActive(true);
	    }
	    else
	    {
		    confirmWriting.gameObject.SetActive(false);
	    }
    }

    public void goToConfirmAnswer() // Move on to confirm answer
    {
	    sendThisAnswer.gameObject.SetActive(true);
	    finalConfirm.gameObject.SetActive(true);
	    finalBack.gameObject.SetActive(true);
	    
	    finalUserAnswer.text = userAnswer.text;
	    userAnswer.text = "";
	    userAnswer.gameObject.SetActive(false);
	    finalUserAnswer.gameObject.SetActive(true);
    }

    public void answerConfirmed()
    {
	    dm.TaskData.RemoveAt(dm.currentTask);
	    SceneManager.LoadScene("MainScreen");
    }

    public void answerGoBack()
    {
	    sendThisAnswer.gameObject.SetActive(false);
	    finalConfirm.gameObject.SetActive(false);
	    finalBack.gameObject.SetActive(false);
	    
	    userAnswer.gameObject.SetActive(true);
	    finalUserAnswer.gameObject.SetActive(false);
    }
}
