using UnityEngine.UI;
using UnityEngine;

//Class to handle storing to player prefs the player game options for music and accelerometer
public class SetupScreen : MonoBehaviour {

	// Runs when class loads
	void Start () {
        //Link variables to GUI elements
        Toggle tmpMusic = GameObject.Find("ToggleMusic").GetComponent<Toggle>();
        Toggle tmpAccel = GameObject.Find("ToggleAccel").GetComponent<Toggle>();
        //By default tick boes are ticked, so now check if player prefs is 0 meaning not ticked an set accordingly!
        if (PlayerPrefs.GetInt("Music", 1) == 0)
            tmpMusic.isOn = false;
        if (PlayerPrefs.GetInt("Accel", 1) == 0)
            tmpAccel.isOn = false;
    }

}
