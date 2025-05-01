using TMPro;
using UnityEngine;

public class ScrollView : MonoBehaviour
{
	public RectTransform scrollView;

	// Screen height bounds
	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	// UI properties corresponding to those bounds
	private const float minY = -225f;
	private const float maxY = -352f;
	private const float minHeightValue = 950f;
	private const float maxHeightValue = 1150f;

	void Start()
	{
		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;

		float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);

		float newY = Mathf.Lerp(minY, maxY, t);
		float newHeight = Mathf.Lerp(minHeightValue, maxHeightValue, t);

		scrollView.anchoredPosition = new Vector2(0, newY);
		Vector2 size = scrollView.sizeDelta;
		size.y = newHeight;
		scrollView.sizeDelta = size;
	}
}