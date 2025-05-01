using TMPro;
using UnityEngine;

public class Ansat : MonoBehaviour
{
	public RectTransform viewport;
	public RectTransform stats;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;
		
		if (screenHeight < 2200)
		{
			viewport.anchoredPosition = new Vector2(0, 150);
			Vector2 size = viewport.sizeDelta;
			size.y = 850;
			viewport.sizeDelta = size;

			stats.anchoredPosition = new Vector2(0, -485);
		}
	}
}