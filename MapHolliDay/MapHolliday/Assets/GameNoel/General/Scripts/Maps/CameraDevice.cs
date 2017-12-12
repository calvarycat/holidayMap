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
    public Quaternion baseRotation;
    void Start()
    {
        webCam = new WebCamTexture();
        camImage.texture = webCam;
        camImage.material.mainTexture = webCam;
        //  baseRotation = transform.rotation;
        webCam.Play();
//#if UNITY_IPHONE
//        transform.Rotate(0, 0, 0);
//#endif
    }
    public void OnHide()
    {
        gameObject.SetActive(false);
    }
    public void OnEnable()
    {
#if UNITY_IPHONE
        int randomOpen = Random.Range(1, 2);
#else
        int randomOpen = Random.Range(10, 30);
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

    //void Update()
    //{
    //    transform.rotation = baseRotation * Quaternion.AngleAxis(webCam.videoRotationAngle, Vector3.up);
    //}
}
