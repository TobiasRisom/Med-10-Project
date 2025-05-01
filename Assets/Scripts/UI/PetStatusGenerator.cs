using TMPro;
using UnityEngine;

public class PetMessage : MonoBehaviour
{
	private TextMeshProUGUI messageText; // Assign this in the Inspector
	private string petName;   // Example pet name

	private string[] messages = new string[]
	{
		"PETNAME glæder sig til at du klarer flere opgaver",
		"PETNAME er i godt humør",
		"PETNAME drømmer om solskinsvejr",
		"PETNAME føler sig lidt søvnig",
		"PETNAME klør sig lidt bag øret",
		"PETNAME har lige spist en snack",
		"PETNAME hepper på dig!",
		"PETNAME er glad lige nu",
		"PETNAME har lige taget en lur",
		"PETNAME synes at skyerne ser pæne ud lige nu",
		"PETNAME føler sig tilfreds",
		"PETNAME tænker på hvad de vil have at spise imorgen",
		"PETNAME er super glad lige nu",
		"PETNAME glæder sig til sin næste lur",
		"PETNAME har taget et bad og ser meget fin ud nu"
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
		string message = "PETNAME byder dig velkommen til ØG APP!";
		string chosenMessage = message.Replace("PETNAME", petName);
		messageText.text = chosenMessage;
	}
}