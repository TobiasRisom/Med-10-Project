using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScreenNavigation : MonoBehaviour
{
	private DataManager dm;
	private FirestoreHandler fish;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    dm = GameObject.FindWithTag("dataManager")
	                   .GetComponent<DataManager>();

	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    fish.ScheduleManager();
    }
    /*dm.spawnTasks();
    dm.setTasks();
    setListeners(dm.tasks);

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
    dm.currentTask = taskIndex;
    SceneManager.LoadScene("TaskScreen");
}*/
}
