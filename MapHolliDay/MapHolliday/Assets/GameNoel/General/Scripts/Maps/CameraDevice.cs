using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraDevice : MonoBehaviour
{

    public RawImage camImage;
    WebCamTexture webCam;
    public GameObject PnQuestion;
    public GameObject gift;
    public ControlQuestion controlQuestion;
 
    //void Start()
    //{
    //    webCam = new WebCamTexture();
    //    camImage.texture = webCam;
      //    camImage.material.mainTexture = webCam;
    //    baseRotation = transform.rotation;
    //    webCam.Play();
    //}
    public void OnHide()
    {
        gameObject.SetActive(false);
    }
    public void OnEnable()
    {
#if UNITY_PHONE
        int randomOpen = Random.Range(10, 20);
#else
        int randomOpen = Random.Range(1, 2);
#endif
        Invoke("ShowTheGift", randomOpen);
    }

    void ShowTheGift()
    {
        gift.gameObject.SetActive(true);
    }
    public void OpenTheBox()
    {

        controlQuestion.InitMultipleChoise();
        PnQuestion.gameObject.SetActive(true);

        OnHide();
    }

  

    private bool camAvailable;
    private WebCamTexture cameraTexture;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public bool frontFacing;

    // Use this for initialization
    void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
            return;

        for (int i = 0; i < devices.Length; i++)
        {
            var curr = devices[i];

            if (curr.isFrontFacing == frontFacing)
            {
              
                cameraTexture = new WebCamTexture(curr.name, Screen.width, Screen.height);
                break;
            }
        }

        if (cameraTexture == null)
            return;

        cameraTexture.Play(); // Start the camera
        background.texture = cameraTexture; // Set the texture

        camAvailable = true; // Set the camAvailable for future purposes.
      //  background.transform.Rotate(0, 0, -90);
    }

    // Update is called once per frame
    void Update()
    {
        if (!camAvailable)
            return;

        //float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
        //fit.aspectRatio = ratio; // Set the aspect ratio

        //float scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
        //background.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

        //int orient = -cameraTexture.videoRotationAngle;
        //background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

    }
}
