using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class DeviceCameraController : MonoBehaviour
{
	public RawImage image;
	public RectTransform imageParent;
	public AspectRatioFitter imageFitter;

	// Device cameras
	WebCamDevice frontCameraDevice;
	WebCamDevice backCameraDevice;
	WebCamDevice activeCameraDevice;

	WebCamTexture frontCameraTexture;
	WebCamTexture backCameraTexture;
	public WebCamTexture activeCameraTexture;

	// Image rotation
	Vector3 rotationVector = new Vector3(0f, 0f, 0f);

	// Image uvRect
	Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
	Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

	// Image Parent's scale
	Vector3 defaultScale = new Vector3(1f, 1f, 1f);
	Vector3 fixedScale = new Vector3(-1f, 1f, 1f);

    System.Action OnPictureTaken;

    public GameObject Feed;
    public GameObject Button;

    public Text LandscapeOnlyText;

    public static DeviceCameraController instance;

    public bool IsActive
    {
        get
        {
            return Feed.activeSelf;
        }
    }

    bool landscapeOnly = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
       
    }


    public void Show(System.Action OnPictureTaken, bool landscape = false)
    {
        this.landscapeOnly = landscape;
        SetActiveCamera(backCameraTexture);
        Feed.SetActive(true);
        Button.SetActive(true);
        this.OnPictureTaken = OnPictureTaken;
    }
    public void Hide()
    {
        Feed.SetActive(false);
        Button.SetActive(false);
    }

    public void SnapPicture(bool landScape = false)
    {
        if (landScape && Input.deviceOrientation != DeviceOrientation.LandscapeLeft && Input.deviceOrientation != DeviceOrientation.LandscapeRight)
            return;
        if (OnPictureTaken != null)
            OnPictureTaken();
        Hide();
    }

    void OnEnable()
	{
		// Check for device cameras
		if (WebCamTexture.devices.Length == 0)
		{
			Debug.Log("No devices cameras found");
			return;
		}

		// Get the device's cameras and create WebCamTextures with them
		frontCameraDevice = WebCamTexture.devices.Last();
		backCameraDevice = WebCamTexture.devices.First();
        

		frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
		backCameraTexture = new WebCamTexture(backCameraDevice.name, 1024, 768, 40);

		// Set camera filter modes for a smoother looking image
		frontCameraTexture.filterMode = FilterMode.Trilinear;
		backCameraTexture.filterMode = FilterMode.Trilinear;

		// Set the camera to use by default		
	}

	// Set the device camera to use and start it
	public void SetActiveCamera(WebCamTexture cameraToUse)
	{
		if (activeCameraTexture != null)
		{
			activeCameraTexture.Stop();
		}

		activeCameraTexture = cameraToUse;
		activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device => 
			device.name == cameraToUse.deviceName);

		image.texture = activeCameraTexture;
		image.material.mainTexture = activeCameraTexture;

		activeCameraTexture.Play();
        
	}

    public byte[] TakePictureData()
    {
        Texture2D picture = new Texture2D(activeCameraTexture.width,activeCameraTexture.height);
        
        picture.SetPixels(activeCameraTexture.GetPixels());
        picture.Apply();
        
        switch(Input.deviceOrientation)
        {
            case DeviceOrientation.LandscapeRight:
                picture = rotateTexture(rotateTexture(picture, true), true);
                break;                
        }

       // picture = rotateTexture(picture, false);
        return picture.EncodeToJPG();
    }

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();        
        return rotatedTexture;
    }


    // Switch between the device's front and back camera
    public void SwitchCamera()
	{
		SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ? 
			backCameraTexture : frontCameraTexture);
	}

	// Make adjustments to image every frame to be safe, since Unity isn't 
	// guaranteed to report correct data as soon as device camera is started
	void Update()
	{
        if (activeCameraTexture == null)
            return;
		// Skip making adjustment for incorrect camera data
		if (activeCameraTexture.width < 100)
		{
			Debug.Log("Still waiting another frame for correct info...");
			return;
		}

        if (IsActive && landscapeOnly)
        {
            if (Input.deviceOrientation != DeviceOrientation.LandscapeLeft
                && Input.deviceOrientation != DeviceOrientation.LandscapeRight)
            {
                LandscapeOnlyText.enabled = true;
            } else
            {
                LandscapeOnlyText.enabled = false;
            }
        } else
        {
            LandscapeOnlyText.enabled = false;
        }

		// Rotate image to show correct orientation 
		rotationVector.z = -activeCameraTexture.videoRotationAngle;
		image.rectTransform.localEulerAngles = rotationVector;

		// Set AspectRatioFitter's ratio
		float videoRatio = 
			(float)activeCameraTexture.width / (float)activeCameraTexture.height;
		imageFitter.aspectRatio = videoRatio;

		// Unflip if vertically flipped
		image.uvRect = 
			activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

		// Mirror front-facing camera's image horizontally to look more natural
		imageParent.localScale = 
			activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;
	}
}