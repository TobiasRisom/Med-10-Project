using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ANSATUserOverviewNavigation : MonoBehaviour
{

	private FirestoreHandler fish;
	public VerticalLayoutGroup VLG;
	public GameObject taskStatusPrefab;

	public TextMeshProUGUI usersName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    ShowUserInfo(fish.currentUserInfo);
	    usersName.text = fish.currentUserInfo;
    }

    public void BackToMain()
    {
	    SceneManager.LoadScene("ANSAT_MainScreen");
    }

    public void ShowUserInfo(string userName)
    {
	    fish.GetTasksAndCount(userName, (taskCount, taskList) =>
	    {
		    foreach (var task in taskList)
		    {
			    GameObject userTaskInfo = Instantiate(taskStatusPrefab, VLG.transform);
			    userTaskInfo.transform.GetChild(1)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Titel;
			    userTaskInfo.transform.GetChild(2)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Emoji;
			    userTaskInfo.transform.GetChild(3)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Description;
			    userTaskInfo.transform.GetChild(4)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Status.ToString();
		    }
	    });
    }
}
