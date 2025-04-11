using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ANSATMainScreenNavigation : MonoBehaviour
{
	private FirestoreHandler fish;

	public Button edit;
	public Button acceptEdit;
	public Button undoEdit;

	public GameObject nameParent;
	public GameObject textFieldParent;
	
	string[] savedNames = { "", "", "", "", "", "", "" };

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();
	    
	    fish.ScheduleManager();
    }

    public void ScheduleEditMode()
    {
	    for (int i = 0; i < 7; i++)
	    {
		    string existingName = nameParent.transform.GetChild(i)
		                            .GetComponent<TextMeshProUGUI>()
		                            .text;
		    textFieldParent.transform.GetChild(i)
		                   .GetComponent<TMP_InputField>()
		                   .text = existingName;

		    savedNames[i] = existingName;
	    }
	    textFieldParent.SetActive(true);
	    nameParent.SetActive(false);
	    
	    edit.gameObject.SetActive(false);
	    acceptEdit.gameObject.SetActive(true);
	    undoEdit.gameObject.SetActive(true);
    }

    public void UndoScheduleEdit()
    {
	    for (int i = 0; i < 7; i++)
	    {
		    nameParent.transform.GetChild(i)
		              .GetComponent<TextMeshProUGUI>()
		              .text = savedNames[i];
	    }
	    
	    textFieldParent.SetActive(false);
	    nameParent.SetActive(true);
	    
	    edit.gameObject.SetActive(true);
	    acceptEdit.gameObject.SetActive(false);
	    undoEdit.gameObject.SetActive(false);
    }

    public void AcceptScheduleEdit()
    {
	    Dictionary<string, object> newNames = new Dictionary<string, object>();
	    string[] ugedage = { "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag", "Søndag" };
	    
	    for (int i = 0; i < 7; i++)
	    {
		    string newName = textFieldParent.transform.GetChild(i)
		                             .GetComponent<TMP_InputField>()
		                             .text;
		    nameParent.transform.GetChild(i)
		              .GetComponent<TextMeshProUGUI>()
		              .text = newName;

		    newNames[ugedage[i]] = newName;
	    }
	    
	    fish.UpdateSchedule(newNames);
	    
	    textFieldParent.SetActive(false);
	    nameParent.SetActive(true);
	    
	    edit.gameObject.SetActive(true);
	    acceptEdit.gameObject.SetActive(false);
	    undoEdit.gameObject.SetActive(false);
    }

    public void goToTaskCreationScreen()
    {
	    SceneManager.LoadScene("ANSAT_TaskCreationScreen");
    }
}
