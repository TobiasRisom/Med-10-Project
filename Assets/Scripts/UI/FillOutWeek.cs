using System.Linq;
using TMPro;
using UnityEngine;

public class FillOutWeek : MonoBehaviour
{
    private DataManager dm;
    public TextMeshProUGUI[] textFields;

    void Start()
    {
	    dm = GameObject.FindWithTag("dataManager")
	                   .GetComponent<DataManager>();

	    for (int i = 0; i < textFields.Length; i++)
	    {
		    textFields[i].text = dm.weekNames[i];
	    }
    }
}
