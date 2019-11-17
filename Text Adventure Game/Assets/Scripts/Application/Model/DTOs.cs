using System;

// This DTO cs file holds multiple class's that form the database table structures
[Serializable]
// Playe Table Structure
public class Player  {

    public int Id; // The int number ID for any player account added to the database
    public string Name; // Players name always added in lowercase to Database
    public string Password; // Players password
    public string Email; // Players email
    public int Online; // Players online status
    public string LastLogin; // Last time player logged in or out

}

[Serializable]
//Table structure for the Story scenes
public class Scenes
{
    public int Id;
    // Data for a Scene
    public string sceneIdentifier; // This is a unique command which is used to know what scene a player wants to goto! Its keywords such as 'go left' that a player types in there command
    public string sceneStory; // This is the Story to output and display for a given Scene, simple!
    public string Item; // This is an Array that will hold 1 or none or all Items for a scene that a player can pickup
    public string sceneHelp; // This string will hold all available commands for navigating in or away from this scene, to help players know how they can navigate scenes. NOTE: if scene they type command to goto is not in the help, it will not understand their command!
    public string sceneImageName; // This is a string text for identifying the image to load according to the images name in the resources 'Images' folder of the unity assests (Must match an existing image name) 
    // Data for Scene Identifier to goto if Alt Identifier Item exist in players Inventory (Redirect Them)
    public string RedirectionScene; // The Scene to redirect too
    public string RedirectionItem; // The Item to exist in players inventory to trigger scene redirection   

}
