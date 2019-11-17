using System.Collections.Generic;
using System;

// Contains all models data related to the app.
[Serializable] // to support the data model being savable and loadable for milestone 2
public class TextAdventureModel
{
    // Data Structures that the model needs to store
    public int PlayerScore = 0; // Players in game score is stored here
    public List<Scenes> lstScenes = new List<Scenes>(); // list of scenes to be used by command processor
    public Scenes currentScene; // The current scene
    public List<string> lstSceneHistory = new List<string>(); // List of Command history or scene history, logging the path player takes through game, and used for going back from scenes to previous scenes which removes from list if go back
    public List<string> lstInventory = new List<string>(); // players inventory list storing all items picked up
}
