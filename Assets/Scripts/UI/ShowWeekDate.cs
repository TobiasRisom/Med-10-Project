using UnityEngine;
using TMPro;
using System;
using System.Globalization;

public class WeekDisplay : MonoBehaviour
{
	// Reference to the TextMeshProUGUI component
	private TextMeshProUGUI weekText;

	void Start()
	{
		weekText = GetComponent<TextMeshProUGUI>();
		DisplayCurrentWeek();
	}

	void DisplayCurrentWeek()
	{
		// Get the current date
		DateTime currentDate = DateTime.Now;

		// Set the culture to Danish (Denmark)
		CultureInfo danishCulture = new CultureInfo("da-DK");

		// Get the current week number (ISO 8601)
		int currentWeekNumber = GetWeekNumber(currentDate);

		// Get the date of the Monday of the current week
		DateTime startOfWeek = GetStartOfWeek(currentDate);

		// Calculate the Sunday of the current week
		DateTime endOfWeek = startOfWeek.AddDays(6);

		// Format the start and end dates using Danish culture
		string startDate = $"{startOfWeek.Day}/{startOfWeek.Month}";
		string endDate = $"{endOfWeek.Day}/{endOfWeek.Month}";

		// Get the current day of the week (e.g., "Mandag", "Tirsdag")
		string currentDayOfWeek = currentDate.ToString("dddd", danishCulture);
		currentDayOfWeek = Char.ToUpper(currentDayOfWeek[0]) + currentDayOfWeek.Substring(1);

		// Update the TextMeshProUGUI with the week info and current day
		weekText.text = $"{currentDayOfWeek}, Uge {currentWeekNumber}\n{startDate} - {endDate}";
	}

	int GetWeekNumber(DateTime date)
	{
		// Get the week number based on ISO 8601 standard (Monday as the first day of the week)
		return System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	}

	DateTime GetStartOfWeek(DateTime date)
	{
		// Calculate the start date of the week (Monday)
		int diff = date.DayOfWeek - DayOfWeek.Monday;
		if (diff < 0)
			diff += 7;
		return date.AddDays(-diff).Date;
	}
}