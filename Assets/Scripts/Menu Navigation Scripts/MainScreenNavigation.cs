using UnityEngine;

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
    }
}
