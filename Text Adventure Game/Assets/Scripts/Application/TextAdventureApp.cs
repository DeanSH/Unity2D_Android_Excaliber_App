using System;

[Serializable]
// App class to store the linking to model / view / controller instances
public class TextAdventureApp
{

    // Reference to the root instance of the Model holding our game Data.
    public TextAdventureModel model = new TextAdventureModel();
    // Reference to the root instance of the Controller that interacts with our Viewable Objects and Model Data
    public TextAdventureController controller = new TextAdventureController();
    // Reference to the root instance of the View that adds listeners and holds methods called from interaction with the visible game objects
    public TextAdventureView view = new TextAdventureView();

}
