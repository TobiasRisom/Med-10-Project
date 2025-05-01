using UnityEngine;
using UnityEngine.UI;
using System.Collections; // <- Added for Coroutine

public class CameraDisplay : MonoBehaviour
{
    public RawImage cameraFeed;  // For live camera feed
    public RawImage capturedImage;  // For captured photo
    public Button captureButton;
    public Button confirmButton;
    public Button retakeButton;

    private WebCamTexture webCamTexture;
    public Texture2D photo;

    private FirestoreHandler fish;

    void Start()
    {
	    fish = GameObject.FindWithTag("dataManager")
	                     .GetComponent<FirestoreHandler>();

	    // Use currentTask directly
	    if (fish.currentTask.ImageFormat)
	    {
		    SetupCamera();
	    }
    }

    void SetupCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogWarning("No camera detected.");
            return;
        }

        string selectedCam = devices[0].name;

#if !UNITY_EDITOR
        foreach (var device in devices)
        {
            if (!device.isFrontFacing)  // Prefer back camera
            {
                selectedCam = device.name;
                break;
            }
        }
#endif

        Debug.Log("Selected camera: " + selectedCam);

        // Set a larger WebCamTexture resolution (e.g., 1600x1200 or higher)
        webCamTexture = new WebCamTexture(selectedCam, 1600, 1200);  // Larger resolution
        cameraFeed.texture = webCamTexture;

        if (webCamTexture != null)
        {
            webCamTexture.Play();
            StartCoroutine(AdjustAspectAfterStart()); // <-- Adjust aspect to fit the mask

            captureButton.onClick.AddListener(CapturePhoto);
            retakeButton.onClick.AddListener(RetakePhoto);
        }
        else
        {
            Debug.LogError("Failed to initialize WebCamTexture.");
        }
    }

    private IEnumerator AdjustAspectAfterStart()
    {
        // Wait for the camera to initialize
        while (webCamTexture.width < 100)
        {
            yield return null;
        }

        // Calculate the aspect ratio of the webcam feed
        float camAspect = (float)webCamTexture.width / webCamTexture.height;

        // Adjust the RectTransform of the camera feed to fit the 800x1200 visible area within the mask
        RectTransform rt = cameraFeed.rectTransform;

        // Set the camera feed's width and height to match the aspect ratio, and center it
        float targetHeight = rt.sizeDelta.y;  // Fixed height of 1200 (set by your mask)
        rt.sizeDelta = new Vector2(targetHeight * camAspect, targetHeight);  // Width adjusted to match aspect

        // Now center the image in the mask
        rt.localPosition = new Vector3(0, 0, 0);  // Ensure it is centered

#if !UNITY_EDITOR
        // Handle device rotation and mirroring
        int rotation = webCamTexture.videoRotationAngle;
        bool isMirrored = webCamTexture.videoVerticallyMirrored;
        rt.localEulerAngles = new Vector3(0, 0, -rotation);

        if (isMirrored)
            rt.localScale = new Vector3(1, -1, 1);
        else
            rt.localScale = Vector3.one;
#endif
    }

    void CapturePhoto()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
        {
            Debug.LogWarning("WebCamTexture is not playing or initialized.");
            return;
        }

        // Capture raw webcam data
        Texture2D rawPhoto = new Texture2D(webCamTexture.width, webCamTexture.height);
        rawPhoto.SetPixels(webCamTexture.GetPixels());
        rawPhoto.Apply();

#if !UNITY_EDITOR
        int rotation = webCamTexture.videoRotationAngle;

        switch (rotation)
        {
            case 0:
                break;
            case 90:
                rawPhoto = RotateTexture(rawPhoto, clockwise: false);
                break;
            case 180:
                rawPhoto = RotateTexture(rawPhoto, clockwise: true);
                rawPhoto = RotateTexture(rawPhoto, clockwise: true);
                break;
            case 270:
                rawPhoto = RotateTexture(rawPhoto, clockwise: true);
                break;
        }
#endif

        // Crop to exactly 800x1200 from the center
        photo = CenterCropTexture(rawPhoto, 800, 1200);

        // Display the cropped image (800x1200)
        capturedImage.texture = photo;
        capturedImage.gameObject.SetActive(true);

        // Hide camera feed and show captured image
        cameraFeed.gameObject.SetActive(false);
        captureButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        retakeButton.gameObject.SetActive(true);
    }

    void RetakePhoto()
    {
        capturedImage.gameObject.SetActive(false);
        cameraFeed.gameObject.SetActive(true);
        captureButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        retakeButton.gameObject.SetActive(false);
    }

    Texture2D RotateTexture(Texture2D original, bool clockwise = true)
    {
        int width = original.width;
        int height = original.height;

        Texture2D rotated = new Texture2D(height, width);
        Color32[] originalPixels = original.GetPixels32();
        Color32[] rotatedPixels = new Color32[originalPixels.Length];

        int x, y;
        for (int i = 0; i < originalPixels.Length; ++i)
        {
            x = i % width;
            y = i / width;

            int newX = clockwise ? height - y - 1 : y;
            int newY = clockwise ? x : width - x - 1;

            rotatedPixels[newY * height + newX] = originalPixels[i];
        }

        rotated.SetPixels32(rotatedPixels);
        rotated.Apply();
        return rotated;
    }

    Texture2D CenterCropTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        // Calculate the starting coordinates for cropping the image
        int startX = Mathf.Max(0, (source.width - targetWidth) / 2);
        int startY = Mathf.Max(0, (source.height - targetHeight) / 2);

        int cropWidth = Mathf.Min(targetWidth, source.width);
        int cropHeight = Mathf.Min(targetHeight, source.height);

        // Get the pixels from the cropped region
        Color[] pixels = source.GetPixels(startX, startY, cropWidth, cropHeight);

        // Create a new texture for the cropped image
        Texture2D result = new Texture2D(cropWidth, cropHeight, source.format, false);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }
    
    public void StopCamera()
    {
	    if (webCamTexture != null && webCamTexture.isPlaying)
	    {
		    webCamTexture.Stop();  // Stop the webcam texture and the capture
		    Debug.Log("Camera stopped.");
	    }
    }
}
