using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selfie : MonoBehaviour
{

    public pnSuccess pnSuccess;
    public pnSentForm pnSentForm;
    private bool camAvailable;
    private WebCamTexture cameraTexture;
    private Texture defaultBackground;
    public RawImage background;
    public AspectRatioFitter fit;
    public bool frontFacing;
    WebCamDevice[] devices;
    public ShareManager share;
    public GameObject[] objBefore;
    public GameObject[] objAfter;
    private void OnEnable()
    {
        StartSelfie();
    }
    void StartSelfie()
    {
        ProcessTakePic(true);
        defaultBackground = background.texture;
        devices = WebCamTexture.devices;

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
    }

    public void OnTakeImageClick()
    {
#if UNITY_EDITOR
        gameObject.SetActive(false);
        pnSentForm.OnShow(true);
        return;
#endif
        cameraTexture.Stop();
        ProcessTakePic(false);

    }
    void ProcessTakePic(bool isHide)
    {
       
        for (int i = 0; i < objBefore.Length; i++)
        {
            objBefore[i].SetActive(isHide);
        }
        for (int i = 0; i < objAfter.Length; i++)
        {
            objAfter[i].SetActive(!isHide);
        }
    }
    public void OnShareFinish(string re)
    {
       // cameraTexture.Play();
        gameObject.SetActive(false);
        pnSentForm.OnShow(true);

    }
    public void OnrotateClick()
    {
        cameraTexture.Stop();
        cameraTexture.deviceName = (cameraTexture.deviceName == devices[0].name) ? devices[1].name : devices[0].name;
        if (cameraTexture.deviceName == devices[0].name)
        {

            background.transform.eulerAngles = new Vector3(0, 0, -90);
            background.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {

            background.transform.eulerAngles = new Vector3(0, 0, -270);
            background.transform.localScale = new Vector3(1, -1, 1);
        }
        cameraTexture.Play();
    }

    public void OnCanCleClick()
    {
        pnSuccess.OnShow(true);
        gameObject.SetActive(false);
    }
    public void OnShareClick()
    {
        share.ShareScreenshotWithText("Share image", OnShareFinish);
    }
    public void OnBackCameraClick()
    {
        cameraTexture.Play();
        ProcessTakePic(true);
    }
}
