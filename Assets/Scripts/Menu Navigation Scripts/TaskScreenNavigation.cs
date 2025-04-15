using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TaskScreenNavigation : MonoBehaviour
{

	public TextMeshProUGUI taskTitle;
	public TextMeshProUGUI taskEmoji;
	public TextMeshProUGUI taskDescription;

	public TMP_InputField userAnswer;
	public TextMeshProUGUI sendThisAnswer;
	public TextMeshProUGUI finalUserAnswer;
	public Button cancelButton;
	public Button finalConfirm;
	public Button finalBack;

	public Button confirmWriting;
	public Button confirmPicture;

	private FirestoreHandler fish;
	private CameraDisplay cd;
    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                   .GetComponent<FirestoreHandler>();

	    cd = GameObject.FindWithTag("CameraDisplay")
	                   .GetComponent<CameraDisplay>();

	    int taskIndex = fish.currentTask;

	    taskTitle.text = fish.TaskData[taskIndex].Titel;
	    taskEmoji.text = fish.TaskData[taskIndex].Emoji;
	    taskDescription.text = fish.TaskData[taskIndex].Description;

	    if (fish.TaskData[taskIndex].ImageFormat)
	    {
		    transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
		    cd.SetupCamera();
	    }
	    else
	    {
		    transform.localPosition = new Vector3(1080, transform.localPosition.y, transform.localPosition.z);
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
	    cancelButton.gameObject.SetActive(false);
	    
	    finalUserAnswer.text = userAnswer.text;
	    userAnswer.text = "";
	    userAnswer.gameObject.SetActive(false);
	    finalUserAnswer.gameObject.SetActive(true);
    }

    public void cancel()
    {
	    answerGoBack();
	    userAnswer.text = "";
	    SceneManager.LoadScene("MainScreen");
    }

    public void writtenAnswerConfirmed()
    {
		fish.submitTask(fish.username, finalUserAnswer.text);
		SceneManager.LoadScene("MainScreen");
    }
    
    public void imageAnswerConfirmed()
    {
	    if (cd.photo != null)
	    {
		    byte[] imageBytes = cd.photo.EncodeToJPG(); 
		    string base64Image = System.Convert.ToBase64String(imageBytes);
		    int fileSizeBytes = base64Image.Length;
		    float fileSizeKB = fileSizeBytes / 1024f;
		    
		    fish.submitTask(fish.username, base64Image);
		    SceneManager.LoadScene("MainScreen");
	    }
    }

    public void answerGoBack()
    {
	    sendThisAnswer.gameObject.SetActive(false);
	    finalConfirm.gameObject.SetActive(false);
	    finalBack.gameObject.SetActive(false);
	    cancelButton.gameObject.SetActive(true);
	    
	    userAnswer.gameObject.SetActive(true);
	    finalUserAnswer.gameObject.SetActive(false);
    }
}
