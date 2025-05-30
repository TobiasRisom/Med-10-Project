using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ContentHandler : MonoBehaviour
{
	public float spacing = 30f;
	public float animationDuration = 0.3f;
	public List<RectTransform> elements = new List<RectTransform>();

	public void AddItem(GameObject item)
	{
		RectTransform rt = item.GetComponent<RectTransform>();
		item.transform.SetParent(transform, false);
		elements.Add(rt);

		LayoutItems();
	}

	public void RemoveItem(int index)
{
	if (index >= 0 && index < elements.Count)
	{
		RectTransform rt = elements[index];
		elements.RemoveAt(index);
		Destroy(rt.gameObject);
		LayoutItems();
	}
	else
	{
		Debug.LogWarning($"RemoveItem: Index {index} is out of range.");
	}
}
	
public void RemoveItem(GameObject taskObject)
{
	// Find the RectTransform of the task GameObject within elements
	RectTransform rt = taskObject.GetComponent<RectTransform>();

	// If it's found in the list, remove it
	if (elements.Contains(rt))
	{
		elements.Remove(rt); // Remove from the list of elements
		Destroy(taskObject); // Destroy the GameObject itself
		LayoutItems(); // Re-layout the remaining items
	}
	else
	{
		Debug.LogWarning($"RemoveItem: GameObject {taskObject.name} is not found in elements.");
	}
}

	void LayoutItems()
	{
		float yOffset = 140f;
		float contentHeight = 0f;

		for (int i = 0; i < elements.Count; i++)
		{
			RectTransform rt = elements[i];
			rt.DOKill();

			float delay = i * 0.03f;

			Vector3 targetLocalPos = new Vector3(0f, -yOffset, 0f);
			rt.DOLocalMove(targetLocalPos, animationDuration)
			  .SetEase(Ease.OutCubic)
			  .SetDelay(delay);

			yOffset += rt.sizeDelta.y + spacing;
			contentHeight += rt.sizeDelta.y + spacing;
		}

		// Resize content area
		RectTransform contentRT = GetComponent<RectTransform>();
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, contentHeight);
	}
}