using System.Collections.Generic;
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
	
	string[] savedNames = { "", "", "", "", "", "", "" };

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    fish.ScheduleManager();
	    setUpUserButtons();
	    fish.GetTasksAwaitingVerification();
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
    
    void OnButtonClick(string userName)
    {
	    fish.currentUserInfo = userName;
	    SceneManager.LoadScene("ANSAT_UserOverviewScreen");
    }
}
