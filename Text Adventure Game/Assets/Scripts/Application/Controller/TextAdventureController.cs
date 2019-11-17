using System.Collections.Generic;

// Text Adventure Controller forms the controller part of MVC and the games AMVCC design pattern
public class TextAdventureController {
    // declares the command processing class to be used for processing commands from player
    public processCommand commandProcessor = new processCommand();
    // declares the instance of a Database connection the game is going to keep using for database interaction
    public DataService dbConnection = new DataService();
    // declares instance of the MQTT server for chat in game and knowing players in each scene
    public MQTT gameServer;
    // Method to get all scenes from database and put into local list for command processing
    public void PopulateScenes()
    {
        // Clear Scenes List incase not empty
        GameManage.instance.app.model.lstScenes.Clear();
        dbConnection.JSNet.jsnGet<Scenes>("tblScene", "", GotScenes); // Get All scenes from database
    }

    private List<Scenes> GotScenes(List<Scenes> aDTO)
    {
        // loops all the scens returned from data base
        foreach (Scenes tmpScene in aDTO)
        {
            // Adds each scene to the list of them held locally in the model
            GameManage.instance.app.model.lstScenes.Add(tmpScene);
            if (tmpScene.Id == 1)
            {
                // Sets model data for current scene to be the start scene from story in database!
                GameManage.instance.app.model.currentScene = tmpScene;
            }
        }
        // Set the story output to show current scene story
        GameManage.instance.app.view.output.text = GameManage.instance.app.model.currentScene.sceneStory;
        // Set the help output to show cureent scene help
        GameManage.instance.app.view.help.text = GameManage.instance.app.controller.commandProcessor.ProcessHelp();
        // Connect player to first loaded scenes chat
        gameServer.connect(GameManage.instance.app.model.currentScene.sceneImageName);
        return aDTO;
    }
}
