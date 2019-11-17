
using System.Collections.Generic;
using System;
using UnityEngine;

// Data service class for connection to database, creating database, building database, interaction with DB
public class DataService  {

    public JSNDrop JSNet; // Variable containing access to the JSNDrop gameobject
    public DataService(){}
    // Method to add tables to local list of tables, reading past connection IDs from player prefs
    public void AddDBTables()
    {
        string tmpID = PlayerPrefs.GetString("tblPlayer"); // Get table connection ID from Prefs
        JSNet.AddTable("tblPlayer", tmpID); // Add to local dictionary of table connection IDs
        tmpID = PlayerPrefs.GetString("tblScene"); // Get table connection ID from Prefs
        JSNet.AddTable("tblScene", tmpID); // Add to local dictionary of table connection IDs
    }

    public void CheckDBExists()
    { // Checks if database exist, creates it if didnt and builds
        MakeTables();
    }

    // If any errors start coming from database change the Name and Password in MAKETABLES then start game from login scene
    // And it will create tables fresh on JSNDrop and populate the database freshly. EG.. Excalibur_12345 / tblPlayer_12345
    #region Make Tables If Not Exist after Check Exist
    // Build/Reg tables method, If exist returns Exist otherwise returns NEW
    private void MakeTables()
    {
        // Calls to reg the tables if not exist, and returns to MadePlayerTable or MadeSceneTable
        JSNet.jsnReg<Player>("Excalibur_1234", "tblPlayer", "tblPlayer_1234", MadePlayerTable);
        JSNet.jsnReg<Scenes>("Excalibur_1234", "tblScene", "tblScene_1234", MadeSceneTable);
    }
    // The retrun method called when finished to Reg new or existing Player table to JSN Drop Db
    private List<Player> MadePlayerTable(List<Player> aDTO)
    {
        Debug.Log("Got: " + JSNet._Message.Message + " " + JSNet._Message.Type);
        if (JSNet._Message.Message == "NEW") // New means table didnt exist and is new!
        {
            PlayerPrefs.SetString("tblPlayer", JSNet._tables["tblPlayer"]); // save local and persistent connection ID
            PlayerPrefs.Save(); // Save it.. making possibly to load table connection IDs on the fly throughout screens
            //Call next method to add first Test Player accounts in JSNDrop
            InsertTestAccounts();
        }
        else if (JSNet._Message.Message == "EXISTS") // Table already exits
        {
            PlayerPrefs.SetString("tblPlayer", JSNet._tables["tblPlayer"]); // save local and persistent connection ID
            PlayerPrefs.Save(); // Save it.. making possibly to load throughout game screens
        }
        return aDTO;
    }
    // The retrun method called when finished to Reg new or existing Scene table to JSN Drop Db
    private List<Scenes> MadeSceneTable(List<Scenes> aDTO)
    {
        Debug.Log("Got: " + JSNet._Message.Message + " " + JSNet._Message.Type);
        if (JSNet._Message.Message == "NEW") // New means table didnt exist and is new!
        {
            PlayerPrefs.SetString("tblScene", JSNet._tables["tblScene"]); // save local and persistent connection ID
            PlayerPrefs.Save(); // Save it.. making possibly to load Connection ID at any screen faster
            //Call next method to add Story to scenes in JSNDrop
            InsertGameStory();
        }
        else if (JSNet._Message.Message == "EXISTS") // Table already exists
        {
            PlayerPrefs.SetString("tblScene", JSNet._tables["tblScene"]); // save local and persistent connection ID
            PlayerPrefs.Save(); // Save it.. making possibly to load Connection ID through any screens faster
        }
        return aDTO;
    }
    // Method to insert test accounts for testing database
    public void InsertTestAccounts()
    {
        // Create 4 players with specified ID number, name, password, email, online status default 0 offline
        // and the last login time, or logout time also as updated when player logs out too.
        Player tmpPlayer; // Temp variable to put test player account information into then push to DB
        // populate test account data
        tmpPlayer = new Player {
            Id = 1,
            Name = "admin",
            Password = "123456",
            Email = "r4.productionz@hotmail.com",
            Online = 0,
            LastLogin = DateTime.Now.ToString("h:mm:ss tt")
        };
        //Push player object to the database and then return to the AddedTestAccount Method
        JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpPlayer.Id.ToString(), tmpPlayer, AddedTestAccount);

        // Repeats same as above another 3 times for more test accounts
        tmpPlayer = new Player
        {
            Id = 2,
            Name = "dean",
            Password = "12345",
            Email = "r4.productionz@hotmail.com",
            Online = 0,
            LastLogin = DateTime.Now.ToString("h:mm:ss tt")
        };
        JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpPlayer.Id.ToString(), tmpPlayer, AddedTestAccount);

        tmpPlayer = new Player {
                Id = 3,
                Name = "todd",
                Password = "12345",
                Email = "Todd-Cocrane@live.nmit.ac.nz",
                Online = 0,
                LastLogin = DateTime.Now.ToString("h:mm:ss tt")
        };
        JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpPlayer.Id.ToString(), tmpPlayer, AddedTestAccount);

        tmpPlayer = new Player {
                Id = 4,
                Name = "arno",
                Password = "12345",
                Email = "Arno-Grupp@live.nmit.ac.nz",
                Online = 0,
                LastLogin = DateTime.Now.ToString("h:mm:ss tt")
        };
        JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpPlayer.Id.ToString(), tmpPlayer, AddedTestAccount);

    }
    // Method returned to after adding test accounts what just tells us at debug log that it was successfuly
    // adding test accounts only occurs once, when game sets up the non existing databse for first time ever.
    private List<Player> AddedTestAccount(List<Player> aDTO)
    {
        Debug.Log("Test Account Added to Player Table = " + JSNet._Message.Message + " : " + JSNet._Message.Type);
        return aDTO;
    }

    // method to insert the game story into database
    public void InsertGameStory()
    {
        // Create temporary instance of the gaem story class conatining all data needed
        TheStory gameStory = new TheStory(); // The story Builder
        gameStory.BuildStory(); // Tell it to build the story into its temp list it 

        // lopp the game story temp list of scenes and insert each to the data base one by one
        foreach (Scenes tmpScene in gameStory.lstScenes)
        {
            JSNet.jsnPut<Scenes>("tblScene", "\"Id\":" + tmpScene.Id.ToString(), tmpScene, AddedScene); // insert a single scene
        }
    }
    // Return method after adding each single scene to database, just tells us in debug log that it was added
    // adding story scenes only occurs once, when game sets up the non existing databse for first time ever.
    private List<Scenes> AddedScene(List<Scenes> aDTO)
    {
        if (aDTO != null)
            Debug.Log("Scene Added to Scene Table = " + JSNet._Message.Message + " : " + JSNet._Message.Type);
        return aDTO;
    }

#endregion


}
// Class for use with handling JSNdrop return messages
public class JSNDropMessage
{
    public string Message;
    public string Type;
}
