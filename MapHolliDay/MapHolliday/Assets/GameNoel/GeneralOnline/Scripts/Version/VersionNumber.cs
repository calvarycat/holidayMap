using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public enum CompareVersionNumber
{
    MajorGreater = 3,
    MajorLess = -3,
    MinorGreater = 2,
    MinorLess = -2,
    PacthGreater = 1,
    PatchLess = -1,
    Equal = 0,
    Error = 999
}

public class VersionNumber : IComparable
{
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Patch { get; private set; }

    public VersionNumber(int major = 0, int minor = 0, int patch = 0)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public int CompareTo(object obj)
    {
        VersionNumber otherVersionNumber = obj as VersionNumber;

        if (otherVersionNumber != null)
        {
            if (Major < otherVersionNumber.Major)
                return (int)CompareVersionNumber.MajorLess;

            if (Major > otherVersionNumber.Major)
                return (int)CompareVersionNumber.MajorGreater;

            if (Minor < otherVersionNumber.Minor)
                return (int)CompareVersionNumber.MinorLess;

            if (Minor > otherVersionNumber.Minor)
                return (int)CompareVersionNumber.MinorGreater;

            if (Patch < otherVersionNumber.Patch)
                return (int)CompareVersionNumber.PatchLess;

            if (Patch > otherVersionNumber.Patch)
                return (int)CompareVersionNumber.PacthGreater;

            return (int)CompareVersionNumber.Equal;
        }

        Debug.LogError("Object is not a VersionNumber");
        return (int)CompareVersionNumber.Error;
    }

    public override string ToString()
    {
        return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
    }

    public static VersionNumber Parse(string numberString)
    {
        VersionNumber result = new VersionNumber();

        try
        {
            if (numberString.Contains('.'))
            {
                string[] stringArray = numberString.Split('.');

                switch (stringArray.Length)
                {
                    case 1:
                        result.Major = int.Parse(stringArray[0]);
                        break;

                    case 2:
                        result.Major = int.Parse(stringArray[0]);
                        result.Minor = int.Parse(stringArray[1]);
                        break;

                    case 3:
                        result.Major = int.Parse(stringArray[0]);
                        result.Minor = int.Parse(stringArray[1]);
                        result.Patch = int.Parse(stringArray[2]);
                        break;

                    default:
                        Debug.LogError("Version number not supported: " + numberString);
                        return null;
                }
            }
            else
            {
                result.Major = int.Parse(numberString);
            }

            return result;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}