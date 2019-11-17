using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System;
// Delegate to support a placeholder for database responses as List of specified types
public delegate List<T> setResult<T>(List<T> pResult);
// Class to handle the JSNdrop database connection and core interactions/responses
public class JSNDrop : MonoBehaviour
{
    private string serverPathURL = "http://jsnDrop.com/Q"; // Server path
    private string qStr; // Variable for string to send to Database
    //Dictionary of tables in databse and there respective connection IDs
    public Dictionary<string, string> _tables = new Dictionary<string, string>();
    // Variable for JSNDrop message class to be used to process Database message respones to PUT/PUSH requests
    public JSNDropMessage _Message = new JSNDropMessage();
    // method adding table and connection ID to dictionary variable if didnt exist
    public void AddTable(string tblname, string tblcontext)
    {
        if (!_tables.ContainsKey(tblname))
            _tables.Add(tblname, tblcontext);
    }
    // Method to Push/Put data into the JSNdrop database
    private IEnumerator SendPut<T>(setResult<T> setter)
    {
        Debug.Log("SENT " + qStr); // Log whats sent
        UnityWebRequest webReq = UnityWebRequest.Get(qStr); // Create Web Request conatining put request string
        yield return webReq.Send(); // send the request and exit method remembering where upto in method for when done
        if (webReq.isNetworkError) // Network error
        {
            Debug.Log(webReq.error); //Log Error
        }
        else
        {
            // Show results as text because send was successful
            Debug.Log("Got TEXT " + webReq.downloadHandler.text);
            try // Error handle incase of errors
            {
                //Convert returned JSN data into message class object
                _Message = JsonUtility.FromJson<JSNDropMessage>(webReq.downloadHandler.text);
                if (_Message.Type == "INSERT" || _Message.Type == "UPDATE") // Confirm it is as expected and log
                {
                    Debug.Log(_Message.Type + " Successfull!");
                }
                List<T> theList = new List<T>(); // Create empty list for returning as requires List for return to work
                setter(theList);// Return to caller the list
            }
            catch(Exception) // Error?
            {
                Debug.Log("JSN Send Put Failed - ERROR!"); // Log
                List<T> theList = null; // Create Null List to return to caller
                setter(theList); // return to caller the list
            }
        }
    }
    // Method to process multiple database Rows returned into a list of each row returned
    private List<string> getJSNStrings(string pServer)
    {
        List<string> result = new List<string>(); // Make list to put in temp
        var parts = Regex.Split(pServer, "},{"); // Split multiple rows
        foreach (string part in parts) //Cycle the split rows
        {
            string newString = part.Replace("{", ""); // Replace front curly brackets from rows with nothing
            newString = "{" + newString.Replace("}", "") + "}"; // Put front and end Curly bracket, replace end bracket if any
            result.Add(newString); // Add newly formed row to temp list
        }
        return result; // Return temp list to caller
    }
    // Method for gettings data from JSNdrop database
    private IEnumerator SendGet<T>(setResult<T> setter)
    {
        Debug.Log("SENT " + qStr); // Log
        UnityWebRequest webReq = UnityWebRequest.Get(qStr); // Create Web request containing query string
        yield return webReq.Send(); // Send request and exit method remembering where upto in method for next call
        if (webReq.isNetworkError) // Error
        {
            Debug.Log(webReq.error); //Log Error
        }
        else
        {
            // Show results as text for Successful GET
            Debug.Log("Got TEXT " + webReq.downloadHandler.text); // Log
            List<T> theList; // Make Temp list with defualt of Null
            if (webReq.downloadHandler.text == "{}") // Check data returned is not empty if so then make sure list is null
            {
                theList = null;
            }
            // Check if data contains close and open curly bracket with comma menaing multiple rows returned
            else if (webReq.downloadHandler.text.Contains("},{") == true)
            {
                // If multiple rows the send to make lost of rows in the required method
                var jsnStrs = getJSNStrings(webReq.downloadHandler.text);
                theList = new List<T>(); // Change null list to be a new empty list
                foreach (string theItem in jsnStrs) // Cycle rows of data from server response that have be seperated
                {
                    T aDTO = JsonUtility.FromJson<T>(theItem); // Convert each list row from JSN to the Data DTO object
                    theList.Add(aDTO); // Add it to the List of the type of data object as requested
                }
            }
            else
            {
                // Else if not multiple rows of data returned treat as single row of data returned from Database
                //Make sure front and end curly brackets are singles not doubles
                string tmpFix = webReq.downloadHandler.text.Replace("{{", "{");
                tmpFix = tmpFix.Replace("}}", "}");
                // Convert the JSN to the DTO object accordingto its request DTO type
                T aDTO = JsonUtility.FromJson<T>(tmpFix);
                theList = new List<T>(); // Make the list new and empty ready to add
                theList.Add(aDTO); // Add the single row response to list
            }
            setter(theList); // Return the List wiehter its single orw, multiple rows, or null!
        }
    }

