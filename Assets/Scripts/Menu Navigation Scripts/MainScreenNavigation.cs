using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScreenNavigation : MonoBehaviour
{
	private FirestoreHandler fish;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    fish.ScheduleManager();
	    //fish.spawnTasks(fish.username);
	    fish.spawnTasks("Leif");
    }

	private void setListeners(List<GameObject> tasks)
	{
	    for (int i = 0; i < tasks.Count; i++)
	    {
		    Button btn = tasks[i]
			    .GetComponent<Button>();

		    int index = i;
		    btn.onClick.AddListener(delegate { goToTask(index); });
	    }
	}

	public void goToTask(int taskIndex)
	{
	    fish.currentTask = taskIndex;
	    SceneManager.LoadScene("TaskScreen");
	}
}
