using UnityEngine;
using UnityEngine.UI;

public class ButtonClickHandler : MonoBehaviour
{
	public JsonServerCommunicator serverCommunicator; // Reference to your JsonServerCommunicator script
	public Button yourButton; // Reference to the Button UI element

	void Start()
	{
		if (yourButton != null)
		{
			yourButton.onClick.AddListener(OnButtonClick);
		}
	}

	void OnButtonClick()
	{
		// This will call the GetGames method when the button is clicked
		StartCoroutine(serverCommunicator.GetGames());
	}
}