using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The class allows you to load a Google Sheet from Drive in CSV format.
/// It parses it and fires an event when it's done.
/// 
/// Note:
/// This implementation will only work for sheets in the format:
/// _____________________________
/// |   name    |     value     |
/// |___________|_______________|
/// |   ....    |     .....     |
/// 
/// To support a different format change the ParseSheetData method
/// </summary>
public class GoogleSheetsLoader : MonoBehaviour
{
    // a Specific end-point for downloading a Google Sheet in CSV format
    private const string GoogleDriveFormat = "http://docs.google.com/feeds/download/spreadsheets/Export?key={0}&exportFormat=csv&gid={1}";
    // The unique identifier of the google drive file we are using
    private const string GoogleDriveFileGuid = "1lW-uec71bgVSEwJCiRjxjYD4E3PDs70MKbErxqWent0";
    // The unique identifier for the sheet inside the file we are using
    private const string GoogleDriveSheetGuid = "0";

    private readonly Dictionary<string, object> _loadedSheet = new Dictionary<string, object>();

    /// <summary>
    /// The parsed Google Sheet available after the event is fired
    /// </summary>
    public Dictionary<string, object> LoadedSheet
    {
        get { return _loadedSheet; }
    }

    /// <summary>
    /// Fired when the class has finished loading and parsing the Google Sheet
    /// </summary>
    public event Action SheetLoaded;

    /// <summary>
    /// Fired when the loading from Google failed
    /// </summary>
    public event Action SheetLoadFailed;

    /// <summary>
    /// Starts loading the sheet from Google
    /// </summary>
    public void LoadSheet()
    {
        StartCoroutine(LoadGoogleSheet(GoogleDriveFileGuid, GoogleDriveSheetGuid));
    }

    /// <summary>
    /// a Coroutine which downloads the Google Sheet in CSV format and parses it on success
    /// </summary>
    /// <param name="docId">The document's unique identifier on Google Drive</param>
    /// <param name="sheetId">The sheet's unique identifier in the Google Sheets document</param>
    private IEnumerator LoadGoogleSheet(string docId, string sheetId)
    {
        _loadedSheet.Clear();

        string downloadUrl = string.Format(GoogleDriveFormat, docId, sheetId);
        WWW serverCall = new WWW(downloadUrl);

        yield return serverCall;

        if (!string.IsNullOrEmpty(serverCall.error))
        {
            Debug.LogError("Unable to fetch CSV data from Google");

            if (SheetLoadFailed != null)
            {
                SheetLoadFailed();
            }
            yield break;
        }

        ParseSheetData(serverCall.text);
    }

    /// <summary>
    /// Parses the downloaded CSV formatted Google Sheet
    /// </summary>
    /// <param name="csvData">The Google Sheet in CSV format</param>
    private void ParseSheetData(string csvData)
    {
        if (string.IsNullOrEmpty(csvData))
        {
            return;
        }

        List<Dictionary<string, object>> gameParametersData = CSVReader.Read(csvData);

        foreach (var rowData in gameParametersData)
        {
            string paramName = null;
            object paramValue = null;
            foreach (var columnData in rowData)
            {
                if (columnData.Key == "name")
                {
                    paramName = (string)columnData.Value;
                }
                else if (columnData.Key == "value")
                {
                    paramValue = columnData.Value;
                }
            }

            if (string.IsNullOrEmpty(paramName) || (paramValue == null))
            {
                continue;
            }

            ApplyDataFromRow(paramName, paramValue);
        }

        if (SheetLoaded != null)
        {
            SheetLoaded();
        }
    }

    /// <summary>
    /// Stores the loaded parameter configuration locally
    /// </summary>
    /// <param name="paramName">The configuration parameter name</param>
    /// <param name="paramValue">The configuration parameter value</param>
    private void ApplyDataFromRow(string paramName, object paramValue)
    {
        if (_loadedSheet.ContainsKey(paramName))
        {
            _loadedSheet[paramName] = paramValue;
        }
        else
        {
            _loadedSheet.Add(paramName, paramValue);
        }
    }
}