    // Send Reg to create table in database if not exist
    private IEnumerator SendReg<T>(string pTblName, setResult<T> setter)
    {
        Debug.Log("Sent Req: " + qStr); // log
        UnityWebRequest webReq = UnityWebRequest.Get(qStr); // create web request containing the request
        yield return webReq.Send(); // Send request and exit method remembering where upto for when co-routine calls again
        if (webReq.isNetworkError) // Network error
        {
            Debug.Log(webReq.error); // log
        }
        else
        {
            //Request success, Convert JSN back into Message class Object format
            _Message = JsonUtility.FromJson<JSNDropMessage>(webReq.downloadHandler.text);
            if (_Message.Message == "NEW" || _Message.Message == "EXISTS") // Check expected New or Exist Response
            {
                string connectionID = _Message.Type; // put into variable, not really needed but clearer to read
                // Keep track of tables by adding the tables name and connection ID returned from request to Dictionary
                if (!_tables.ContainsKey(pTblName))
                    _tables.Add(pTblName, connectionID);
            }
            else
            {
                Debug.Log("What just happened? This should never occur :("); // Should never happen but just incase
            }
            List<T> theList = new List<T>(); // Caller Expects reply so create empty list
            setter(theList); // Return to caller the empty list
        }
    }

    // Method to Register a table into the database called publically with set parameters for query string
    public void jsnReg<T>(string pGameName, string pTableName, string pPassword, setResult<T> aSetter)
    {
        // Form required Query string
        qStr = serverPathURL + "?cmd=jsnReg&value=" + pGameName + "," + pTableName + "," + pPassword;
        // Start Co-Routine threading for the Web request and provide the Setter call back for return response.
        StartCoroutine(SendReg<T>(pTableName, aSetter));
    }
    // Method to Get a table with optionaly Conditional Pattern, called publically with set parameters for query string
    public void jsnGet<T>(string pTableName, string pStrPattern, setResult<T> aSetter)
    {
        string tblConnection = _tables[pTableName]; // Get Tables connection ID from Dictionary of tables
        if (tblConnection != "") // Check not empty
        {
            // Form required Query string
            if (pStrPattern == "")
                qStr = serverPathURL + "?cmd=jsnGet&value=" + tblConnection; // If no pattern get all table data
            else
                qStr = serverPathURL + "?cmd=jsnGet&value=" + tblConnection + "," + pStrPattern;
            // Start Co-Routine threading for the Web request and provide the Setter call back for return response.
            StartCoroutine(SendGet<T>(aSetter));
        }
    }
    // Method to log all aSetter response returns and support the Co-Routines ASYNC abilities
    private List<T> aSetter<T>(List<T> pResult)
    {
        Debug.Log("aSetter List Result - " + pResult.ToString());
        return null;
    }
    // Method called publically to put/Push datainto the database using set parameters including a DTO object of data.
    public void jsnPut<T>(string pTableName, string pStrKey, T aDTO, setResult<T> aSetter)
    {
        string tblConnection = _tables[pTableName]; // Get Tables connection ID from Dictionary of tables
        if (tblConnection != "") // Check not empty
        {
            string jsnStr = JsonUtility.ToJson(aDTO); // Convert DTO object to Push/Put into a JSN string
            // Form the Push string containing the table ID, Key and String
            qStr = serverPathURL + "?cmd=jsnPut&value=" + tblConnection + "," + pStrKey + "," + jsnStr;
            // Start Co-Routine threading for the Web request and provide the Setter call back for return response.
            StartCoroutine(SendPut<T>(aSetter));
        }
    }
}
