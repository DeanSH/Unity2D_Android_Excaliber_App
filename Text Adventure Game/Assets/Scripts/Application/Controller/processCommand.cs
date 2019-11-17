using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
// A serializable class because its included and instantiated by a parent class, text adventure controller.
[Serializable]
//This Class handles the logical processing of game actions, manipulating of data and things to output on display
public class processCommand {

    public bool IsBlueStacks = false; // To know if playing in blue stacks or not
    public float zAxis = 0.0001f; // Default for android accelerometer but changed if bluestack detected

    // The parse method will process and handle errors for any command text input submitted for processing.
    public string Parse(string arg0) // arg0 is the command text passed over from the input text box in TextIO Scene.
    {
        try // Try catch here so the event of any error, it will catch and return do not understand message
        {
            string lcOutput = "Do not understand!\n\n"; // Default Output Message + Current Storyline added below
            // Now append the current scene story to the default do not understand message
            lcOutput = lcOutput + GameManage.instance.app.model.currentScene.sceneStory;
            // Now check if player input just a commmand number not text.. and set cmdText to the text command mapped for that number
            string cmdText = ConvertIfNumberCommand(arg0);
            cmdText = cmdText.ToLower(); // declares the input command into a new variable and Converted to lower case
            bool Redirect = false; //declares the Redirection variable in preparation for possible redirecting

            //Declares local temporary variable instances of certain classes required for command processing
            GetInventory lcInventory = new GetInventory(); // Used for inventory interactions
            GetItems lcItems = new GetItems(); // Used for scene Items checking and interactions
            // Chat command detection
            if (cmdText.Length >= 5) // check length for it having chat message
            {
                if (cmdText.Substring(0, 5) == "chat/") //check the first 5 characters are the chat command
                {
                    // Send message to MQTT server
                    GameManage.instance.app.controller.gameServer.SendMessage("CHAT", cmdText.Substring(5, cmdText.Length - 5));
                    return GameManage.instance.app.model.currentScene.sceneStory; // return story for game display
                }
            }

            Scenes lcScene = GameManage.instance.app.model.currentScene; // puts current scene into temporary local scene variable
            Redo: // The following for each loop jumps out to here in the event of redirection being true to reload the new redirection scene
            
            // loops through every story scene in the list of scenes created by The Story class instance.
            foreach (Scenes tmpScene in GameManage.instance.app.model.lstScenes)
            {
                // Check and identify if command text input contains the current scene being cycled throughs identifier
                if (cmdText.Contains(tmpScene.sceneIdentifier))
                {
                    // Check that current scenes help (available commands) contains the identifier for scene identified
                    // Or alternatively just continues if Redirect variable = True
                    if (lcScene.sceneHelp.Contains(tmpScene.sceneIdentifier) || Redirect == true)
                    {
                        // Checks the identified new scenes redirection item, if any, exist or not in players inventory
                        if (lcInventory.Exists(tmpScene.RedirectionItem) == false)
                        {
                            GameManage.instance.app.controller.gameServer.SendMessage("LEFT", "Selected " + tmpScene.sceneIdentifier.ToUpper() + " and Left Chat!");
                            // Add previous scene to history of scene list that keeps record of players route in game
                            GameManage.instance.app.model.lstSceneHistory.Add(lcScene.sceneIdentifier);
                            // If didnt exist in player inventory, set current scene to found identified scene
                            GameManage.instance.app.model.currentScene = tmpScene;
                            // Set temporary output variable and return the newly navigated scenes story
                            lcOutput = tmpScene.sceneStory;
                            //Increase players score each time scene changes
                            GameManage.instance.app.model.PlayerScore = GameManage.instance.app.model.PlayerScore + 100;
                            
                            GameManage.instance.app.controller.gameServer.connect(GameManage.instance.app.model.currentScene.sceneImageName);
                            return lcOutput; // Returns new current scenes story
                        }
                        else
                        {
                            // If redirection item for identified scene exited in players inventory set command to the
                            // Redirection scene identifier for the identified scene, then start loop process again.
                            cmdText = tmpScene.RedirectionScene.ToLower(); // Changed command to redirection scene command
                            Redirect = true; // lets us know its doing a redirect for when reloops
                            goto Redo; // goes back to top of loop to start loop again using new cmdtext
                        }
                    }
                }
            }

            // In the event command was not a navigational to a new scene, it might be a common scene command, processed here
            if (lcScene.sceneHelp.Contains(cmdText) == true) // Check Available command supports it, or if its the pickup command
            {
                switch (cmdText) // Select case switch statement to identify common scene commands
                {
                    case "exit": // Exit game case back to start scene
                        //Send left game and chat to the subscribed chat
                        GameManage.instance.app.controller.gameServer.SendMessage("LEFT", "Exited Game and Left Chat!");
                        GameManage.instance.app.controller.gameServer.UnSubscribeNow(); // unsubscribe
                        SceneManager.LoadScene("Start"); // load start scene
                        return "";
                    case "save": // Save game case
                        Save(); // Caller to a sub task for saving
                        lcOutput = "Game Saved!\n\n" + lcScene.sceneStory; // set Output variable = message + Scene Story
                        break; // Exit switch
                    case "load": // load game case
                        //Send left chat and loaded game to the subscribed chat
                        GameManage.instance.app.controller.gameServer.SendMessage("LEFT", "Loaded Game and Left Chat!");
                        Load(); // Caller to load sub task
                        lcScene = GameManage.instance.app.model.currentScene; // set local variable for scene
                        lcOutput = "Game Loaded!\n\n" + lcScene.sceneStory; // Same as previously described above.
                        //Connect to newly loaded scenes chat
                        GameManage.instance.app.controller.gameServer.connect(GameManage.instance.app.model.currentScene.sceneImageName);
                        break;
                    case "items": // show inventory items player has
                        lcOutput = lcInventory.Show(); // set Output variable to inventory data + story
                        break;
                    case "find": // find and show items for scene
                        lcOutput = lcItems.Show(); // set Output variable to scene items if any + story
                        break;
                    case "back": // Will change scene to the last scene identifier added to the scene history list
                        int LastSceneInt = GameManage.instance.app.model.lstSceneHistory.Count - 1; // gets last scene identifier position in list
                        string LastScene = GameManage.instance.app.model.lstSceneHistory[LastSceneInt]; // gets Last Scene
                        //Send left chat and went back to the subscribed chat
                        GameManage.instance.app.controller.gameServer.SendMessage("LEFT", "Selected BACK and Left Chat!");
                        // Remove the scene from history list since they going back to it
                        GameManage.instance.app.model.lstSceneHistory.RemoveAt(LastSceneInt);
                        //Set the scene being loaded back to by getting it from the locla list
                        foreach (Scenes tmpScenes in GameManage.instance.app.model.lstScenes)
                        {
                            // Finds matching Scene identifier and loads the scene its going back to
                            if(tmpScenes.sceneIdentifier == LastScene)
                            GameManage.instance.app.model.currentScene = tmpScenes;
                        }
                        // Anti cheat for score (Back and forward cheat) When player goes back lose 200 points!
                        GameManage.instance.app.model.PlayerScore = GameManage.instance.app.model.PlayerScore - 200;
                        lcOutput = GameManage.instance.app.model.currentScene.sceneStory; // set Output variable
                        //Join the chat for scene player went back too
                        GameManage.instance.app.controller.gameServer.connect(GameManage.instance.app.model.currentScene.sceneImageName);
                        break;
                    case "pickup":
                        lcOutput = lcInventory.ItemAdd(GameManage.instance.app.model.currentScene.Item); // add item to the players inventory list
                        break;
                }
            }
            return lcOutput; // Return whatever the output is now set to...
        }
        catch(Exception) // if any error just return the default do not understand message + story
        {
            string lcOutput = "Do not understand!\n\n";
            lcOutput = lcOutput + GameManage.instance.app.model.currentScene.sceneStory;
            return lcOutput;
        }

    }

