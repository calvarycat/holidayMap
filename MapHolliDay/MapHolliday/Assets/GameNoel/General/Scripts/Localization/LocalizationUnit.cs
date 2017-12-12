using System;
using UnityEngine;
using System.Collections.Generic;


public class LocalizationUnit
{
    public LocalizationUnit()
    {
        mLanguageIndex = -1;
    }

    public void SelectLanguage()
    {
        mLanguageIndex = -1;
    }

    public delegate byte[] LoadFunction(string path);

    public delegate void OnLocalizeNotification();

    /// <summary>
    ///     Want to have Localization loading be custom instead of just Resources.Load? Set this function.
    /// </summary>
    public LoadFunction loadFunction;

    /// <summary>
    ///     Notification triggered when the localization data gets changed, such as when changing the language.
    ///     If you want to make modifications to the localization data after it was loaded, this is the place.
    /// </summary>
    public OnLocalizeNotification onLocalize;

    // Loaded languages, if any
    private string[] mLanguages;

    public string[] knownLanguages
    {
        get { return mLanguages; }
    }

    public string language
    {
        get { return Localization.language; }
    }

    // Key = Value dictionary (single language)
    private Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

    // Key = Values dictionary (multiple languages)
    private Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

    // Replacement dictionary forces a specific value instead of the existing entry
    private readonly Dictionary<string, string> mReplacement = new Dictionary<string, string>();

    // Index of the selected language within the multi-language dictionary
    private int mLanguageIndex = -1;

    /// <summary>
    ///     Localization dictionary. Dictionary key is the localization key.
    ///     Dictionary value is the list of localized values (columns in the CSV file).
    /// </summary>
    public Dictionary<string, string[]> dictionary
    {
        get { return mDictionary; }
    }

    /// <summary>
    ///     Load the specified asset and activate the localization.
    /// </summary>
    public void Load(TextAsset asset)
    {
        ByteReader reader = new ByteReader(asset);
        Set(asset.name, reader.ReadDictionary());
    }

    /// <summary>
    ///     Set the localization data directly.
    /// </summary>
    public void Set(string languageName, byte[] bytes)
    {
        ByteReader reader = new ByteReader(bytes);
        Set(languageName, reader.ReadDictionary());
    }

    /// <summary>
    ///     Forcefully replace the specified key with another value.
    /// </summary>
    public void ReplaceKey(string key, string val)
    {
        if (!string.IsNullOrEmpty(val)) mReplacement[key] = val;
        else mReplacement.Remove(key);
    }

    /// <summary>
    ///     Clear the replacement values.
    /// </summary>
    public void ClearReplacements()
    {
        mReplacement.Clear();
    }

    /// <summary>
    ///     Load the specified CSV file.
    /// </summary>
    public bool LoadCSV(TextAsset asset, bool merge = false)
    {
        return LoadCSV(asset.bytes, asset, merge);
    }

    /// <summary>
    ///     Load the specified CSV file.
    /// </summary>
    public bool LoadCSV(byte[] bytes, bool merge = false)
    {
        return LoadCSV(bytes, null, merge);
    }

    private bool mMerging;

    /// <summary>
    ///     Whether the specified language is present in the localization.
    /// </summary>
    private bool HasLanguage(string languageName)
    {
        for (int i = 0, imax = mLanguages.Length; i < imax; ++i)
            if (mLanguages[i] == languageName) return true;
        return false;
    }

