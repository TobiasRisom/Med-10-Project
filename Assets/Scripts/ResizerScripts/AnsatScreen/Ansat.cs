using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ansat : MonoBehaviour
{
    private const int minHeight = 1920;
    private const int maxHeight = 2412;

    public RectTransform viewport;
    public RectTransform stats;

    // Min/Max values
    [Header("Viewport Height")]
    public float viewportMinHeight;
    public float viewportMaxHeight;

    [Header("Stats Y Position")]
    public float statsMinY;
    public float statsMaxY;

    [Header("Stats Spacing")]
    public float statsSpacingMin;
    public float statsSpacingMax;

    [Header("Stats Font Size")]
    public float fontSizeMin;
    public float fontSizeMax;

    void Start()
    {
        float screenHeight = GameObject.FindWithTag("MainCanvas")
                                       .GetComponent<RectTransform>()
                                       .rect.height;

        float t = Mathf.InverseLerp(minHeight, maxHeight, screenHeight);

        // Calculate lerped values
        float viewportHeight = Mathf.Lerp(viewportMinHeight, viewportMaxHeight, t);
        float statsY = Mathf.Lerp(statsMinY, statsMaxY, t);
        float statsSpacing = Mathf.Lerp(statsSpacingMin, statsSpacingMax, t);
        float statsFontSize = Mathf.Lerp(fontSizeMin, fontSizeMax, t);

        // Apply viewport height
        if (viewport != null)
        {
            Vector2 size = viewport.sizeDelta;
            size.y = viewportHeight;
            viewport.sizeDelta = size;
        }

        // Apply stats position
        if (stats != null)
        {
            Vector3 pos = stats.anchoredPosition;
            pos.y = statsY;
            stats.anchoredPosition = pos;

            // Apply spacing (VerticalLayoutGroup must be on same GameObject)
            VerticalLayoutGroup layout = stats.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.spacing = statsSpacing;
            }

            // Apply font size to all children
            for (int i = 0; i < stats.childCount; i++)
            {
                TextMeshProUGUI text = stats.GetChild(i).GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    text.fontSize = statsFontSize;
                }
            }
        }
    }
}
