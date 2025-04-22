using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ANSATMainScreenNavigation : MonoBehaviour
{
	private FirestoreHandler fish;

	public Button edit;
	public Button acceptEdit;
	public Button undoEdit;

	public GameObject taskToVerify;

	public GameObject nameParent;
	public GameObject textFieldParent;

	public RectTransform userHolder;
	public GameObject userButtonPrefab;

	public GameObject leaderboardText;

	public GameObject noMoreTasks;
	private GameObject content;
	
	string[] savedNames = { "", "", "", "", "", "", "" };
	
	private int pos = -1080;

	private float tweenSpeed = 0.5f;
	private Ease tweenEase = Ease.OutQuad;

	public GameObject ZoomInPanel;

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();

	    content = GameObject.FindWithTag("content");
	    
	    fish.ScheduleManager();
	    setUpUserButtons();
	    fish.spawnVerifiedTasks();
	    SetUpLeaderboard();
    }

    void Update()
    {
	    checkForNoMoreTasks();
    }

    public void ZoomOut()
    {
	    ZoomInPanel.SetActive(false);
    }

    public void UpdateEverything()
    {
	    fish.ScheduleManager();
	    setUpUserButtons();
	    fish.spawnVerifiedTasks();
	    SetUpLeaderboard();
    }

    public void ScheduleEditMode()
    {
	    for (int i = 0; i < 7; i++)
	    {
		    string existingName = nameParent.transform.GetChild(i)
		                            .GetComponent<TextMeshProUGUI>()
		                            .text;
		    textFieldParent.transform.GetChild(i)
		                   .GetComponent<TMP_InputField>()
		                   .text = existingName;

		    savedNames[i] = existingName;
	    }
	    textFieldParent.SetActive(true);
	    nameParent.SetActive(false);
	    
	    edit.gameObject.SetActive(false);
	    acceptEdit.gameObject.SetActive(true);
	    undoEdit.gameObject.SetActive(true);
    }

    public void UndoScheduleEdit()
    {
	    for (int i = 0; i < 7; i++)
	    {
		    nameParent.transform.GetChild(i)
		              .GetComponent<TextMeshProUGUI>()
		              .text = savedNames[i];
	    }
	    
	    textFieldParent.SetActive(false);
	    nameParent.SetActive(true);
	    
	    edit.gameObject.SetActive(true);
	    acceptEdit.gameObject.SetActive(false);
	    undoEdit.gameObject.SetActive(false);
    }

    public void AcceptScheduleEdit()
    {
	    Dictionary<string, object> newNames = new Dictionary<string, object>();
	    string[] ugedage = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };
	    
	    for (int i = 0; i < 7; i++)
	    {
		    string newName = textFieldParent.transform.GetChild(i)
		                             .GetComponent<TMP_InputField>()
		                             .text;
		    nameParent.transform.GetChild(i)
		              .GetComponent<TextMeshProUGUI>()
		              .text = newName;

		    newNames[ugedage[i]] = newName;
	    }
	    
	    fish.UpdateSchedule(newNames);
	    
	    textFieldParent.SetActive(false);
	    nameParent.SetActive(true);
	    
	    edit.gameObject.SetActive(true);
	    acceptEdit.gameObject.SetActive(false);
	    undoEdit.gameObject.SetActive(false);
    }

    public void goToTaskCreationScreen()
    {
	    SceneManager.LoadScene("ANSAT_TaskCreationScreen");
    }

    private async void setUpUserButtons()
    {
	    List<string> userList = await fish.GetUsers();

	    if (userList.Count == 0)
	    {
		    Debug.Log("No users found.");
		    return;
	    }

	    GridLayoutGroup glg = userHolder.GetComponent<GridLayoutGroup>();

	    foreach (string user in userList)
	    {
		    GameObject userButton = Instantiate(userButtonPrefab, glg.transform);
		    userButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = user;

		    Button btn = userButton.GetComponentInChildren<Button>();
		    
		    btn.onClick.AddListener(delegate { OnButtonClick(user); });
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
    
    void OnButtonClick(string userName)
    {
	    fish.currentUserInfo = userName;
	    SceneManager.LoadScene("ANSAT_UserOverviewScreen");
    }
    
    public void changeWindow(int windowIndex)
    {
	    float targetX = pos * windowIndex;
	    transform.DOLocalMoveX(targetX, tweenSpeed).SetEase(tweenEase);
    }

    private void checkForNoMoreTasks()
    {
	    noMoreTasks.SetActive(content.transform.childCount == 0);
    }
}
