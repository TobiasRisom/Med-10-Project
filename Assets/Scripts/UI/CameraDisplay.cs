using UnityEngine;
using UnityEngine.UI;

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

        if (fish.TaskData[fish.currentTask].ImageFormat)
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
        // Try selecting the back-facing camera on Android
        foreach (var device in devices)
        {
            if (!device.isFrontFacing)  // Prefer back camera
            {
                selectedCam = device.name;
                break;
            }
        }
#endif

        // Debugging: log the selected camera
        Debug.Log("Selected camera: " + selectedCam);

        // Create a new WebCamTexture with a reasonable resolution for mobile
        webCamTexture = new WebCamTexture(selectedCam, 640, 480); // 640x480 resolution
        cameraFeed.texture = webCamTexture; // Directly assign the WebCamTexture to RawImage

        // Start the camera feed
        if (webCamTexture != null)
        {
            webCamTexture.Play();

#if !UNITY_EDITOR // Rotate camera feed
            int rotation = webCamTexture.videoRotationAngle;
            bool isMirrored = webCamTexture.videoVerticallyMirrored;

            // Apply rotation to the feed (camera preview)
            cameraFeed.rectTransform.localEulerAngles = new Vector3(0, 0, -rotation);

            if (isMirrored)
            {
                cameraFeed.rectTransform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                cameraFeed.rectTransform.localScale = Vector3.one;
            }
#endif

            // Debugging: Check if the WebCamTexture is playing
            if (webCamTexture.isPlaying)
            {
                Debug.Log("WebCamTexture is playing.");
            }
            else
            {
                Debug.LogError("WebCamTexture failed to start playing.");
            }
        }
        else
        {
            Debug.LogError("Failed to initialize WebCamTexture.");
        }

        captureButton.onClick.AddListener(CapturePhoto);
        retakeButton.onClick.AddListener(RetakePhoto);
    }

    void CapturePhoto()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
        {
            Debug.LogWarning("WebCamTexture is not playing or initialized.");
            return;
        }

        // Create a photo from the WebCamTexture
        photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

#if !UNITY_EDITOR
        // Handle the final image rotation based on the webCamTexture's videoRotationAngle
        int rotation = webCamTexture.videoRotationAngle;

        // Apply rotation based on the camera feed angle
        switch (rotation)
        {
            case 0:
                // No rotation needed, already upright
                break;

            case 90:
                // 90 degrees counterclockwise, so we rotate 90 degrees clockwise
                photo = RotateTexture(photo, clockwise: false);
                break;

            case 180:
                // 180 degrees (upside down), so we rotate 180 degrees
                photo = RotateTexture(photo, clockwise: true);
                photo = RotateTexture(photo, clockwise: true);  // Apply 180-degree rotation
                break;

            case 270:
                // 270 degrees counterclockwise (equivalent to 90 degrees clockwise), so rotate 90 degrees counterclockwise
                photo = RotateTexture(photo, clockwise: true);
                break;
        }
#endif

        // Now assign the captured photo to the capturedImage RawImage
        capturedImage.texture = photo;
        capturedImage.gameObject.SetActive(true);

        // Hide the live camera feed and show the captured photo
        cameraFeed.gameObject.SetActive(false);
        captureButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        retakeButton.gameObject.SetActive(true);
    }

    void RetakePhoto()
    {
        // Hide the captured photo and re-enable the camera feed
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
}
