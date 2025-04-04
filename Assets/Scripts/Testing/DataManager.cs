using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
	public GameObject taskTemplate;
	public string name = "";

	public string[] weekNames = {"Leif", "Leeeeiiiiiiffff", "LeifLeif", "Lief", "It'sMyLeif", "KarlosIkkeLeif", "Looooof"};

	public List<GameObject> tasks;
	
	public string[,] taskData = { {"Støvsug Gulvet", ":)", "Husk at støvsuge gulvet en gang om ugen. Vær sikker på at gulvet i hele køkkenet og stuen bliver gjort.\nTør efter hvis der stadig ligger snavs."}
								, {"Skab en tingenot", ":D", "Du burde lave en sjov tingest idag."}
								, {"Av min finger", ":(", "Tag en pause."}
	};

	public int currentTask;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void setName(bool ansat)
	{
		TMP_InputField nameInput;
		if (ansat)
		{
			nameInput = GameObject.FindWithTag("ansatName")
			                                     .GetComponent<TMP_InputField>();	
		}
		else
		{
			nameInput = GameObject.FindWithTag("beboerName")
			                                     .GetComponent<TMP_InputField>();
		}

		name = nameInput.text;

		GameObject[] nameDisplays = GameObject.FindGameObjectsWithTag("nameDisplay");
		foreach (GameObject n in nameDisplays)
		{
			n.GetComponent<TextMeshProUGUI>()
			 .text = name + "!";
		}
	}

	public void spawnTasks()
	{
		GameObject mainScreen = GameObject.FindWithTag("mainScreen");
		int taskAmount = taskData.GetLength(1);
		int buffer = -300;

		for (int i = 0; i < taskAmount; i++)
		{
			GameObject newTask = Instantiate(taskTemplate, mainScreen.transform, false);
			newTask.transform.localPosition = new Vector3(0, buffer * i, 0);
			
			tasks.Add(newTask);
		}

	}

	public void setTasks()
	{
		int taskAmount = taskData.GetLength(1);

		for (int i = 0; i < taskAmount; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				tasks[i]
					.transform.GetChild(j)
					.GetComponent<TextMeshProUGUI>()
					.text = taskData[i, j];
			}
		}
	}
}
