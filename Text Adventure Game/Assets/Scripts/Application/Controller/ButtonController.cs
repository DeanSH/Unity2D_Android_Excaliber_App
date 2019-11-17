using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
// This class processes clicks from the button handler class that is re-usable across multiple buttons in the game.
public class ButtonController {

    // Declare connection to JSN database
    public DataService dbConnection = new DataService();
    private string GetData = "";
    private string PutData = "";

    // Method to process a button click from a button listner, passed a button identifier command
    public void BtnProcess(string prCmd)
    {
        dbConnection.AddDBTables();
        switch (prCmd) // Select case switch statement to compare string passed into the menthod and do button sub task.
        {
            case "StartGame": // Identifiers for each seen as seen for each in this switch select case
                StartGame(); // Methods called to for each case, repeats for each case below same style
                break;
            case "ExitGame":
                ExitGame();
                break;
            case "QuitGame":
                // If player leaves game, send message to chat server and unsubscribe from channel
                GameManage.instance.app.controller.gameServer.SendMessage("LEFT", "Quit Game and Left Chat!");
                GameManage.instance.app.controller.gameServer.UnSubscribeNow();
                QuitGame();
                break;
            case "SetupGame":
                SetupGame();
                break;
            case "ExitSetup":
                ExitSetup();
                break;
            case "LoginUser":
                LoginUser();
                break;
            case "Logout":
                Logout();
                break;
            case "ShowReg":
                ShowReg();
                break;
            case "CreateUser":
                CreateUser();
                break;
            case "ShowLogin":
                ShowLogin();
                break;
        }
    }


    #region scene changers and basic button methods
    // Below is all the method sub tasks to do stuff according to which button got clicked.
    void StartGame()
    {
        SceneManager.LoadScene("Game"); // Load other scene
    }

    void SetupGame()
    {
        SceneManager.LoadScene("Setup"); // load other scene
    }

    void ExitSetup()
    {
        //Save players settings, First connect temp variables to GUI elements on setup scene
        Toggle tmpMusic = GameObject.Find("ToggleMusic").GetComponent<Toggle>();
        Toggle tmpAccel = GameObject.Find("ToggleAccel").GetComponent<Toggle>();
        // Set Player Prefs to 0 for not ticked as defaults
        PlayerPrefs.SetInt("Music", 0);
        PlayerPrefs.SetInt("Accel", 0);
        // now check if the options where ticked and if so change playerprefs from default 0 to ON value 1
        if (tmpMusic.isOn == true)
            PlayerPrefs.SetInt("Music", 1);
        if (tmpAccel.isOn == true)
            PlayerPrefs.SetInt("Accel", 1);
        PlayerPrefs.Save(); // Save player prefs.
        QuitGame(); // reuse the quitgame method further below
    }

    void ExitGame()
    {
        Application.Quit(); // close down the application
    }

    void QuitGame()
    {
        SceneManager.LoadScene("Start"); // load another scee
    }

    void ShowReg()
    {
        SceneManager.LoadScene("Register"); // load another scene
    }

    void ShowLogin()
    {
        SceneManager.LoadScene("Login"); // oad another scene
    }

    #endregion


    #region Methods for processing Login, Logout and Create Account with JSN calls
    void LoginUser() // Login user method
    {
        // Get login screen GUI elements into variables for interaction, Name / Password / Status
        InputField tmpName = GameObject.Find("InputUserName").GetComponent<InputField>();
        InputField tmpPW = GameObject.Find("InputPassword").GetComponent<InputField>();
        Text tmpStatus = GameObject.Find("txtStatus").GetComponent<Text>();
        // Set status text to logging in...
        tmpStatus.text = "Status: Logging In..";
        GetData = "Check Login"; // Set GetData variable for response type to process in the reusable Get method
        // Check if player name and password exist together in JSNDrop database
        dbConnection.JSNet.jsnGet<Player>("tblPlayer", "\"Name\":\"" + tmpName.text.ToLower() + "\",\"Password\":\"" + tmpPW.text + "\"", ProcessGet);
    }

