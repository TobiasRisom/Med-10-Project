using UnityEngine;
using UnityEngine.UI;

public class HeartsScript : MonoBehaviour
{
	private Toggle toggle;

	public GameObject heart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    toggle = GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
	    if (toggle.isOn)
	    {
		    heart.SetActive(true);
	    }
	    else
	    {
		    heart.SetActive(false);
	    }
    }
}
