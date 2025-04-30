using UnityEngine;
using TMPro;
using DG.Tweening;

public class MoneyAnimation : MonoBehaviour
{
	public TextMeshProUGUI text;
	private float moveUpAmount = 50f;
	private float duration = 1f;

	public void Play(Vector3 worldPosition, Transform canvas)
	{

		// Convert world position to local canvas space
		Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvas.GetComponent<RectTransform>(), screenPos, Camera.main, out localPoint);

		RectTransform rt = GetComponent<RectTransform>();
		rt.SetParent(canvas, false);
		rt.anchoredPosition = localPoint;

		text.alpha = 1f;

		// Animate
		Sequence seq = DOTween.Sequence();
		seq.Append(rt.DOAnchorPosY(localPoint.y + moveUpAmount, duration));
		seq.Join(text.DOFade(0f, duration));
		seq.OnComplete(() => Destroy(gameObject));
	}
}