    /// <summary>
    ///     Load the specified CSV file.
    /// </summary>
    private bool LoadCSV(byte[] bytes, TextAsset asset, bool merge = false)
    {
        if (bytes == null) return false;
        ByteReader reader = new ByteReader(bytes);

        // The first line should contain "KEY", followed by languages.
        BetterList<string> header = reader.ReadCSV();

        // There must be at least two columns in a valid CSV file
        if (header.size < 2) return false;
        header.RemoveAt(0);

        string[] languagesToAdd = null;

        // Clear the dictionary
        if ((!merge && !mMerging) || mLanguages == null || mLanguages.Length == 0)
        {
            mDictionary.Clear();
            mLanguages = new string[header.size];

            for (int i = 0; i < header.size; ++i)
            {
                mLanguages[i] = header[i];
                if (mLanguages[i] == language)
                    mLanguageIndex = i;
            }
        }
        else
        {
            languagesToAdd = new string[header.size];
            for (int i = 0; i < header.size; ++i) languagesToAdd[i] = header[i];

            // Automatically resize the existing languages and add the new language to the mix
            for (int i = 0; i < header.size; ++i)
            {
                if (!HasLanguage(header[i]))
                {
                    int newSize = mLanguages.Length + 1;
#if UNITY_FLASH
					string[] temp = new string[newSize];
					for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = mLanguages[b];
					mLanguages = temp;
#else
                    Array.Resize(ref mLanguages, newSize);
#endif
                    mLanguages[newSize - 1] = header[i];

                    Dictionary<string, string[]> newDict = new Dictionary<string, string[]>();

                    foreach (KeyValuePair<string, string[]> pair in mDictionary)
                    {
                        string[] arr = pair.Value;
#if UNITY_FLASH
						temp = new string[newSize];
						for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = arr[b];
						arr = temp;
#else
                        Array.Resize(ref arr, newSize);
#endif
                        arr[newSize - 1] = arr[0];
                        newDict.Add(pair.Key, arr);
                    }
                    mDictionary = newDict;
                }
            }
        }

        Dictionary<string, int> languageIndices = new Dictionary<string, int>();
        for (int i = 0; i < mLanguages.Length; ++i)
            languageIndices.Add(mLanguages[i], i);

        // Read the entire CSV file into memory
        for (;;)
        {
            BetterList<string> temp = reader.ReadCSV();
            if (temp == null || temp.size == 0) break;
            if (string.IsNullOrEmpty(temp[0])) continue;
            AddCSV(temp, languagesToAdd, languageIndices);
        }

        if (!mMerging && onLocalize != null)
        {
            mMerging = true;
            OnLocalizeNotification note = onLocalize;
            onLocalize = null;
            note();
            onLocalize = note;
            mMerging = false;
        }
        return true;
    }

    /// <summary>
    ///     Helper function that adds a single line from a CSV file to the localization list.
    /// </summary>
    private void AddCSV(BetterList<string> newValues, string[] newLanguages, Dictionary<string, int> languageIndices)
    {
        if (newValues.size < 2) return;
        string key = newValues[0];
        if (string.IsNullOrEmpty(key)) return;
        string[] copy = ExtractStrings(newValues, newLanguages, languageIndices);

        if (mDictionary.ContainsKey(key))
        {
            mDictionary[key] = copy;
            if (newLanguages == null) Debug.LogWarning("Localization key '" + key + "' is already present");
        }
        else
        {
            try
            {
                mDictionary.Add(key, copy);
            }
            catch (Exception ex)
            {
                Debug.LogError("Unable to add '" + key + "' to the Localization dictionary.\n" + ex.Message);
            }
        }
    }

    /// <summary>
    ///     Used to merge separate localization files into one.
    /// </summary>
    private string[] ExtractStrings(BetterList<string> added, string[] newLanguages,
        Dictionary<string, int> languageIndices)
    {
        if (newLanguages == null)
        {
            string[] values = new string[mLanguages.Length];
            for (int i = 1, max = Mathf.Min(added.size, values.Length + 1); i < max; ++i)
                values[i - 1] = added[i];
            return values;
        }
        else
        {
            string[] values;
            string s = added[0];

            if (!mDictionary.TryGetValue(s, out values))
                values = new string[mLanguages.Length];

            for (int i = 0, imax = newLanguages.Length; i < imax; ++i)
            {
                string language = newLanguages[i];
                int index = languageIndices[language];
                values[index] = added[i + 1];
            }
            return values;
        }
    }

    /// <summary>
    ///     Load the specified asset and activate the localization.
    /// </summary>
    public void Set(string languageName, Dictionary<string, string> dictionary)
    {
        mOldDictionary = dictionary;
        mLanguageIndex = -1;
        mLanguages = new[] {languageName};
        if (onLocalize != null) onLocalize();
    }

