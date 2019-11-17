using System.Collections.Generic;

// Class used to Show Items available for a scene or check an item exists
public class GetItems {

    // Returns as string any Items for a scene or that there isnt any, plus includes current scene story attached to end.
    public string Show()
    {
        string lcOutput = "No items found in current Scene!\n\n" + GameManage.instance.app.model.currentScene.sceneStory;
        //Basically if there are no items for current scene (Items = 0) then return message + story and exit!
        if (GameManage.instance.app.model.currentScene.Item == "")
            return lcOutput;

        // Instantiate a GetInventory class for purpose of using to check Items havnt already been pickup from this scene.
        GetInventory lcInventory = new GetInventory();

        if (lcInventory.Exists(GameManage.instance.app.model.currentScene.Item) == true)
            return lcOutput;

        // If made it this far, then there is an Items available so return the string and infrom player how to pickup!
        lcOutput = "Item Found: " + GameManage.instance.app.model.currentScene.Item + "\n\n" + GameManage.instance.app.model.currentScene.sceneStory;
        return lcOutput;
    }

    // Use this for checking item exists before adding pickup to players inventory (seen in GetInventory class)
    public string Exists(string Name)
    {
        if (GameManage.instance.app.model.currentScene.Item != "")
            return GameManage.instance.app.model.currentScene.Item; // Returns the Exact Item Name with Text Casing exactly as Input at Story Builder!

        return "Unknown"; // Returns this text meaning that it didnt exist!
    }
}
