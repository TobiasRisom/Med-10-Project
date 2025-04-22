using UnityEngine;
using UnityEngine.UI;

public class CameraDisplay : MonoBehaviour
{
    public RawImage cameraFeed;
    public RawImage capturedImage;
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

    private void SetupCamera()
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

        // Create a new WebCamTexture with a resolution that is reasonable for mobile devices
        webCamTexture = new WebCamTexture(selectedCam, 640, 480); // 640x480 resolution
        cameraFeed.texture = webCamTexture;
        cameraFeed.material.mainTexture = webCamTexture;

        // Start the camera feed
        if (webCamTexture != null)
        {
            webCamTexture.Play();

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

        capturedImage.texture = photo;
        capturedImage.gameObject.SetActive(true);

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
}
