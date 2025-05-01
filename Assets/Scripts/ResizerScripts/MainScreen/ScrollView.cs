using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollView : MonoBehaviour
{
	public RectTransform scrollView;
	public List<RectTransform> headlines;

	// Screen height bounds
	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	// UI properties corresponding to those bounds
	public float minY;
	public float maxY;
	public float minHeightValue;
	public float maxHeightValue;

	public float headlineMin;
	public float headlineMax;

	void Start()
	{
		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;

		float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);

		// SCROLL VIEW
		float newY = Mathf.Lerp(minY, maxY, t);
		float newHeight = Mathf.Lerp(minHeightValue, maxHeightValue, t);

		scrollView.anchoredPosition = new Vector2(scrollView.anchoredPosition.x, newY);
		Vector2 size = scrollView.sizeDelta;
		size.y = newHeight;
		scrollView.sizeDelta = size;
		
		// HEADLINES
		float headlinesY = Mathf.Lerp(headlineMin, headlineMax, t);

		foreach (RectTransform headline in headlines)
		{
			headline.anchoredPosition = new Vector2(headline.anchoredPosition.x, headlinesY);
		}
	}
}