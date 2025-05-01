using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskScreenResize : MonoBehaviour
{
	public List<RectTransform> photoObject;
	public List<RectTransform> allButtons;
	public RectTransform texts;
    void Start()
    {
	    float screenHeight = GameObject.FindWithTag("MainCanvas")
	                                   .GetComponent<RectTransform>()
	                                   .rect.height;

	    if (screenHeight < 2250)
	    {
		    foreach (RectTransform rt in photoObject)
		    {
			    rt.anchoredPosition = new Vector2(0, -675);
		    }

		    foreach (RectTransform rt in allButtons)
		    {
			    rt.anchoredPosition = new Vector2(0, 330);
		    }

		    texts.anchoredPosition = new Vector2(0, -552);
		    texts.transform.GetChild(2)
		         .GetComponent<TextMeshProUGUI>()
		         .color = new Color(1, 1, 1, 0);
	    }
    }
}
