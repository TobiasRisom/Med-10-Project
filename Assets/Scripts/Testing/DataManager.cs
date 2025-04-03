using System;
using UnityEngine;
using TMPro;

public class DataManager : MonoBehaviour
{
	public string name = "";

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
}
