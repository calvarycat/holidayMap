using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Utility
{
    public static void DeleteFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return;

        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
        {
            directory.Delete(true);
        }
    }

    public static string GetMd5Hash(string input)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
    public static Sprite Texture2DToSprite(Texture2D input)
    {
        Rect rec = new Rect(0, 0, input.width, input.height);
        return Sprite.Create(input, rec, new Vector2(0.5f, 0.5f));
    }
}