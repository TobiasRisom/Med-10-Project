using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
	public List<AnimatorController> animCon = new List<AnimatorController>();
	
    void Start()
    {
	    GetComponent<Animator>()
		    .runtimeAnimatorController = animCon[PlayerPrefs.GetInt("Pet")];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