    // Loout method for the start screen to logout back to login screen
    void Logout()
    {
        // get the persistant stored in local player prefs 'name' for player who logged in
        string UserName = PlayerPrefs.GetString("UserName");
        GetData = "Set Offline"; // Set GetData variable for response type to process in the reusable Get method
        //  Set players online status to 0 for offline in the data bases player table by first getting player info
        dbConnection.JSNet.jsnGet<Player>("tblPlayer", "\"Name\":\"" + UserName + "\"", ProcessGet);
    }

    // register new account method for register screen of game
    void CreateUser()
    {
        // Declare and link to GUI elements for register screen, name / password / email / status
        InputField tmpName = GameObject.Find("InputUserName").GetComponent<InputField>();
        InputField tmpPW = GameObject.Find("InputPassword").GetComponent<InputField>();
        InputField tmpEmail = GameObject.Find("InputEmail").GetComponent<InputField>();
        Text tmpStatus = GameObject.Find("txtStatus").GetComponent<Text>();
        // Set Status to creating account
        tmpStatus.text = "Status: Creating Account..";
        // call method to check credentials are in the allowed boundaries (Validation)
        if (CheckCredentials(tmpName.text.ToLower(), tmpPW.text, tmpEmail.text) == true)
        {
            GetData = "Check Exist"; // Set variable for get method identifying and method reusability
            // Now contact database to get players information and check they exist or not before creating
            dbConnection.JSNet.jsnGet<Player>("tblPlayer", "\"Name\":\"" + tmpName.text.ToLower() + "\"", ProcessGet);
        }
        else
        {
            // Set Status to bad credentials
            tmpStatus.text = "Status: Error in Name,Password or Email!";
        }
    }

    // Method to validate if aplayers inputs for register are valid
    private bool CheckCredentials(string tmpName, string tmpPW, string tmpEmail)
    {
        if (tmpName.Length < 2 || tmpPW.Length < 2 || tmpEmail.Length < 9)
            return false; // returns false if lengths of any input is below the minimum

        if (tmpEmail.Contains("@") == false)
            return false; // returns false if the email does not contain the @ symbol

        if (tmpEmail.Contains(".") == false)
            return false; // returns false if the email does not contain the . symbol

        return true; // return true if inputs passed the validation check!
    }

    #endregion


    #region Process Get and put JSN responses

