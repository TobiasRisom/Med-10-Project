using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ANSATUserOverviewNavigation : MonoBehaviour
{

	private FirestoreHandler fish;
	public GameObject content;
	public GameObject taskStatusPrefab;
	public GameObject deleteWindow;
	public GameObject taskToDelete;

	public TextMeshProUGUI usersName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    ShowUserInfo(fish.currentUserInfo);
	    usersName.text = fish.currentUserInfo;
	    fish.GetUserStats(fish.currentUserInfo);
    }

    public void BackToMain()
    {
	    SceneManager.LoadScene("ANSAT_MainScreen");
    }

    public void ShowUserInfo(string userName)
    {
	    fish.GetTasksAndCount(userName, true, (taskCount, taskList) =>
	    {
		    foreach (var t in taskList)
		    {
			    FirestoreHandler.Task task = t.TaskData;
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
				    case 3:
					    userTaskInfo.transform.GetChild(6).gameObject.SetActive(true);
					    userTaskInfo.transform.GetChild(4).gameObject.SetActive(false);
					    userTaskInfo.transform.GetChild(5).gameObject.SetActive(false);
					    break;
			    }
			    
			    userTaskInfo.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = task.ImageFormat ? "\ud83d\udcf8" : "\ud83d\udcdd";

			    userTaskInfo.transform.GetChild(9)
			                .GetComponent<Button>()
			                .onClick.AddListener(() => {DeleteWindow(userTaskInfo.gameObject); });
			    
			    ch.AddItem(userTaskInfo);
		    }
	    });
    }

    public void DeleteWindow(GameObject parent)
    {
	    Debug.Log("delete window opened");
	    deleteWindow.SetActive(true);
	    taskToDelete = Instantiate(parent, deleteWindow.transform.GetChild(0));
	    taskToDelete.transform.localPosition = Vector3.zero;
	    taskToDelete.transform.GetChild(9)
	                .GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void DeleteWindowClose()
    {
	    deleteWindow.SetActive(false);
	    Destroy(taskToDelete);
    }

    public void ClearTask()
    {
	    foreach (Transform child in content.transform)
	    {
		    ContentHandler ch = content.GetComponent<ContentHandler>();
		    ch.RemoveItem(child.gameObject);
		    Destroy(child.gameObject);
	    }
    }
    
    public void DeleteForUserWrapper()
    {
	    _ = DeleteForUser(); // fire and forget
    }

    public void DeleteForAllWrapper()
    {
	    _ = DeleteForAll();
    }

    public async Task DeleteForUser()
    {
	    await fish.DeleteTaskForUser(taskToDelete.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text, fish.currentUserInfo);
	    ShowUserInfo(fish.currentUserInfo);
	    fish.GetUserStats(fish.currentUserInfo);
	    DeleteWindowClose();
    }
    
    public async Task DeleteForAll()
    {
	    await fish.DeleteTaskForAllUsers(taskToDelete.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
	    ShowUserInfo(fish.currentUserInfo);
	    fish.GetUserStats(fish.currentUserInfo);
	    DeleteWindowClose();
    }
}
