

// The class for getting inventory to show, check exists, or add item to inventory
public class GetInventory {
    // Shows the players current Inventory into a string
    public string Show()
    {
        string lcInventory = "You have no Items!\n\n" + GameManage.instance.app.model.currentScene.sceneStory;
        // Checks if Inventory list of player is empyty and returns message to say so if is.. and exits code
        if (GameManage.instance.app.model.lstInventory.Count == 0)
            return lcInventory;

        // Make temporary string to store inventory into then cycle the inventory list making a string to return it all!
        lcInventory = "Your Items:";
        foreach (string Value in GameManage.instance.app.model.lstInventory)
        {
            lcInventory = lcInventory + ", " + Value; // adds each single inventory items to the string on a new line \n
        }
        lcInventory = lcInventory.Replace("Your Items:,", "Your Items:\n");
        return lcInventory + "\n\n" + GameManage.instance.app.model.currentScene.sceneStory; // returns the inventory list string + Story
    }

    // Use this for checking item exists in players inventory (seen used in GetItems class) returning true or false
    public bool Exists(string Name)
    {
        // loops the inventory looking for a match to specified item name
        foreach (string Value in GameManage.instance.app.model.lstInventory)
        {
            if (Value.ToLower() == Name.ToLower()) // checking lowercases
            {
                return true; // If find match return true
            }
        }
        return false; // return false if no match found
    }

    // Method for adding item to player inventory if doesnt exist already, and exists in current scenes Items,
    // Plus returns any currently still available items for pickup, if any... plus the current story at end.
    public string ItemAdd(string Name)
    {
        //Instantiates a local GetItems class for checking the item to add exists
        GetItems lcItems = new GetItems();

        // Makes string saying no items of that name found and hsowing hwat items are available!
        string lcText = "No Item Found!\n\n" + GameManage.instance.app.model.currentScene.sceneStory;

        // Checks if Inventory already has said item to add, and returns message + avaialble items + Story & exits code
        if (Exists(Name) == true)
            return "You already Picked Up from here!\n\n" + GameManage.instance.app.model.currentScene.sceneStory;

        // Checks items exists for current scene and if does, adds it to players inventory list
        if (lcItems.Exists(Name) != "Unknown")
        {
            GameManage.instance.app.model.lstInventory.Add(lcItems.Exists(Name)); // Adds it to player inventory
            // Overrides string saying no item found, to say.. ItemName picked up and show still available items if any + story
            GameManage.instance.app.controller.gameServer.SendMessage("CHAT", "Picked up the " + Name);
            lcText = lcItems.Exists(Name) + " was picked up!\n\n" + GameManage.instance.app.model.currentScene.sceneStory;
        }

        return lcText; // Returns the message string for output display
    }
}
