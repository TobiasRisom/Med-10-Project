using System.Data.Common;
using TMPro;
using UnityEngine;

public class SetName : MonoBehaviour
{
	private DataManager dm;

	void Start()
	{
		dm = GameObject.FindWithTag("dataManager")
		               .GetComponent<DataManager>();
		GetComponent<TextMeshProUGUI>()
		    .text = dm.userName;
	}
}
