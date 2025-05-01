using TMPro;
using UnityEngine;

public class PetMessage : MonoBehaviour
{
	private TextMeshProUGUI messageText; // Assign this in the Inspector
	private string petName;   // Example pet name

	private string[] messages = new string[]
	{
		"PETNAME glæder sig til at du klarer flere opgaver \ud83d\udccb",
		"PETNAME er i godt humør \ud83d\ude04",
		"PETNAME drømmer om solskinsvejr \u2600\ufe0f",
		"PETNAME føler sig lidt søvnig \ud83d\ude2a",
		"PETNAME klør sig lidt bag øret \ud83d\ude0c",
		"PETNAME har lige spist en snack \ud83c\udf6c",
		"PETNAME hepper på dig! \ud83c\udf89",
		"PETNAME er glad lige nu \ud83d\ude04",
		"PETNAME har lige taget en lur \ud83d\ude34",
		"PETNAME synes at skyerne ser pæne ud lige nu \ud83c\udf25\ufe0f",
		"PETNAME føler sig tilfreds \ud83d\ude0c",
		"PETNAME tænker på hvad de vil have at spise imorgen \ud83e\udd14",
		"PETNAME er super glad lige nu \ud83d\ude04",
		"PETNAME glæder sig til sin næste lur \ud83d\ude34",
		"PETNAME har taget et bad og ser meget fin ud nu \ud83d\udebf"
	};

	void Start()
	{
		petName = PlayerPrefs.GetString("PetName");
		messageText = GetComponent<TextMeshProUGUI>();
		if (PlayerPrefs.GetInt("StartPetMessage") == 0)
		{
			PlayerPrefs.SetInt("StartPetMessage", 1);
			ShowStartMessage();
		}
		else
		{
			ShowRandomMessage();	
		}
	}

	void ShowRandomMessage()
	{
		int index = Random.Range(0, messages.Length);
		string chosenMessage = messages[index].Replace("PETNAME", petName);
		messageText.text = chosenMessage;
	}

	void ShowStartMessage()
	{
		string message = "PETNAME byder dig velkommen til ØG APP! \ud83d\udc4b";
		string chosenMessage = message.Replace("PETNAME", petName);
		messageText.text = chosenMessage;
	}
}