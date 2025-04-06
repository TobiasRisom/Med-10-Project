using System.Net.Mime;
using TMPro;
using UnityEngine;

public class TaskScreenNavigation : MonoBehaviour
{
	private DataManager dm;
	
	public TextMeshProUGUI taskTitle;
	public TextMeshProUGUI taskEmoji;
	public TextMeshProUGUI taskDescription;
    void Start()
    {
	    dm = GameObject.FindWithTag("dataManager")
	                   .GetComponent<DataManager>();

	    int taskIndex = dm.currentTask;

	    taskTitle.text = dm.taskData[taskIndex, 0];
	    taskEmoji.text = dm.taskData[taskIndex, 1];
	    taskDescription.text = dm.taskData[taskIndex, 2];

	    if (dm.taskData[taskIndex, 3] == "P")
	    {
		    transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
	    }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