    /// <summary>
    ///     Change or set the localization value for the specified key.
    ///     Note that this method only supports one fallback language, and should
    ///     ideally be called from within Localization.onLocalize.
    ///     To set the multi-language value just modify Localization.dictionary directly.
    /// </summary>
    public void Set(string key, string value)
    {
        if (mOldDictionary.ContainsKey(key)) mOldDictionary[key] = value;
        else mOldDictionary.Add(key, value);
    }

    /// <summary>
    ///     Localize the specified value.
    /// </summary>
    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        if (mLanguages == null)
        {
            Debug.LogError("No localization data present " + key);
            return null;
        }

        string lang = language;

        if (mLanguageIndex == -1)
        {
            for (int i = 0; i < mLanguages.Length; ++i)
            {
                if (mLanguages[i] == lang)
                {
                    mLanguageIndex = i;
                    break;
                }
            }
        }

        if (mLanguageIndex == -1)
        {
            mLanguageIndex = 0;
            Debug.LogWarning("Language not found: " + lang);
        }

        string val;
        string[] vals;

        if (mReplacement.TryGetValue(key, out val)) return val;

        if (mLanguageIndex != -1 && mDictionary.TryGetValue(key, out vals))
        {
            if (mLanguageIndex < vals.Length)
            {
                string s = vals[mLanguageIndex];
                if (string.IsNullOrEmpty(s)) s = vals[0];
                return s;
            }
            return vals[0];
        }
        if (mOldDictionary.TryGetValue(key, out val)) return val;

#if UNITY_EDITOR
        Debug.LogWarning("Localization key not found: '" + key + "' for language " + lang);
#endif
        return key;
    }

    /// <summary>
    ///     Localize the specified value and format it.
    /// </summary>
    public string Format(string key, params object[] parameters)
    {
        return string.Format(Get(key), parameters);
    }

    /// <summary>
    ///     Returns whether the specified key is present in the localization dictionary.
    /// </summary>
    public bool Exists(string key)
    {
#if UNITY_IPHONE || UNITY_ANDROID
        string mobKey = key + " Mobile";
        if (mDictionary.ContainsKey(mobKey)) return true;
        if (mOldDictionary.ContainsKey(mobKey)) return true;
#endif
        return mDictionary.ContainsKey(key) || mOldDictionary.ContainsKey(key);
    }

    /// <summary>
    ///     Add a new entry to the localization dictionary.
    /// </summary>
    public void Set(string language, string key, string text)
    {
        // Check existing languages first
        string[] kl = mLanguages;

        if (kl == null)
        {
            mLanguages = new[] {language};
            kl = mLanguages;
        }

        for (int i = 0, imax = kl.Length; i < imax; ++i)
        {
            // Language match
            if (kl[i] == language)
            {
                string[] vals;

                // Get all language values for the desired key
                if (!mDictionary.TryGetValue(key, out vals))
                {
                    vals = new string[kl.Length];
                    mDictionary[key] = vals;
                    vals[0] = text;
                }

                // Assign the value for this language
                vals[i] = text;
                return;
            }
        }

        // Expand the dictionary to include this new language
        int newSize = mLanguages.Length + 1;
#if UNITY_FLASH
		string[] temp = new string[newSize];
		for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = mLanguages[b];
		mLanguages = temp;
#else
        Array.Resize(ref mLanguages, newSize);
#endif
        mLanguages[newSize - 1] = language;

        Dictionary<string, string[]> newDict = new Dictionary<string, string[]>();

        foreach (KeyValuePair<string, string[]> pair in mDictionary)
        {
            string[] arr = pair.Value;
#if UNITY_FLASH
			temp = new string[newSize];
			for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = arr[b];
			arr = temp;
#else
            Array.Resize(ref arr, newSize);
#endif
            arr[newSize - 1] = arr[0];
            newDict.Add(pair.Key, arr);
        }
        mDictionary = newDict;

        // Set the new value
        string[] values;

        if (!mDictionary.TryGetValue(key, out values))
        {
            values = new string[kl.Length];
            mDictionary[key] = values;
            values[0] = text;
        }
        values[newSize - 1] = text;
    }
}