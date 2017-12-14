using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

/*
 * https://github.com/ChrisMaire/unity-native-sharing
 */

public class ShareManager : MonoBehaviour {
	public string ScreenshotName = "screenshot.png";
    private Action<string> _OnShareFinish;

    //public void ShareImage(string text, Action<string> OnShareFinish)
    //{
    //    ShareScreenshotWithText(text, OnShareFinish);
    //}
    public void ShareScreenshotWithText(string text, Action<string> OnShareFinish)
    {
        _OnShareFinish = OnShareFinish;

        string screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        if(File.Exists(screenShotPath)) File.Delete(screenShotPath);

        Application.CaptureScreenshot(ScreenshotName);

        StartCoroutine(delayedShare(screenShotPath, text));
    }

    //CaptureScreenshot runs asynchronously, so you'll need to either capture the screenshot early and wait a fixed time
    //for it to save, or set a unique image name and check if the file has been created yet before sharing.
    IEnumerator delayedShare(string screenShotPath, string text)
    {
        while(!File.Exists(screenShotPath)) {
    	    yield return new WaitForSeconds(.05f);
        }

		NativeShare.Share(text, screenShotPath, "", "", "image/png", true, "");
        yield return null;
        if (_OnShareFinish != null)
            _OnShareFinish("Share finish");
    }
}
