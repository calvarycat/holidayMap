using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System;
using System.Reflection;
using ICSharpCode.SharpZipLib.Core;

public class ZipManager : MonoSingleton<ZipManager>
{
    public static bool Tracking;

    private int _fileNumber;
    private const string TempName = "TMP_FILE_";

    public void DownloadAndUnzip(string url,
        string password,
        string outFolder,
        Action<float> onDownloading = null,
        Action<bool> onDownloadAndUnzipFinish = null)
    {
        StartCoroutine(DownloadAndUnzipCoroutine(url, password, outFolder, onDownloading, onDownloadAndUnzipFinish));
    }

    public IEnumerator DownloadAndUnzipCoroutine(string url,
        string password,
        string outFolder,
        Action<float> onDownloading = null,
        Action<bool> onDownloadAndUnzipFinish = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
            PrintTrack("password: " + password);
            PrintTrack("outFolder: " + outFolder);
        }
#endif

        string archiveName = TempName + _fileNumber++;
        yield return DownloadZipCoroutine(url, outFolder, archiveName, onDownloading,
            (bool downloadSucess) =>
            {
                if (downloadSucess)
                {
                    ExtractZipFile(outFolder + "/" + archiveName, password, outFolder, onDownloadAndUnzipFinish);
                }
                else
                {
                    if (onDownloadAndUnzipFinish != null)
                        onDownloadAndUnzipFinish(false);
                }
            }
            );
    }

    public void DownloadZip(string url,
        string outFolder,
        string archiveName,
        Action<float> onDownloading,
        Action<bool> onDownloadFinish)
    {
        StartCoroutine(DownloadZipCoroutine(url, outFolder, archiveName, onDownloading, onDownloadFinish));
    }

    public IEnumerator DownloadZipCoroutine(string url,
        string outFolder,
        string archiveName,
        Action<float> onDownloading,
        Action<bool> onDownloadFinish)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("url: " + url);
            PrintTrack("outFolder: " + outFolder);
            PrintTrack("archiveName: " + archiveName);
        }
#endif

        WWW www = new WWW(url);

        while (!www.isDone)
        {
            if (onDownloading != null)
                onDownloading(www.progress);
            yield return null;
        }

        if (onDownloading != null)
            onDownloading(www.progress);

        try
        {
            if (string.IsNullOrEmpty(www.error))
            {
                if (!Directory.Exists(outFolder))
                {
                    Directory.CreateDirectory(outFolder);
                }

                string tmpName = outFolder + archiveName;
                File.WriteAllBytes(tmpName, www.bytes);

                if (onDownloadFinish != null)
                    onDownloadFinish(true);
            }
            else
            {
                if (onDownloadFinish != null)
                    onDownloadFinish(false);
            }
        }
        catch (Exception error)
        {
            Debug.Log(error);
            if (onDownloadFinish != null)
                onDownloadFinish(false);
        }
    }

    public void ExtractZipFile(string archiveName,
        string password,
        string outFolder,
        Action<bool> onExtractFinish = null,
        bool deleteArchiveFile = true)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("archiveName: " + archiveName);
            PrintTrack("password: " + password);
            PrintTrack("outFolder: " + outFolder);
            PrintTrack("deleteArchiveFile: " + deleteArchiveFile);
        }
#endif

        ZipFile zf = null;

        try
        {
            FileStream fs = File.OpenRead(archiveName);
            zf = new ZipFile(fs);

            if (!string.IsNullOrEmpty(password))
            {
                zf.Password = password; // AES encrypted entries are handled automatically
            }

            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile)
                {
                    continue; // Ignore directories
                }

                string entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                byte[] buffer = new byte[4096]; // 4K is optimum
                Stream zipStream = zf.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                string fullZipToPath = Path.Combine(outFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }

            if (onExtractFinish != null)
                onExtractFinish(true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            if (onExtractFinish != null)
                onExtractFinish(false);
        }
        finally
        {
            if (zf != null)
            {
                zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                zf.Close(); // Ensure we release resources
            }

            if (deleteArchiveFile && File.Exists(archiveName))
                File.Delete(archiveName);
        }
    }

    public void CompressZip(string outPathname,
        string password,
        string folderName,
        Action<bool> onCompressFinish = null)
    {
#if UNITY_EDITOR
        if (Tracking)
        {
            PrintTrack(MethodBase.GetCurrentMethod().Name);
            PrintTrack("outPathname: " + outPathname);
            PrintTrack("password: " + password);
            PrintTrack("folderName: " + folderName);
        }
#endif

        FileStream fsOut = null;
        ZipOutputStream zipStream = null;

        try
        {
            fsOut = File.Create(outPathname);
            zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            zipStream.Password = password; // optional. Null is the same as not setting. Required if using AES.

            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            CompressFolder(folderName, zipStream, folderOffset);

            if (onCompressFinish != null)
                onCompressFinish(true);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            if (onCompressFinish != null)
                onCompressFinish(false);
        }
        finally
        {
            if (zipStream != null)
            {
                zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                zipStream.Close();
            }
        }
    }

    private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
    {
        string[] files = Directory.GetFiles(path);

        foreach (string filename in files)
        {
            FileInfo fi = new FileInfo(filename);

            string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
            entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

            // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
            // A password on the ZipOutputStream is required if using AES.
            //   newEntry.AESKeySize = 256;

            // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
            // you need to do one of the following: Specify UseZip64.Off, or set the Size.
            // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
            // but the zip will be in Zip64 format which not all utilities can understand.
            //   zipStream.UseZip64 = UseZip64.Off;
            newEntry.Size = fi.Length;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];

            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }

            zipStream.CloseEntry();
        }

        string[] folders = Directory.GetDirectories(path);

        foreach (string folder in folders)
        {
            CompressFolder(folder, zipStream, folderOffset);
        }
    }

    private void PrintTrack(string message)
    {
        Debug.Log("ZipManager: " + message);
    }
}