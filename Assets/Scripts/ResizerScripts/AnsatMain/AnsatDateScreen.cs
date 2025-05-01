using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

	public float fontSizeMinDate;
	public float fontSizeMaxDate;
	
	public float fontSizeMin;
	public float fontSizeMax;
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
		
		// FONT SIZE - SCHEDULE
		float textFontSize = Mathf.Lerp(fontSizeMin, fontSizeMax, t);
		for (int layoutIndex = 1; layoutIndex <= 2; layoutIndex++)
		{
			if (layoutIndex < scheduleLayouts.Count)
			{
				Transform layout = scheduleLayouts[layoutIndex].transform;
				for (int i = 0; i < layout.childCount; i++)
				{
					TextMeshProUGUI text = layout.GetChild(i).GetComponent<TextMeshProUGUI>();
					if (text != null)
					{
						text.fontSize = textFontSize;
					}
				}
			}
		}
		
		// HEIGHT - INPUT FIELDS
		float inputHeight = Mathf.Lerp(InputMinY, InputMaxY, t);
		for (int i = 0; i < 7; i++)
		{
			RectTransform inputRect = inputFields.transform.GetChild(i).GetComponent<RectTransform>();
			if (inputRect != null)
			{
				Vector2 size = inputRect.sizeDelta;
				size.y = inputHeight;
				inputRect.sizeDelta = size;
			}
		}
		inputFields.SetActive(false);
	}
}