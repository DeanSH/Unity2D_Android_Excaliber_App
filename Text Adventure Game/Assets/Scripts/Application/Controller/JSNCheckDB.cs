using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//class attached to login screen to preform check for if database exists yet or not, creating it if it didnt!
public class JSNCheckDB : MonoBehaviour {
    // Variable containing access to the JSNDrop game object
    public JSNDrop TheJSNDrop;
    // method called when script loads
    void Start () {
        DataService tmpDbConnection = new DataService(); // Temp variable for Data Service
        tmpDbConnection.JSNet = TheJSNDrop; // Pass JSN Drop object to Dataservices JSNDrop variable
        tmpDbConnection.CheckDBExists(); // Call method to check exists
	}

}
