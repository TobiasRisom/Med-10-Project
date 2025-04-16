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
        foreach (var device in devices)
        {
            if (!device.isFrontFacing)
            {
                selectedCam = device.name;
                break;
            }
        }
#endif

        webCamTexture = new WebCamTexture(selectedCam, Screen.width, Screen.height);
        cameraFeed.texture = webCamTexture;
        cameraFeed.material.mainTexture = webCamTexture;
        webCamTexture.Play();
        
        captureButton.onClick.AddListener(CapturePhoto);
        retakeButton.onClick.AddListener(RetakePhoto);
    }

    void CapturePhoto()
    {
        if (webCamTexture == null || !webCamTexture.isPlaying)
            return;

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
