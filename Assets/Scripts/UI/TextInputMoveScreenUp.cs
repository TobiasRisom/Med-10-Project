using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Make sure DOTween is installed

public class TextInputMoveScreenUp : MonoBehaviour
{
    public RectTransform uiPanel;    // The UI Panel or container to move (assign in the inspector)
    public float slideAmount = 200f; // Amount to slide the UI up
    public float slideDuration = 0.5f; // Duration of the slide animation
    private InputField inputField;   // The InputField to detect focus and keyboard visibility
    private bool isKeyboardVisible = false;  // Track if keyboard is visible

    private void Start()
    {
        inputField = GetComponent<InputField>();  // Get the InputField component on the same GameObject
    }

    void Update()
    {
        // Check if the virtual keyboard is visible and the input field is focused
        if (TouchScreenKeyboard.visible && inputField.isFocused)
        {
            if (!isKeyboardVisible)
            {
                // Keyboard just became visible
                isKeyboardVisible = true;
                OnKeyboardVisible();  // Call method to handle actions when the keyboard appears
            }
        }
        else
        {
            if (isKeyboardVisible)
            {
                // Keyboard just became hidden
                isKeyboardVisible = false;
                OnKeyboardHidden();  // Call method to handle actions when the keyboard disappears
            }
        }
    }

    // Method called when the keyboard becomes visible
    private void OnKeyboardVisible()
    {
        if (uiPanel != null)
        {
            // Animate the UI panel to move up and make room for the keyboard
            uiPanel.DOLocalMoveY(slideAmount, slideDuration).SetEase(Ease.OutCubic);
        }
        else
        {
            Debug.LogWarning("UI Panel not assigned. Please assign a RectTransform to 'uiPanel'.");
        }
    }

    // Method called when the keyboard becomes hidden
    private void OnKeyboardHidden()
    {
        if (uiPanel != null)
        {
            // Animate the UI panel to move back down to its original position
            uiPanel.DOLocalMoveY(0, slideDuration).SetEase(Ease.OutCubic);
        }
        else
        {
            Debug.LogWarning("UI Panel not assigned. Please assign a RectTransform to 'uiPanel'.");
        }
    }
}
