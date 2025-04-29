using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetTodayInSchedule : MonoBehaviour
{
	public List<TextMeshProUGUI> dates;

	void Start()
	{
		int today = ((int)DateTime.Now.DayOfWeek + 6) % 7; // 0 = Monday, 6 = Sunday

		for (int i = 0; i < 7; i++)
		{
			if (i == today)
			{
				dates[i].text = dates[i].text + " \ud83d\udcc5";
				break;
			}
		}
	}
}
