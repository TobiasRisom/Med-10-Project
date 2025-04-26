using UnityEngine;

public class AllTasksDone : MonoBehaviour
{
	public GameObject taskContent;
    // Update is called once per frame
    void Update()
    {
	    if (taskContent.transform.childCount == 0)
	    {
		    gameObject.SetActive(true);
	    }
	    else
	    {
		    gameObject.SetActive(false);
	    }
    }
}
