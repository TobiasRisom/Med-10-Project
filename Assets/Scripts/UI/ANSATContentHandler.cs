using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ANSATContentHandler : MonoBehaviour
{
	public float spacing = 30f;
	public float animationDuration = 0.3f;
	private List<RectTransform> elements = new List<RectTransform>();

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

	void LayoutItems()
	{
		float h = GetComponent<RectTransform>()
		                   .rect.height;
		float yOffset = -(h / 2) + 125;

		for (int i = 0; i < elements.Count; i++)
		{
			RectTransform rt = elements[i];
			rt.DOKill(); // Stop ongoing tweens

			// Animate top to bottom using delay
			float delay = i * 0.03f; // tweak this for faster/slower cascade

			Vector2 targetPos = new Vector2(rt.anchoredPosition.x, -yOffset);
			rt.DOAnchorPos(targetPos, animationDuration)
			  .SetEase(Ease.OutCubic)
			  .SetDelay(delay);

			yOffset += rt.sizeDelta.y + spacing;
		}
	}
}