using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
// Base class for managing all elements in the game play scene.
[Serializable] // Serializable to support the local game Save Load feature available
public class GameManage : MonoBehaviour
{
    // Creates a Static Singleton style instance of this class itself.
    public static GameManage instance;
    // Gives access to the application and the instances of model view controller.
    public TextAdventureApp app = new TextAdventureApp();

    public AudioSource TheMusic; // Variable to give access to the Audio game object for music
    public JSNDrop TheJSNDrop; // Provides access to the JSNDrop game object for getting game scenes
    public InputField TheInput; // Provides access to the game text input object for text based game play
    public MQTT TheMQTT; // Provides access to the MQTT game object for game scene players and chat
    public GameObject TheScroll; //Provides access to the chat text scroll bar for programatically pushing it to 

    //Creates or Destroys self instance, and Assigns GUI game objects to the view class, updates display and starts input listner
    void Awake()
    {
        if (instance == null) // If null creates the instance, otherwise destorys instance.
        {
            instance = this; // Assigns this static class instance to itself to be singleton
                             // Model / View / Contorller instantiate when the static instance forms the App variable
            
            // Code to keep screen awake while game is running!
            Screen.sleepTimeout = (int)0f;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Check if players settings is allowing game music and 0 for NO, mutes the Audio Source
            if (PlayerPrefs.GetInt("Music", 1) == 0)
                TheMusic.mute = true;

            // Assign GUI elements and store them into element variables found in the view class (Where they belong)!
            app.view.input = TheInput; // Player input
            app.view.scroll = TheScroll; // Chat scroll
            app.view.output = GameObject.Find("SceneOutput").GetComponent<Text>(); // Story, etc output
            app.view.help = GameObject.Find("SceneHelp").GetComponent<Text>(); // help output
            app.view.score = GameObject.Find("ScoreOutput").GetComponent<Text>(); // score output
            app.view.chat = GameObject.Find("ChatOutput").GetComponent<Text>(); // chat output
            app.view.users = GameObject.Find("txtScroll").GetComponent<Text>(); //chat users output
            // call the method which links the input field submit to a listner
            app.view.LinkInputSubmit();
            // pass the jSNdrop object to the controllers variable for it where it belongs
            app.controller.dbConnection.JSNet = TheJSNDrop;
            // pass the MQTT object to the controllers variable for it where it belongs
            app.controller.gameServer = TheMQTT;
            // Tell the controller to Verify DB tables exist on JSNdrop and add them if they dont
            app.controller.dbConnection.AddDBTables();
            // Calls method in controller to get all Scenes from Database into a local list
            //to use with command processing and then loads first scene!
            app.controller.PopulateScenes();

        }
        else
        {
            Destroy(gameObject); // Destroys instance if already existed somehow
        }

    }

}
