using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using UnityEngine.UI;

public class Utils
{
    public static Sprite LoadResourcesSprite(string param)
    {
        return Resources.Load<Sprite>("" + param);
    }

    public static T[] CloneArray<T>(T[] paramArray)
    {
        if (paramArray == null)
            return null;
        return paramArray.Clone() as T[];
    }

    public static List<T> CloneArray<T>(List<T> paramArray)
    {
        if (paramArray == null)
            return null;
        List<T> list = new List<T>(paramArray.ToArray());
        return list;
    }

    public static GameObject Spawn(GameObject paramPrefab, Transform paramParent = null)
    {
        GameObject newObject = Object.Instantiate(paramPrefab);
        newObject.transform.SetParent(paramParent, false);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = paramPrefab.transform.localScale;
        newObject.SetActive(true);
        return newObject;
    }

    public static void RemoveAllChildren(Transform paramParent, bool paramInstant = true)
    {
        if (paramParent == null)
            return;

        for (int i = paramParent.childCount - 1; i >= 0; i--)
        {
            if (paramInstant)
            {
                Object.DestroyImmediate(paramParent.GetChild(i).gameObject);
            }
            else
            {
                paramParent.GetChild(i).gameObject.SetActive(false);
                Object.Destroy(paramParent.GetChild(i).gameObject);
            }
        }
    }

    public static bool IsValidEmail(string email)
    {
        string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

        if (Regex.IsMatch(email, expresion))
        {
            if (Regex.Replace(email, expresion, string.Empty).Length == 0)
            {
                return true;
            }
            return false;
        }

        return false;
    }
    // bắt đầu từ số 0 tương ứng với A
    public static string GetLableAnswer(int lab)
    {
        char rs = (char)65;
        rs = Convert.ToChar(lab + 65);
        return rs.ToString().ToUpper();

    }
    public static string TextureToString(Texture2D value)
    {
        byte[] byteArray = value.EncodeToPNG();
        string stringData = Convert.ToBase64String(byteArray);
        return stringData;
    }

    public static Texture2D StringToTexture(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        byte[] decodedFromBase64;

        try
        {
            decodedFromBase64 = Convert.FromBase64String(value);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }

        Texture2D image = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        image.LoadImage(decodedFromBase64);
        return image;
    }

    public static string LoadTextFromFile(string path)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadToEnd();
                return line;
            }
        }
        catch (Exception error)
        {
            Debug.Log("Read File Error: " + error);
            return null;
        }
    }

    public static void SaveTextToFile(string path, string data)
    {
        File.WriteAllText(path, data);
    }

    public static byte[] LoadBytesFromFile(string path)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                byte[] bytes = sr.CurrentEncoding.GetBytes(sr.ReadToEnd());
                return bytes;
            }
        }
        catch (Exception error)
        {
            Debug.Log("Read File Error: " + error);
            return null;
        }
    }

    public static string MilisecondsToString(long miliseconds) //return hour - minutes-seconds
    {
        long totalseconds = miliseconds / 1000;
        long hour = 0;
        long minute = totalseconds / 60;
        long second = totalseconds % 60;

        if (minute > 60)
        {
            hour = minute / 60;
            minute = minute % 60;
        }

        if (hour > 0)
        {
            return hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");
        }

        return minute.ToString("00") + ":" + second.ToString("00");
    }
    public static string SecondToString(int totalseconds) //return hour - minutes-seconds
    {

        long hour = 0;
        long minute = totalseconds / 60;
        long second = totalseconds % 60;

        if (minute > 60)
        {
            hour = minute / 60;
            minute = minute % 60;
        }

        if (hour > 0)
        {
            return hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");
        }

        return minute.ToString("00") + ":" + second.ToString("00");
    }

    public static bool SetTextDropDown(Dropdown drop, string value, bool addnew = true)
    {
        for (int i = 0; i < drop.options.Count; i++)
        {
            if (drop.options[i].text == value)
            {
                drop.value = i;
                return true;
            }
        }

        if (addnew)
        {
            drop.options.Add(new Dropdown.OptionData { text = value });
            drop.value = drop.options.Count - 1;
        }

        return false;
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(fileData);
        }

        if (tex == null)
        {
            string pathWithExtension = filePath + ".png";
            if (File.Exists(pathWithExtension))
            {
                fileData = File.ReadAllBytes(pathWithExtension);
                tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.LoadImage(fileData);
            }
        }

        if (tex == null)
        {
            string pathWithExtension = filePath + ".jpg";
            if (File.Exists(pathWithExtension))
            {
                fileData = File.ReadAllBytes(pathWithExtension);
                tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.LoadImage(fileData);
            }
        }

        return tex;
    }

    public static void ClampImageSize(Graphic image, float max)
    {
        float width = image.mainTexture.width;
        float height = image.mainTexture.height;

        if (width > height)
        {
            if (width <= max)
                return;

            float ratio = max / width;
            float newWidth = ratio * width;
            float newHeight = ratio * height;
            image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
        else
        {
            if (height <= max)
                return;

            float ratio = max / height;
            float newWidth = ratio * width;
            float newHeight = ratio * height;
            image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }




    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public static double CalculateDistance(Location location1, Location location2)
    {
        double circumference = 40000.0; // Earth's circumference at the equator in km
        double distance = 0.0;

        //Calculate radians
        double latitude1Rad = DegreesToRadians(location1.Latitude);
        double longitude1Rad = DegreesToRadians(location1.Longitude);
        double latititude2Rad = DegreesToRadians(location2.Latitude);
        double longitude2Rad = DegreesToRadians(location2.Longitude);

        double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

        if (logitudeDiff > Math.PI)
        {
            logitudeDiff = 2.0 * Math.PI - logitudeDiff;
        }

        double angleCalculation =
            Math.Acos(
              Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
              Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

        distance = circumference * angleCalculation / (2.0 * Math.PI);

        return distance;
    }

    public static double CalculateDistance(params Location[] locations)
    {
        double totalDistance = 0.0;

        for (int i = 0; i < locations.Length - 1; i++)
        {
            Location current = locations[i];
            Location next = locations[i + 1];

            totalDistance += CalculateDistance(current, next);
        }

        return totalDistance;
    }
}
public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class EnumParser
{
    public static T Parse<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}

public enum MonthOfYear
{
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}