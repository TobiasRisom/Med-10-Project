using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenNavigation : MonoBehaviour
{
	private FirestoreHandler fish;

	private int pos = -1080;

	private float tweenSpeed = 0.5f;
	private Ease tweenEase = Ease.OutQuad;

	public List<TextMeshProUGUI> weekNames;
	public List<Image> weekStripes;

	public TextMeshProUGUI petName;
	public TMP_InputField inputPetName;
	public Button petEditName;
	public Button petNameAccept;
	public Button petNameReject;

	public GameObject noTasksText;
	private bool buffer;

	[SerializeField]
	private Color darkStripe;
	[SerializeField]
	private Color lightStripe;

	[SerializeField]
	private TextMeshProUGUI ØGDollarText1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    fish.ScheduleManager();
	    fish.spawnTasks(PlayerPrefs.GetString("Name"));
	    
	    if (isItUpdateTime())
	    {
		    UpdateTasks();
	    }

	    petName.text = PlayerPrefs.GetString("PetName");
	    setDollarsText();
	    
	    Invoke(nameof(StartCheckingForNoTasks), 1f);
    }

    void Update()
    {
	    ContentHandler ch = GameObject.FindWithTag("content")
	                                  .GetComponent<ContentHandler>();
	    if (ch.elements.Count == 0 && buffer)
	    {
		    noTasksText.SetActive(true);
	    }
	    else
	    {
		    noTasksText.SetActive(false);
	    }
    }

    void StartCheckingForNoTasks()
    {
	    buffer = true;
    }

    public void setDollarsText()
    {
	    ØGDollarText1.text = PlayerPrefs.GetInt("Dollars") + " \ud83d\udcb5";
    }
    
    public void changeWindow(int windowIndex)
    {
	    float targetX = pos * windowIndex;
	    transform.DOLocalMoveX(targetX, tweenSpeed).SetEase(tweenEase);
    }
    
    private Boolean isItUpdateTime()
    {
	    DateTime now = DateTime.Now;
	    string updateTime = PlayerPrefs.GetString("UpdateTime");
	    
	    if (DateTime.TryParse(updateTime, out DateTime storedTime))
	    {
		    if (now >= storedTime)
		    {
			    // Update Time!
			    return true;
		    }
	    }
	    return false;
    }

    public void AccessedSchedule()
    {
	    fish.UpdateStats(PlayerPrefs.GetString("Name"),"ScheduleAccessedAmount", 1);
    }
    
    private void UpdateTasks()
    {
	    fish.UpdateDailyAndWeeklyTasks((((int)DateTime.Now.DayOfWeek + 6) % 7) + 2); // Monday = 2, Tuesday = 3, Wednesday = 4, Thursday = 5, Friday = 6, Saturday = 7, Sunday = 8
	        
	    // Change next update date to tomorrow
	    DateTime tomorrow = DateTime.Now.Date.AddDays(1);
	    DateTime nextUpdate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 0, 0);
	    PlayerPrefs.SetString("UpdateTime", nextUpdate.ToString("yyyy-MM-dd"));
    }
    

    public void highlightUserInSchedule()
    {
	    string userName = PlayerPrefs.GetString("Name");
	    string pattern = $@"\b{Regex.Escape(userName)}\b";

	    for (int i = 0; i < 7; i++)
	    {
		    TextMeshProUGUI weekText = weekNames[i]; // Or TextMeshProUGUI
		    string originalText = weekText.text;

		    if (Regex.IsMatch(originalText, pattern))
		    {
			    // Bold only the exact word match
			    string boldedText = Regex.Replace(
				    originalText,
				    pattern,
				    $"<b><u>{userName}</u></b>"
			    );

			    weekText.text = boldedText;

			    // Highlight background
			    //weekStripes[i].color = new Color(1f, 0.447f, 0.125f, 1f);
		    }
		    else
		    {
			    // Reset background stripe
			    //weekStripes[i].color = (i % 2 == 1) ? darkStripe : lightStripe;
		    }
	    }
    }


    public void editPetName()
    {
	    inputPetName.gameObject.SetActive(true);
	    petEditName.gameObject.SetActive(false);
	    petNameAccept.gameObject.SetActive(true);
	    petNameReject.gameObject.SetActive(true);
	    inputPetName.text = PlayerPrefs.GetString("PetName");
    }

    public void acceptPetName()
    {
	    PlayerPrefs.SetString("PetName", inputPetName.text);
	    petName.text = inputPetName.text;
	    
	    inputPetName.gameObject.SetActive(false);
	    petEditName.gameObject.SetActive(true);
	    petNameAccept.gameObject.SetActive(false);
	    petNameReject.gameObject.SetActive(false);
    }

    public void rejectPetName()
    {
	    inputPetName.gameObject.SetActive(false);
	    petEditName.gameObject.SetActive(true);
	    petNameAccept.gameObject.SetActive(false);
	    petNameReject.gameObject.SetActive(false);
    }

    public void closeShop()
    {
	    GameObject shopPanel = GameObject.FindWithTag("BuyWindow");

	    if (shopPanel != null)
	    {
		    shopPanel.SetActive(false);   
	    }
    }
}
