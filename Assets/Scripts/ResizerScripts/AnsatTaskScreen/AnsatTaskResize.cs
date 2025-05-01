using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AnsatTaskResize : MonoBehaviour
{

	private const int minHeight = 1920;
	private const int maxHeight = 2412;

	[Header("Page 1")]
	public RectTransform ScrollView;
	public float ScrollMinHeight;
	public float ScrollMaxHeight;

	[Header("Page 2")]
	public List<RectTransform> Buttons;
	public float ButtonHeightminY;

	public float ButtonHeightmaxY;

	public float ButtonFontminY;
	public float ButtonFontmaxY;

	[Header("Page 3")]
	public RectTransform Titel;
	public RectTransform Desc;
	public RectTransform Emoji;
	public RectTransform Dropdown;
	public RectTransform AnswerFormat;
	public RectTransform Button3rdPage;
	
	public float TitelminY;

	public float TitelmaxY;

	public float DescminY;
	public float DescmaxY;

	public float EmojiminY;
	public float EmojimaxY;

	public float DropDownminY;
	public float DropDownmaxY;

	public float AnswerFormatminY;
	public float AnswerFormatmaxY;

	public float Button3rdPageminY;
	public float Button3rdPagemaxY;

	public float ButtonHeight3rdpageminY;
	public float ButtonHeight3rdpagemaxY;

	void Start()
	{
		float screenHeight = GameObject.FindWithTag("MainCanvas")
		                               .GetComponent<RectTransform>()
		                               .rect.height;

		float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);

		// Lerp values
		float scrollHeight = Mathf.Lerp(ScrollMinHeight, ScrollMaxHeight, t);
		float buttonHeight = Mathf.Lerp(ButtonHeightminY, ButtonHeightmaxY, t);
		float buttonFontSize = Mathf.Lerp(ButtonFontminY, ButtonFontmaxY, t);
		float titelY = Mathf.Lerp(TitelminY, TitelmaxY, t);
		float descY = Mathf.Lerp(DescminY, DescmaxY, t);
		float emojiY = Mathf.Lerp(EmojiminY, EmojimaxY, t);
		float dropdownY = Mathf.Lerp(DropDownminY, DropDownmaxY, t);
		float answerFormatY = Mathf.Lerp(AnswerFormatminY, AnswerFormatmaxY, t);
		float button3rdPageY = Mathf.Lerp(Button3rdPageminY, Button3rdPagemaxY, t);
		float button3rdPageHeight = Mathf.Lerp(ButtonHeight3rdpageminY, ButtonHeight3rdpagemaxY, t);

		// Set Y positions
		SetYPosition(Titel, titelY);
		SetYPosition(Desc, descY);
		SetYPosition(Emoji, emojiY);
		SetYPosition(Dropdown, dropdownY);
		SetYPosition(AnswerFormat, answerFormatY);
		SetYPosition(Button3rdPage, button3rdPageY);

		// Set heights
		SetHeight(ScrollView, scrollHeight);
		SetHeight(Button3rdPage, button3rdPageHeight);

		// Set height and font of buttons
		foreach (RectTransform button in Buttons)
		{
			SetHeight(button, buttonHeight);

			TMPro.TextMeshProUGUI text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
			if (text != null)
			{
				text.fontSize = buttonFontSize;
			}
		}
	}
	
	void SetYPosition(RectTransform rect, float newY)
	{
		if (rect != null)
		{
			Vector2 anchoredPos = rect.anchoredPosition;
			anchoredPos.y = newY;
			rect.anchoredPosition = anchoredPos;
		}
	}

	void SetHeight(RectTransform rect, float height)
	{
		if (rect != null)
		{
			Vector2 size = rect.sizeDelta;
			size.y = height;
			rect.sizeDelta = size;
		}
	}
}
