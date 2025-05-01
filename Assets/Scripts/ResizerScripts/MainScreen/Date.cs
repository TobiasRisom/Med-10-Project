using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Date : MonoBehaviour
{
	// Screen height bounds
	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	public List<GameObject> scheduleLayouts;

	// UI properties corresponding to those bounds
	public float ScheduleminY;
	public float SchedulemaxY;

	public float posYMin;
	public float posYMax;
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
		
		// SCHEDULE Y'S
		float scheduleY = Mathf.Lerp(posYMin, posYMax, t);
		foreach (GameObject layout in scheduleLayouts)
		{
			RectTransform pos = layout.GetComponent<RectTransform>();
			pos.anchoredPosition = new Vector2(pos.anchoredPosition.x, scheduleY);
		}
	}
}