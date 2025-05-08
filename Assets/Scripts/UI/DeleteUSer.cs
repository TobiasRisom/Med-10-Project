using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteUSer : MonoBehaviour
{
	public GameObject harBruger;
	public Button deleteUser;
	public GameObject container;
	public TMP_InputField input;
	private int clicks = 0;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
	    if (harBruger.activeSelf == true)
	    {
		    deleteUser.gameObject.SetActive(true);
	    }
	    else
	    {
		    deleteUser.gameObject.SetActive(false);
	    }
	    
	    if (clicks > 9)
	    {
		    container.SetActive(true);
		    clicks = 0;
	    }
    }

    public void deleteClickedOnce()
    {
	    clicks++;
    }

    public void Cancel()
    {
	    container.SetActive(false);
	    input.text = "";
    }

    public void DeleteUser()
    {
	    int userCode = int.Parse(input.text);


	    if (userCode != 9827)
	    {
		    return;
	    }

	    PlayerPrefs.SetString("Name", "NoN");
	    container.transform.GetChild(0).gameObject.SetActive(false);
	    container.transform.GetChild(1).gameObject.SetActive(false);
	    container.transform.GetChild(2).gameObject.SetActive(true);
	    container.transform.GetChild(3).gameObject.SetActive(false);
	    container.transform.GetChild(4).gameObject.SetActive(false);
	    container.transform.GetChild(5).gameObject.SetActive(false);
    }
}
