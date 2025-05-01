using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskScreenResize : MonoBehaviour
{
	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	public float buttonMinY;
	public float buttonMaxY;

	public float textMinYPhoto;
	public float textMinYWrite;
	public float textMaxY;
	
	public List<RectTransform> allButtons;
	public RectTransform texts;
	public List<RectTransform> answerCheck;
    void Start()
    {
	    float screenHeight = GameObject.FindWithTag("MainCanvas")
	                                   .GetComponent<RectTransform>()
	                                   .rect.height;
	    
	    float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);
	    
	    // ALL BUTTONS POS Y
	    float buttonsY = Mathf.Lerp(buttonMinY, buttonMaxY, t);
	    
		foreach(RectTransform button in allButtons)
		{
			button.anchoredPosition = new Vector2(button.anchoredPosition.x, buttonsY);
		}
		
		// TEXTS
		float textsY;
		if (transform.GetComponent<RectTransform>().anchoredPosition.x < 500)
		{
			textsY = Mathf.Lerp(textMinYPhoto, textMaxY, t);
		}
		else
		{
			textsY = Mathf.Lerp(textMinYWrite, textMaxY, t);
		}
		texts.anchoredPosition = new Vector2(texts.anchoredPosition.x, textsY);

		if (screenHeight < 2300)
		{
			texts.transform.GetChild(2).gameObject.SetActive(false);
		}
		
		// CONFIRM ANSWER OBJECTS
		float downY = Mathf.Lerp(0, 120, t);

		foreach (RectTransform rect in answerCheck)
		{
			rect.anchoredPosition = new Vector2(texts.anchoredPosition.x, texts.anchoredPosition.y - downY);
		}
    }
}
