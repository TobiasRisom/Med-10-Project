using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnsatDateScreen : MonoBehaviour
{
	// Screen height bounds
	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	public List<GameObject> scheduleLayouts;
	public GameObject inputFields;
	public TextMeshProUGUI date;

	// UI properties corresponding to those bounds
	public float ScheduleminY;
	public float SchedulemaxY;

	public float InputMinY;
	public float InputMaxY;

	public float SpacingMin;
	public float SpacingMax;

	public float fontSizeMinDate;
	public float fontSizeMaxDate;
	void Start()
	{

		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;
		
		float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);
		
		
		// SCHEDULE HEIGHTS
		float scheduleHeight = Mathf.Lerp(ScheduleminY, SchedulemaxY, t);
		foreach (GameObject layout in scheduleLayouts)
		{
			for (int i = 0; i < layout.transform.childCount; i++)
			{
				RectTransform childRect = layout.transform.GetChild(i).GetComponent<RectTransform>();
				if (childRect != null)
				{
					Vector2 size = childRect.sizeDelta;
					size.y = scheduleHeight;
					childRect.sizeDelta = size;
				}
			}
		}
		
		// FONT SIZE - DATE
		float dateFontSize = Mathf.Lerp(fontSizeMinDate, fontSizeMaxDate, t);
		date.fontSize = dateFontSize;
		
		// HEIGHT - INPUT FIELDS
		float inputY = Mathf.Lerp(InputMinY, InputMaxY, t);
		float inputSpacing = Mathf.Lerp(SpacingMin, SpacingMax, t);
		inputFields.GetComponent<VerticalLayoutGroup>()
		           .spacing = inputSpacing;
		for (int i = 0; i < 7; i++)
		{
			RectTransform inputRect = inputFields.transform.GetChild(i).GetComponent<RectTransform>();
			if (inputRect != null)
			{
				inputRect.anchoredPosition = new Vector2(inputRect.anchoredPosition.x, inputY);
			}
		}
		inputFields.SetActive(false);
	}
}