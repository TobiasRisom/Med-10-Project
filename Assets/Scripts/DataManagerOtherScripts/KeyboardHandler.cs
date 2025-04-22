using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class KeyboardHandler : MonoBehaviour
{
    [Header("Tween Settings")]
    public float moveDuration = 0.25f;
    public Ease easing = Ease.OutCubic;

    private RectTransform panelHolder;
    private Vector2 originalAnchoredPosition;
    private bool hasStoredOriginalPosition = false;

    private TMP_InputField currentInput;
    private Tween currentTween;
    private float keyboardHeight = 0f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // persist across scenes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        DetectFocusedInputField();

        if (TouchScreenKeyboard.visible && currentInput != null)
        {
            float newHeight = GetKeyboardHeight();

            if (!Mathf.Approximately(newHeight, keyboardHeight))
            {
                keyboardHeight = newHeight;
                Debug.Log($"[KeyboardHandler] Keyboard height detected: {keyboardHeight}");
                RepositionUI();
            }
        }
        else if (keyboardHeight > 0f)
        {
            Debug.Log("[KeyboardHandler] Keyboard closed. Resetting UI.");
            keyboardHeight = 0f;
            ResetUIPosition();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject found = GameObject.FindWithTag("ScreenContentHolder");
        if (found != null)
        {
            panelHolder = found.GetComponent<RectTransform>();
            Debug.Log($"[KeyboardHandler] Found panelHolder: {panelHolder.name}");
            hasStoredOriginalPosition = false;
        }
        else
        {
            Debug.LogWarning("[KeyboardHandler] Could not find 'ScreenContentHolder' in scene.");
        }
    }

    private void DetectFocusedInputField()
    {
        if (currentInput != null && currentInput.isFocused)
            return;

        TMP_InputField[] fields = FindObjectsByType<TMP_InputField>(FindObjectsSortMode.None);
        foreach (var field in fields)
        {
            if (field.isActiveAndEnabled && field.isFocused)
            {
                currentInput = field;
                Debug.Log($"[KeyboardHandler] Detected focused input: {field.name}");
                return;
            }
        }

        currentInput = null;
    }

    private float GetKeyboardHeight()
    {
#if UNITY_IOS
        return TouchScreenKeyboard.area.height;
#elif UNITY_ANDROID
        float screenHeight = Screen.height;
        float safeBottom = Screen.safeArea.y;
        return screenHeight - (Screen.safeArea.height + safeBottom);
#else
        return 0f;
#endif
    }

    private void RepositionUI()
    {
        if (panelHolder == null || currentInput == null)
            return;

        if (!hasStoredOriginalPosition)
        {
            originalAnchoredPosition = panelHolder.anchoredPosition;
            hasStoredOriginalPosition = true;
        }

        Vector3 inputWorldPos = currentInput.transform.position;
        float inputY = inputWorldPos.y;

        float visibleHeight = Screen.height - keyboardHeight;
        float targetMid = visibleHeight / 2f;

        float offset = inputY - targetMid;

        Vector2 target = originalAnchoredPosition - new Vector2(0, offset);

        AnimateToPosition(target);
    }

    private void ResetUIPosition()
    {
        if (panelHolder != null && hasStoredOriginalPosition)
        {
            AnimateToPosition(originalAnchoredPosition);
        }

        currentInput = null;
        hasStoredOriginalPosition = false;
    }

    private void AnimateToPosition(Vector2 pos)
    {
        if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

        currentTween = panelHolder.DOAnchorPos(pos, moveDuration).SetEase(easing);
    }
}
