using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenNavigation : MonoBehaviour
{
	private DataManager dm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    dm = GameObject.FindWithTag("dataManager")
	                   .GetComponent<DataManager>();
	    
	    dm.spawnTasks();
	    dm.setTasks();
	    setListeners(dm.tasks);
    }

    private void setListeners(List<GameObject> tasks) // fix this
    {
	    for (int i = 0; i < tasks.Count; i++)
	    {
		    Button btn = tasks[i]
			    .GetComponent<Button>();

		    btn.onClick.AddListener(goToTask);
	    }
    }

    public void goToTask()
    {
	    Debug.Log("debug");
    }
}
