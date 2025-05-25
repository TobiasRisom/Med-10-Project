using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
	
	public GameObject leaderboardText;

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
	    
	    petName.text = PlayerPrefs.GetString("PetName");
	    setDollarsText();
        SetUpLeaderboard();
	    UpdateAndSpawnTasksAsync();
    }
    
    private async void UpdateAndSpawnTasksAsync()
    {
	    if (isItUpdateTime())
	    {
		    PlayerPrefs.SetInt("DaysActive", PlayerPrefs.GetInt("DaysActive") + 1);
		    await UpdateTasks();
	    }
	    
	    fish.spawnTasks(PlayerPrefs.GetString("Name"));
	    Invoke(nameof(StartCheckingForNoTasks), 1.5f);
    }
    
    void OnApplicationPause(bool isPaused)
    {
	    if (!isPaused)
	    {
		    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	    }
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
    
    private async Task UpdateTasks()
    {
	    await fish.UpdateDailyAndWeeklyTasks((((int)DateTime.Now.DayOfWeek + 6) % 7) + 2); // Monday = 2, Tuesday = 3, Wednesday = 4, Thursday = 5, Friday = 6, Saturday = 7, Sunday = 8
	        
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
		    TextMeshProUGUI weekText = weekNames[i];
		    string originalText = weekText.text;

		    if (Regex.IsMatch(originalText, pattern))
		    {
			    string boldedText = Regex.Replace(
				    originalText,
				    pattern,
				    $"<b><u>{userName}</u></b>"
			    );

			    weekText.text = boldedText;
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

	    GameObject.FindWithTag("PetStatus")
	              .GetComponent<TextMeshProUGUI>()
	              .text = inputPetName.text + " er glad for sit nye navn! \ud83d\udc95";

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
    
    private async void SetUpLeaderboard()
    {
	    Dictionary<string, string> leaderboard = await fish.GetLeaderboard();

	    leaderboardText.transform.GetChild(6)
	                   .GetComponent<TextMeshProUGUI>()
	                   .text = "";
	    leaderboardText.transform.GetChild(7)
	                   .GetComponent<TextMeshProUGUI>()
	                   .text = "";

	    for (int i = 0; i < leaderboard.Count; i++)
	    {
		    if (i == 0)
		    {
			    leaderboardText.transform.GetChild(0)
			                   .GetComponent<TextMeshProUGUI>()
			                   .text = leaderboard.ElementAt(i)
			                                      .Key;
			    leaderboardText.transform.GetChild(3)
			                   .GetComponent<TextMeshProUGUI>()
			                   .text = leaderboard.ElementAt(i)
			                                      .Value;
			    continue;
		    }
		    
		    if (i == 1)
		    {
			    leaderboardText.transform.GetChild(1)
			                   .GetComponent<TextMeshProUGUI>()
			                   .text = leaderboard.ElementAt(i)
			                                      .Key;
			    leaderboardText.transform.GetChild(4)
			                   .GetComponent<TextMeshProUGUI>()
			                   .text = leaderboard.ElementAt(i)
			                                      .Value;
			    continue;
		    }
		    
		    if (i == 2)
		    {
			    leaderboardText.transform.GetChild(2)
			                   .GetComponent<TextMeshProUGUI>()
			                   .text = leaderboard.ElementAt(i)
			                                      .Key;
			    leaderboardText.transform.GetChild(5)
			                   .GetComponent<TextMeshProUGUI>()
			                   .text = leaderboard.ElementAt(i)
			                                      .Value;
			    continue;
		    }
		    
		    leaderboardText.transform.GetChild(6)
		                   .GetComponent<TextMeshProUGUI>()
		                   .text += leaderboard.ElementAt(i)
		                                      .Key + "\n";
		    
		    leaderboardText.transform.GetChild(7)
		                   .GetComponent<TextMeshProUGUI>()
		                   .text += leaderboard.ElementAt(i)
		                                      .Value + "\n";
	    }
	    
	    foreach (var entry in leaderboard)
	    {
		    Debug.Log(entry.Key);
		    Debug.Log(entry.Value);
	    }
    }
}
