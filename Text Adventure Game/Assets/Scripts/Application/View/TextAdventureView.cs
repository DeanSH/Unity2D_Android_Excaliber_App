using UnityEngine;
using UnityEngine.UI;

//No code in here purely exist to reinforce the MVC theory, and for possibly future requirements
public class TextAdventureView {
// Class attached to input text feild to add listner for when enter is pressed on input to submit command

    //private Variables
    InputField.SubmitEvent se;
    //public variables for game GUI elements to be assigned to
    public InputField input;
    public Text output;
    public Text help;
    public Text score;
    public Text chat;
    public Text users;
    public GameObject scroll;

    // this method is to link the input field with the submit input event on pushing enter key
    public void LinkInputSubmit()
    {
        if (input != null)
        { // if we get a null this script is running when it should not, 
            //now add listner and reset input feild
            se = new InputField.SubmitEvent();
            se.AddListener(SubmitInput);
            input.onEndEdit = se;
            // The below commented code is used or not used depending on platform game is compiled for,
            // Currently comments out for compiling to and testing on a real android phone!
            // Would be uncomments to test in unity/blue stacks or if compiled to windows for example!
            //input.ActivateInputField();
        }
    }
    // Sub task called when enter key pressed to submit command
    private void SubmitInput(string arg0)
    {
        // Parse command then update scene output
        output.text = GameManage.instance.app.controller.commandProcessor.Parse(arg0);
        // Update help output to current scenes Help
        help.text = GameManage.instance.app.controller.commandProcessor.ProcessHelp();
        // Update score output to show players current score
        score.text = "Score: " + GameManage.instance.app.model.PlayerScore.ToString();

        // reset input box
        input.text = "";

        // The below commented code is used or not used depending on platform game is compiled for,
        // Currently comments out for compiling to and testing on a real android phone!
        // Would be uncomments to test in unity/blue stacks or if compiled to windows for example!
        // Its not good for android phone because it makes the onscreen keyboards constantly cover the game!
        //input.ActivateInputField();

    }

}
