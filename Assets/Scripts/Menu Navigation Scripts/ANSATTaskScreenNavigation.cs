using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

public class ANSATTaskScreenNavigation : MonoBehaviour
{
	private readonly float pos = -1080;
	private bool preventRecursion = false;
	private FirestoreHandler fish;

	[Header("New Task Inputs")]
	public TMP_InputField inputTitle;
	public TMP_InputField inputEmoji;
	public TMP_InputField inputDescription;
	public Toggle inputText;
	public Toggle inputImage;

	public Button continueToConfirmTask;
	
	[Header("Task Display")]
	public TextMeshProUGUI confirmTitle;
	public TextMeshProUGUI confirmEmoji;
	public TextMeshProUGUI confirmDescription;
	public TextMeshProUGUI answerFormat;
	public TextMeshProUGUI status;
	public TextMeshProUGUI usersToGetTask;
	public TextMeshProUGUI howOftenToRepeatTask;

	public List<TextMeshProUGUI> displays = new List<TextMeshProUGUI>();

	public bool imageFormat;

	private List<string> users = new List<string>();
	public GameObject userToggleParent;
	
	private enum Repetition
	{
		Once,
		Daily,
		Weekly
	}
	
	private Repetition howOften;

	void Start()
	{
		fish = GameObject.FindWithTag("dataManager")
		                 .GetComponent<FirestoreHandler>();
		
		fish.SetUserToggles();
	}

	void Update()
	{
		continueToConfirmTask.gameObject.SetActive(setContinueButtonActive());
	}
	
	private bool setContinueButtonActive()
	{
		if (inputTitle.text.Length > 0 && inputEmoji.text.Length > 0 && inputDescription.text.Length > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void changeWindow(int windowIndex)
	{
		transform.localPosition = new Vector3(pos * windowIndex, transform.localPosition.y, transform.localPosition.z);
	}

	public void setRepetitionParameter(int time) // 0 = Once, 1 = Daily, 2 = Weekly
    {
	    if (time == 0)
	    {
		    howOften = Repetition.Once;
	    }
	    else if (time == 1)
	    {
		    howOften = Repetition.Daily;
	    }
	    else if (time == 2)
	    {
		    howOften = Repetition.Weekly;
	    }
    }

	public void setUserAmount()
	{
		users.Clear();
		foreach (RectTransform rectTransform in userToggleParent.transform)
		{
			Toggle toggle = rectTransform.gameObject.GetComponent<Toggle>();
			if (toggle.isOn)
			{
				users.Add(toggle.GetComponentInChildren<TextMeshProUGUI>().text);
				print("Added user: " + toggle.GetComponentInChildren<TextMeshProUGUI>().text);
			}
		}
	}

    public void setTextFieldsToConfirm()
    {
	    confirmTitle.text = inputTitle.text;
	    confirmEmoji.text = inputEmoji.text;
	    confirmDescription.text = inputDescription.text;
	    answerFormat.text = inputText.isOn ? "T" : "P";
	    status.text = "0";

	    usersToGetTask.text = users.Count + " beboere";


	    if (howOften == Repetition.Once)
	    {
		    howOftenToRepeatTask.text = "En Gang";
	    }
	    else if (howOften == Repetition.Daily)
	    {
		    howOftenToRepeatTask.text = "Dagligt";
	    }
	    else
	    {
		    howOftenToRepeatTask.text = "Ugentligt";
	    }
    }

    public void toggleManager(bool whichToggle) // True = Text, False = Image
    {
	    if (!preventRecursion)
	    {

		    preventRecursion = true;

		    if (whichToggle)
		    {
			    inputImage.isOn = !inputText.isOn;
		    }
		    else
		    {
			    inputText.isOn = !inputImage.isOn;
		    }

		    preventRecursion = false;
	    }

	    if (inputImage.isOn)
	    {
		    imageFormat = true;
	    }
	    else
	    {
		    imageFormat = false;
	    }
    }

    public void CreateNewTaskAndExit()
    {
	    FirestoreHandler.Task newTask = new FirestoreHandler.Task();

	    newTask.Titel = confirmTitle.text;
	    newTask.Emoji = confirmEmoji.text;
	    newTask.Description = confirmDescription.text;
	    newTask.ImageFormat = imageFormat;
	    newTask.Status = 0;
	    
	    switch(howOften)
	    {
		    case Repetition.Once:
				newTask.Repeat = 0; // Once!
				break;
		    
		    case Repetition.Daily:
			    newTask.Repeat = 1; // Daily!
			    break;
		    
		    case Repetition.Weekly:
			    newTask.Repeat = (((int)DateTime.Now.DayOfWeek + 6) % 7) + 2; // 2 = Monday, 3 = Tuesday, etc...
			    break;
	    }
	    
	    fish.addTaskToUsers(newTask, users);

	    SceneManager.LoadScene("ANSAT_MainScreen");
    }

    public void setUsers()
    {
	    displays[0].text = users.Count + " beboere";
	    displays[1].text = users.Count + " beboere";
    }

    public void setRepetition(string rep)
    {
	    displays[2].text = rep;
	    displays[3].text = rep;
    }

    public void GoBack()
    {
	    SceneManager.LoadScene("ANSAT_MainScreen");
    }
}
