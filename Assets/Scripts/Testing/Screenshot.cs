using UnityEngine;
using System.IO;
using System.Collections;

public class HighResUIScreenshot : MonoBehaviour
{
    public Camera uiCamera; // Assign in Inspector
    public int width = 3240;
    public int height = 6600;

    private void Start()
    {
        StartCoroutine(TakeScreenshotAfterDelay(5f));
    }

    private IEnumerator TakeScreenshotAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TakeScreenshot(width, height);
    }

    public void TakeScreenshot(int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        uiCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        uiCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        uiCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        string fileName = $"Screenshot_{width}x{height}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string filePath = Path.Combine(Application.dataPath, "..", fileName);
        File.WriteAllBytes(filePath, screenShot.EncodeToPNG());

        Debug.Log($"Screenshot saved to: {filePath}");
    }
}
