using TMPro;
using UnityEngine;

public class AnsatUserOverview : MonoBehaviour
{
	public RectTransform scrollView;

	// Screen height bounds
	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	// UI properties corresponding to those bounds
	public float minY;
	public float maxY;

	void Start()
	{
		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;

		float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);
		float newY = Mathf.Lerp(minY, maxY, t);
		scrollView.anchoredPosition = new Vector2(0, newY);
	}
}