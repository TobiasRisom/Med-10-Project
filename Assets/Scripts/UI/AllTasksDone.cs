using UnityEngine;

public class AllTasksDone : MonoBehaviour
{
	public GameObject taskContent;

	private bool buffer = false;
    // Update is called once per frame

    void Start()
    {
	    Invoke(nameof(StartChecking), 1.0f);
    }
    void Update()
    {
	    if (taskContent.transform.childCount == 0 && buffer)
	    {
		    gameObject.SetActive(true);
	    }
	    else
	    {
		    gameObject.SetActive(false);
	    }
    }

    void StartChecking()
    {
	    buffer = true;
    }
}
