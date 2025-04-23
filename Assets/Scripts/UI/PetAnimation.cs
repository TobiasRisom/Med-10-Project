using System.Collections.Generic;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
	public List<Animator> animCon = new List<Animator>();
	
    void Start()
    {
	    GetComponent<Animator>()
		    .runtimeAnimatorController = animCon[PlayerPrefs.GetInt("Pet")].runtimeAnimatorController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
