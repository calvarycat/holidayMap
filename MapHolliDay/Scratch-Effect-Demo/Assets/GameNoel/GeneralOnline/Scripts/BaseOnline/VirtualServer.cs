using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirtualServer : MonoSingleton<VirtualServer>
{
    private readonly Dictionary<string, IVirtualServerProcess> _dictionary
        = new Dictionary<string, IVirtualServerProcess>();

    public void Register(string url, IVirtualServerProcess process)
    {
        if (_dictionary.ContainsKey(url))
        {
            _dictionary[url] = process;
        }
        else
        {
            _dictionary.Add(url, process);
        }
    }

    public void UnRegister(string url)
    {
        if (_dictionary.ContainsKey(url))
        {
            _dictionary.Remove(url);
        }
    }

    public string Send(string url, object data = null)
    {
        if (_dictionary.ContainsKey(url))
        {
            return _dictionary[url].ProcessData(url, data);
        }

        Debug.LogError("Virtual Server: url not found: " + url);
        return "";
    }

    public bool IsContainUrl(string url)
    {
        return _dictionary.ContainsKey(url);
    }
}

public interface IVirtualServerProcess
{
    string ProcessData(string url, object inputData);
}