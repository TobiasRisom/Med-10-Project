using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ANSATUserOverviewNavigation : MonoBehaviour
{

	private FirestoreHandler fish;
	public GameObject content;
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
	    fish.GetTasksAndCount(userName, (taskCount, taskList, document) =>
	    {
		    foreach (var task in taskList)
		    {
			    ContentHandler ch = content.GetComponent<ContentHandler>();
			    GameObject userTaskInfo = Instantiate(taskStatusPrefab, content.transform, false);
			    userTaskInfo.transform.GetChild(1)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Titel;
			    userTaskInfo.transform.GetChild(2)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Emoji;
			    userTaskInfo.transform.GetChild(3)
			                .GetComponent<TextMeshProUGUI>()
			                .text = task.Description;
			    
			    switch (task.Status)
			    {
				    case 0:
					    userTaskInfo.transform.GetChild(4).gameObject.SetActive(true);
					    userTaskInfo.transform.GetChild(5).gameObject.SetActive(false);
					    userTaskInfo.transform.GetChild(6).gameObject.SetActive(false);
					    break;
				    case 1:
					    userTaskInfo.transform.GetChild(5).gameObject.SetActive(true);
					    userTaskInfo.transform.GetChild(4).gameObject.SetActive(false);
					    userTaskInfo.transform.GetChild(6).gameObject.SetActive(false);
					    break;
				    case 2:
					    userTaskInfo.transform.GetChild(6).gameObject.SetActive(true);
					    userTaskInfo.transform.GetChild(4).gameObject.SetActive(false);
					    userTaskInfo.transform.GetChild(5).gameObject.SetActive(false);
					    break;
			    }
			    
			    userTaskInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = task.ImageFormat ? "\ud83d\udcf8" : "\ud83d\udcdd";
			    
			    ch.AddItem(userTaskInfo);
		    }
	    });
    }
}
