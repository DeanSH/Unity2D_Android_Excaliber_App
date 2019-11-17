using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class handles things for the GUI on the start screen most importantly shwoing online players list
public class StartScreen : MonoBehaviour {
    public Text TheList; // Public variable for the list text on scroll view UI element for start screen
    public JSNDrop TheJSNDrop; // Variable containg the Database JSNDrop game object
    DataService dbConnection = new DataService(); // Declare and instantiate DataService class

    // Method called when this class instantiates
    void Start () {
        // Setup connection to database
        dbConnection.JSNet = TheJSNDrop; // Pass JSNdrop object to the local one in this class
        dbConnection.AddDBTables(); // Populate the DB tables Dictionary
        // Gets all online players from database
        dbConnection.JSNet.jsnGet<Player>("tblPlayer", "\"Online\":1", GotOnlinePlayers);
    }
    // Method called in response to get players
    private List<Player> GotOnlinePlayers(List<Player> aDTO)
    {
        // Declare string to hold the list of player in each on a new line in string with \n
        string tmpTheList = "";
        if (aDTO != null && aDTO.Count > 0)
        {
            // Loop through the returned list from database of online players to get names, maybe highscores etc..
            foreach (Player tmpPlayer in aDTO)
            {
                // Check if the tring is blank or not to detect if adding on new line or not yet..
                if (tmpTheList == "")
                {
                    tmpTheList = "Online Players: " + aDTO.Count.ToString() + "\n-----\n" + tmpPlayer.Name; // add first player in online list to string
                }
                else
                {
                    tmpTheList = tmpTheList + "\n" + tmpPlayer.Name; // after first player , other players added appended on new line each
                }
            }
            TheList.text = tmpTheList + "\n"; // Set the start screen list text to be the string just created, showing it to player!
        }
        return aDTO;
    }
}
