using System.Collections.Generic;
using UnityEngine;

public class PetAnimation : MonoBehaviour
{
	public List<string> animStateNames = new List<string>(); // List of animation state names, not clips!

	private Animator animator;

	void Start()
	{
		animator = GetComponent<Animator>();

		int petIndex = PlayerPrefs.GetInt("Pet", 0);

		if (petIndex >= 0 && petIndex < animStateNames.Count)
		{
			animator.Play(animStateNames[petIndex]);
		}
		else
		{
			Debug.LogWarning("Invalid Pet Index for animation!");
		}
	}

	public void ChangeAnimation(int newPetIndex)
	{
		if (newPetIndex >= 0 && newPetIndex < animStateNames.Count)
		{
			animator.Play(animStateNames[newPetIndex]);
		}
		else
		{
			Debug.LogWarning("Invalid Pet Index for animation!");
		}
	}
}