using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Reusable class to assign to any button on any scene / game screen
public class ButtonHandler : MonoBehaviour {
    //private variable to hold the object this script is attached to in
    Button btnMe;
    //public variable to assign access to the JSNdrop game object as required for use by some buttons
    public JSNDrop TheJSNDrop;
    //public string variable that can be custom set in each object its attached to,
    //must be unique for button command processor to be able to process the button when clicked.
    public string ButtonCommand;
    // Use this for initialization
    void Start()
    {
        btnMe = this.GetComponent<Button>(); // Gets the button component for the game object script is attached to
        if (btnMe != null)
        { // if we get a null this script is running when it should not
            btnMe.onClick.AddListener(SubmitIt); // Attach listner for on click event
        }
    }
    // The sub task method called upon when buttons are clicked
	void SubmitIt()
    {
        // Creates local temporary Button Commands class to process the ButtonCommand according to a 
        //Unique string assigned when script was attached to a game button object
        ButtonController lcBtnController = new ButtonController();
        lcBtnController.dbConnection.JSNet = TheJSNDrop; // Pass the JSNdrop gameobject to the temporary Button Controller
        lcBtnController.BtnProcess(ButtonCommand); // Process button click according to its command identifier
	}
}
