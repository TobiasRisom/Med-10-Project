using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldLineLimiter : MonoBehaviour
{
	[Tooltip("Maximum number of allowed lines.")]
	public int maxLines = 5;

	private TMP_InputField inputField;

	private void Awake()
	{
		inputField = GetComponent<TMP_InputField>();
		inputField.onValueChanged.AddListener(OnTextChanged);
	}

	private void OnDestroy()
	{
		inputField.onValueChanged.RemoveListener(OnTextChanged);
	}

	private void OnTextChanged(string text)
	{
		string[] lines = text.Split('\n');

		if (lines.Length > maxLines)
		{
			string trimmedText = string.Join("\n", lines, 0, maxLines);
			inputField.text = trimmedText;

			// Move the caret to the end
			inputField.caretPosition = trimmedText.Length;
		}
	}
}