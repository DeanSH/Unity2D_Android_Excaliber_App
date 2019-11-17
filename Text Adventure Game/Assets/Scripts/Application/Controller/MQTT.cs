using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;
//Class used to preform MQTT IOT functionality for game Chat and In Scene Players
public class MQTT : MonoBehaviour {
    //Variables for Game Chat and Scene players and connection to MQTT Server
	private MqttClient client; // Connection to MQQT Server 
    private string PlayersName; // Store name of player playing the game
	private string mqtt; // Store all chat text for chat window diplay here
    private string chatusers; // store the list of all players in scene here
    private ScrollRect Scroller; // Link to accessand interact with chat boxes scroll bar here
    private string SceneID = ""; // Store current scenes Identifier here for what chat to subscribe to
    private bool IgnoreFirstMsg = false; // Recognize if in chat and got first MQTT response yet.
    private bool scrolldown = false; //Used to know if new chat arrived and needs to scroll down
	// Public method to call connecting to a scenes chat, parameter passed to identify the scene chat to join
	public void connect (string prSceneID) {

        if (SceneID != "") // If not blank then already in a chat room so unsubscribe to it first
        {
            client.Unsubscribe(new string[] { "Excalibur/" + SceneID.ToString() });
        }
        else // else if was blank then its the first time joining chat since starting game so get scroller GUI access
        {
            Scroller = GameManage.instance.app.view.scroll.GetComponent<ScrollRect>();
        }

        PlayersName = PlayerPrefs.GetString("UserName");// store players name
        IgnoreFirstMsg = false; // Set back to false for joining new chat
        chatusers = "\n" + PlayersName + "\n"; // Clear chat users list and input players own name in first position
        mqtt = "\nJoining chat as " + PlayersName + "..\n"; // Clear Chat text history and state player is joining
        SceneID = prSceneID; // Update scene ID to the identifier passed in at method parameters
		// create client instance pointing to MQTT server
		client = new MqttClient("newsimland.com", 443 , false , null ); 

		// register to message received, making is call the specified method whenever responses arrive
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
        // create new GUID string and then connect to MQTT server using it
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId);

        // subscribe to the chat topic with QoS 2, and using the SceneID stored for current chat to join
        client.Subscribe(new string[] { "Excalibur/" + SceneID }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        // Notify self and others that Player has joined calling the send message mthod with the seen parameters
        SendMessage("JOIN", "Joined Chat!");
    }

    //Methid called whenever MQTT server messages are recieved
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string json = System.Text.Encoding.UTF8.GetString(e.Message); // Get the string from server response into string
        Debug.Log("Recieved: " + json); // log
        if (IgnoreFirstMsg == false) // Check if ignore first message is still false if so do code and exit method
        {
            mqtt = mqtt + "You are now chatting in: " + SceneID.ToString() + "\n"; // append to chat that you joined
            IgnoreFirstMsg = true; // Change to true
            SendMessage("HERE", "Im Here!"); // Broadcast to chat that you are here which makes you show up in others lists
            return;
        }
            //Convert the JSN response into the custom Message Class structure for chat (ID,Name,Message)
            messaging TheObject = JsonUtility.FromJson<messaging>(json);
            switch (TheObject.ID) // Identify what case we are handling by the Objects ID
            {
                case "CHAT": // Got Chat message / Picked Up item message
                    mqtt = mqtt + TheObject.Name + ": " + TheObject.Message + "\n"; // Append new Chat to existing chat
                    break;
                case "JOIN": // Got Joined message telling you Someone joined
                    SendMessage("HERE", "Im Here!"); // Reply when someone joins saying Here to add yourself to their list
                    if (TheObject.Name == PlayersName) { return; } // Dont post in chat user joined if its yourself
                    mqtt = mqtt + TheObject.Name + ": " + TheObject.Message + "\n"; //append joined message to chat
                    break;
                case "HERE": // Cse to add name to players list if not there yet
                    if (chatusers.Contains(TheObject.Name) == false) // is or isnt there already
                        chatusers = chatusers + TheObject.Name + "\n"; // Wasnt there so append it to the players list
                    return;
                case "LEFT": // Case when user leaves chat
                    if (TheObject.Name == PlayersName) { return; } // Dont process if its yourself
                    mqtt = mqtt + TheObject.Name + ": " + TheObject.Message + "\n"; // Append to chat the left message
                    chatusers = chatusers.Replace(TheObject.Name + "\n", ""); // Replace player in list with blank
                    break;
            }

        scrolldown = true; // Set to true so chat will scroll to bottom once
    } 

    // Method to send message with Case and message parameters, no Player Name parameter needed as stored already
	public void SendMessage(string prCase, string prMessage){
            messaging TheObject = new messaging(); // create message Object then put values into object
            TheObject.ID = prCase;
            TheObject.Name = PlayersName;
            TheObject.Message = prMessage;
            string json = JsonUtility.ToJson(TheObject); // Convert object into JSN string for sending to server
            Debug.Log("Sent: " + json); // log
            // Publish the message being sent to the chat MQTT server
            client.Publish ("Excalibur/" + SceneID, System.Text.Encoding.UTF8.GetBytes (json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
	}

    // Publically callabale method to Unsubscribe from whatever currently subscribed to, used for leaving game
    public void UnSubscribeNow()
    {
        client.Unsubscribe(new string[] { "Excalibur/" + SceneID });
    }
	// Update is called once per frame
	void Update () {
		GameManage.instance.app.view.chat.text = mqtt; // set Chat display
        GameManage.instance.app.view.users.text = chatusers; // set Players in chat display
        // check if scrolldown true, it means new chat arrived and chat window needs to scroll to bottom
        // Only do one time when chat arrives not once per frame, otherwise palyer can never scroll up chat history
        if (scrolldown == true)
        {
            scrolldown = false; // set back to false so it only scrolls to bottom once, not once per frame
            try // This sometimes error's maybe when not enough content to scroll for example so error handling needed
            {
                Scroller.verticalScrollbar.value = 0f; // Set chat scrool bar position value to 0f meaning bottom!
            }
            catch (Exception)
            {
                Debug.Log("Cannot Assign Value To Scrollbar Currently."); // log
            }
        }
    }
}

//Class containing the message object structure
public class messaging
    {
    public string ID = "";
    public string Name = "";
    public string Message = "";
    }