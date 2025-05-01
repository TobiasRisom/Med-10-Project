using TMPro;
using UnityEngine;

public class AnsatDateScreen : MonoBehaviour
{
	public TextMeshProUGUI bigDate;
	public TextMeshProUGUI smallDate;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;
		if (screenHeight < 2100)
		{
			bigDate.gameObject.SetActive(false);
			smallDate.gameObject.SetActive(true);
		}
		else if (screenHeight < 2250)
		{
			bigDate.gameObject.SetActive(true);
			smallDate.gameObject.SetActive(false);

			bigDate.fontSize = 50;
		}
		else
		{
			bigDate.gameObject.SetActive(true);
			smallDate.gameObject.SetActive(false);

			bigDate.fontSize = 90;
		}
	}
}