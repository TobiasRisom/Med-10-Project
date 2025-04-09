using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
	public GameObject taskTemplate;
	public string userName = "";

	public List<GameObject> tasks;
	
	public class Task
	{
		public string Titel { get; set; }
		public string Emoji { get; set; }
		public string Description { get; set; }
		public string AnswerFormat { get; set; }
		public int Status { get; set; }
	}
	
	public List<Task> TaskData = new List<Task>();

	public int currentTask;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		getInitialTestTasks();
	}

	private void getInitialTestTasks()
	{
		TaskData.Add(new Task { Titel = "Støvsug Gulvet", Emoji = ":)", Description = "Husk at støvsuge gulvet en gang om ugen. Vær sikker på at gulvet i hele køkkenet og stuen bliver gjort.\nTør efter hvis der stadig ligger snavs.", AnswerFormat = "P" });
		TaskData.Add(new Task { Titel = "Skab en tingenot", Emoji = ":D", Description = "Du burde lave en sjov tingest idag.", AnswerFormat = "P" });
		TaskData.Add(new Task { Titel = "Av min finger", Emoji = ":(", Description = "Tag en pause.", AnswerFormat = "T" });
	}

	public void setTasks()
	{
		int taskAmount = TaskData.Count;

		for (int i = 0; i < taskAmount; i++)
		{
			tasks[i]
				.transform.GetChild(0)
				.GetComponent<TextMeshProUGUI>()
				.text = TaskData[i].Titel;
			tasks[i]
				.transform.GetChild(1)
				.GetComponent<TextMeshProUGUI>()
				.text = TaskData[i].Emoji;
			tasks[i]
				.transform.GetChild(2)
				.GetComponent<TextMeshProUGUI>()
				.text = TaskData[i].Description;
			tasks[i]
				.transform.GetChild(3)
				.GetComponent<TextMeshProUGUI>()
				.text = TaskData[i].AnswerFormat;
		}
	}
}
