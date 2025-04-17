using TMPro;
using UnityEngine;

public class SetNameFromPlayerPrefs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    GetComponent<TextMeshProUGUI>()
	        .text = PlayerPrefs.GetString("Name");
    }
}