    //Save the current state of the games Model Data class instance from the Game Manage app instance.
    public void Save()
    {
        // Declare and instantiate Binary formatter for Binary serialization of data
        BinaryFormatter bf = new BinaryFormatter();
        //Declare file stream and create it at path
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        //Write data from model to the File as serialized
        bf.Serialize(file, GameManage.instance.app.model);
        file.Close(); // Close file
    }

    //Load a previously saved state into the games Model Data class instance from the Game Manage app instance.
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")) // Check exists first
        {
            BinaryFormatter bf = new BinaryFormatter(); //Declare binary formatter
            // Open the file stream at location
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            // Set model data to be the file deserialized binary info
            GameManage.instance.app.model = (TextAdventureModel)bf.Deserialize(file);
            file.Close(); // close file
        }
    }

    // Method for parsing the help data for a scene into a text list of numbered commands for display
    public string ProcessHelp()
    {
        string tmpHelp = "Commands:"; // Declare string an preload with first line of text
        // Split the help text by comma's ready to loop through the array and number them
        string[] tmpCommands = GameManage.instance.app.model.currentScene.sceneHelp.Split(',');
        int tmpNum = 0; // Declare int variable for counting up and using the number to number commands
        foreach (string tmpString in tmpCommands) // loop the array of help commands
        {
            tmpNum = tmpNum + 1; // increase number by 1
            // Append to string variable the current string + number + the help string
            tmpHelp = tmpHelp + "\n" + tmpNum.ToString() + ": " + tmpString;
        }
        return tmpHelp; // finally return the string containing all help commands now numbered
    }

    // method to convert a number input command back into its text command string for then processing it
    private string ConvertIfNumberCommand(string cmd)
    {
        try // try but if any error then just return the command as was
        {
            // Check command is a number by trying to put it into a integer, will error and catch if not
            int tmpCheck = Convert.ToInt16(cmd);
            // if was a number command, we now split the help commands into an array
            string[] tmpCommands = GameManage.instance.app.model.currentScene.sceneHelp.Split(',');
            // We then get the help command text located exactly in the command number position - 1
            string actualCMD = tmpCommands[tmpCheck - 1];
            // We now have the exact text for that number command and return the command text
            return actualCMD;
        }
        catch (Exception)
        {
            return cmd; // return the originally passed in command
        }
    }
    //Method to handle accelerometer input usage to play game
    public void HandleAccelerometer(float xPos, float zPos)
    {
        //Split command available into strings
        string[] tmpCommands = GameManage.instance.app.model.currentScene.sceneHelp.Split(',');
        int tmpNum = -1; // Declare int variable for rotating
        int tmpNum2 = 0; // Declare int variable for rotating

        if (xPos > 0.15) // Right tilt detected
        {
            tmpNum2 = tmpCommands.Length - 1; //Get Count of commands
            foreach (string tmpString in tmpCommands) // loop the array of help commands
            {
                tmpNum = tmpNum + 1; // increase number by 1 as we find a match
                if (GameManage.instance.app.view.input.text == tmpString) // If command currently there matches goto next
                {
                    if (tmpNum == tmpNum2) // If reached last command go back to first command as next command
                        tmpNum = 0;
                    else
                        tmpNum += 1; // goto next command
                    GameManage.instance.app.view.input.text = tmpCommands[tmpNum]; // Put command into input textbox
                    return;
                }
            }
            GameManage.instance.app.view.input.text = tmpCommands[0]; // If Not Match then start at first command
            return;
        }
        else if (xPos < -0.15) // left tilt detected
        {
            
            tmpNum2 = tmpCommands.Length - 1; // Get Count of commands
            foreach (string tmpString in tmpCommands) // loop the array of help commands
            {
                tmpNum = tmpNum + 1; // increase number by 1
                if (GameManage.instance.app.view.input.text == tmpString) // If command currently there matches goto next
                {
                    if (tmpNum == 0)  // If reached first command go to last command as next command
                        tmpNum = tmpNum2;
                    else
                        tmpNum -= 1; // Goto previous command
                    GameManage.instance.app.view.input.text = tmpCommands[tmpNum];  // Put command into input textbox
                    return;
                }
            }
            tmpNum = tmpNum2; // Set to last command
            GameManage.instance.app.view.input.text = tmpCommands[tmpNum]; // If no match start with last command
            return;
        }

        if (zPos > zAxis && zPos != 0) // forward tilt detection
        {
            // Parse command then update scene output
            GameManage.instance.app.view.output.text = GameManage.instance.app.controller.commandProcessor.Parse(GameManage.instance.app.view.input.text);
            // Update help output to current scenes Help
            GameManage.instance.app.view.help.text = GameManage.instance.app.controller.commandProcessor.ProcessHelp();
            // Update score output to show players current score
            GameManage.instance.app.view.score.text = "Score: " + GameManage.instance.app.model.PlayerScore.ToString();

            // reset input box
            GameManage.instance.app.view.input.text = "";
        }
    }
}
