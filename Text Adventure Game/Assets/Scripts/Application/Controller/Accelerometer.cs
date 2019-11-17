using UnityEngine;
using System.Timers;

// Class to handle accelerometer sensor input and allow using it to play game
public class Accelerometer : MonoBehaviour {

    //Variables to put accelerometer X and Z axis's force values into
    float xPos;
    float zPos;
    //Timer to tick and process accelerometer forces every 500ms
    Timer ProcessTimer = new Timer(500);

    // Start method for when script loads
    void Start()
    {
        // Timer setup
        ProcessTimer.AutoReset = true;
        ProcessTimer.Elapsed += DoTimer; //Points timer to DoTimer method to execute it each timer tick
    }

    // Update executes the code once per frame
    void Update () {
        //Check player settings for if accelerometer is enabled if yes then process code
        if (PlayerPrefs.GetInt("Accel", 1) == 1)
        {
            //Populate the X and Z axis variables with the current acceleration values 
            xPos = Input.acceleration.x;
            zPos = Input.acceleration.z;
            //Check if Z axis ever output a unusual -1 value and if so then game is running in bluestacks
            //Accelerometer control via keystrokes in bluestacks requires altering my Z Axiz detection
            if (zPos == -1 && GameManage.instance.app.controller.commandProcessor.IsBlueStacks == false)
            {
                GameManage.instance.app.controller.commandProcessor.IsBlueStacks = true;
                GameManage.instance.app.controller.commandProcessor.zAxis = -1f;
            }
            //Check is timer is still running and if not process accelerometer (using timer as a delay handler)
            if (ProcessTimer.Enabled == false)
            {
                //Send the X and Z force values to the Accelerometer handler and process them
                GameManage.instance.app.controller.commandProcessor.HandleAccelerometer(xPos, zPos);
                //Start the timer delay again
                ProcessTimer.Enabled = true;
                ProcessTimer.Start();
            }
        }
    }
    //Timer method to stop timer and support the desired delay effect between accelerometer processing of 500ms
    private void DoTimer(object sender, ElapsedEventArgs e)
    {
        //Stop timer
        ProcessTimer.Enabled = false;
        ProcessTimer.Stop();
    }
}