    // Single reusable method for processing various types of Get Requests from databse
    private List<Player> ProcessGet(List<Player> aDTO)
    {
        // Declare and link to GUI elements for register/login screens, name / password / status
        InputField tmpName;
        InputField tmpPW;
        Text tmpStatus;
        // Check which get method we are expecting datafor and process accoridngly
        switch (GetData)
        {
            // Case for checking user exists before creating
            case "Check Exist":
                if (aDTO == null)
                {
                    // username didnt exist now get list of all user accounts for purpose of counting them
                    // To get the next number for use as the new player accounts primary key ID
                    GetData = "Next ID";
                    dbConnection.JSNet.jsnGet<Player>("tblPlayer", "", ProcessGet);
                }
                else
                {
                    //user existed
                    tmpStatus = GameObject.Find("txtStatus").GetComponent<Text>();
                    tmpStatus.text = "Status: Account already exists!"; // If not null then existed
                }
                break;
            // Case for creating new account where we get all users back from DB to count how many and get new users ID
            case "Next ID":
                // Declare and link to GUI element for register screen, email
                tmpName = GameObject.Find("InputUserName").GetComponent<InputField>();
                tmpPW = GameObject.Find("InputPassword").GetComponent<InputField>();
                InputField tmpEmail = GameObject.Find("InputEmail").GetComponent<InputField>();
                // Set new user accounts primary key ID to be the userstable count plus 1
                int tmpID = aDTO.Count + 1;
                Debug.Log("Making Account with ID: " + tmpID.ToString()); //Show in log
                // create the player into a Player DTO class object ready to insert to databse
                Player tmpPlayer = new Player();
                tmpPlayer.Id = tmpID; // Primary key ID
                tmpPlayer.Name = tmpName.text.ToLower(); // Name
                tmpPlayer.Password = tmpPW.text; // Password
                tmpPlayer.Email = tmpEmail.text; // Email
                tmpPlayer.Online = 0; // Online status 0 = offline
                tmpPlayer.LastLogin = DateTime.Now.ToString("h:mm:ss tt"); // Last Time player interacted with DB
                // Set PUT data identifier for PUT data arrival reusable processing similar to get data processing
                PutData = "Made Account";
                // Push the player object to the database adding the new player
                dbConnection.JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpID.ToString(), tmpPlayer, ProcessPut);
                break;
            // Case for when user is logging in and we check if account with certain name and password existed or not
            case "Check Login":
                if (aDTO == null)
                {
                    // Set status for failed login
                    tmpStatus = GameObject.Find("txtStatus").GetComponent<Text>(); // get GUI component to set status
                    tmpStatus.text = "Status: Wrong Username/Password!";
                }
                else
                {
                    // If not null then player exists and we log them in successfully
                    var tmpUser = aDTO[0]; // Assign players data to variable
                    // Incase Music and Accelerometer values dont exist yet in player prefs we add them for first time!
                    if (PlayerPrefs.GetInt("Music", 2) == 2)
                        PlayerPrefs.SetInt("Music", 1); // Add player pref
                    if (PlayerPrefs.GetInt("Accel", 2) == 2)
                        PlayerPrefs.SetInt("Accel", 1); // add player pref
                    PlayerPrefs.SetString("UserName", tmpUser.Name); // save local and persistent who they logged in as
                    PlayerPrefs.Save(); // Save it.. making possibly to load username nad use it at other scenes
                    // Set Online for player table data to 1 for online, and update last online DB interaction time/date
                    tmpUser.Online = 1;
                    tmpUser.LastLogin = DateTime.Now.ToString("h:mm:ss tt");
                    PutData = "Set Online"; // Set Put resonse type
                    // Push new player DB state to database replacing existing data for play, so they show as online in DB
                    dbConnection.JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpUser.Id.ToString(), tmpUser, ProcessPut); // Set player online status to 1 for online in database
                }
                break;
            // Case for when setting player to offline when logout
            case "Set Offline":
                if (aDTO == null)
                {
                    // If account not in DB then we cant change the Online status
                    Debug.Log("Failed to find player Data to set to Offline! Shouldnt happen..");
                }
                else
                {
                    // If not null then player exists and we log them off successfully
                    // Change players state locally into DTO object
                    var tmpUser = aDTO[0];
                    tmpUser.Online = 0;
                    tmpUser.LastLogin = DateTime.Now.ToString("h:mm:ss tt");
                    PutData = "Set Offline"; // Set PUT identifier
                    // Push new players DB state to database for now offline
                    dbConnection.JSNet.jsnPut<Player>("tblPlayer", "\"Id\":" + tmpUser.Id.ToString(), tmpUser, ProcessPut); // Set player online status to 1 for online in database
                }
                break;
        }
        return aDTO;
    }

    // Reusable method via PUTdata identifier for when pushing data to DB
    private List<Player> ProcessPut(List<Player> aDTO)
    {
        // Get login screen GUI elements into variables for interaction, Status
        Text tmpStatus;
        // Check what identifier is to process push action accordingly
        switch (PutData)
        {
            //case where account was made successfully or not
            case "Made Account":
                tmpStatus = GameObject.Find("txtStatus").GetComponent<Text>();
                if (aDTO == null)
                {
                    // Fail safe for if some reason we got two nulls coming back.. possible means connection issue
                    tmpStatus.text = "Status: Failed to create account!";
                }
                else
                {
                    tmpStatus.text = "Status: Account creation successful!"; // if not null then was created
                }
                break;
            //Case where users online state was set to online now we take them to start scene
            case "Set Online":
                if (aDTO == null)
                {
                    Debug.Log("Failed To Set Online Status! Should never occur");
                }
                else
                {
                    // Successfully Logged in now show start game screen
                    SceneManager.LoadScene("Start"); // load the start scene
                }
                break;
            //Case where user online state set to offline and take them to login scene
            case "Set Offline":
                if (aDTO == null)
                {
                    Debug.Log("Failed To Set Offline Status! Should never occur");
                }
                else
                {
                    // Successfully Logged out now show login screen
                    SceneManager.LoadScene("Login"); // load the login scene
                }
                break;
        }
        return aDTO;
    }
    #endregion
}
