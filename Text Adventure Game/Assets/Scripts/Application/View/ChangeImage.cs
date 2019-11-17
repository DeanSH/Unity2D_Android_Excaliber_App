using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
// Class that is attached to the Image game object containing the story image for the current scene
//This is running a constant loop to check if scene changed and when does updates the image automatically and dynamically
public class ChangeImage : MonoBehaviour {
    //Variables
    string LastScene = "";
    List<Sprite> Sprites; // List of the images in Resources / images folder of assets.

    void Start()
    {
        // Get all images into variant
        var ObjSprites = Resources.LoadAll("Images", typeof(Sprite)).Cast<Sprite>();
        // istantiate the images list
        Sprites = new List<Sprite>();
        //Input one by one from the variant variable all the images into the Sprites list ready to access anytime
        foreach (var Obj in ObjSprites)
        {
            Sprites.Add(Obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Checks there is a current scene
        try
        {
            if (GameManage.instance.app.model.currentScene.sceneImageName != null)
            {
                // Prevent updating image in every frame, only when scene has changed
                if (LastScene != GameManage.instance.app.model.currentScene.sceneImageName)
                {
                    // Cycle all images to find the image matching current scene then assign it to the image game object
                    foreach (Sprite Value in Sprites)
                    {
                        if (Value.name == GameManage.instance.app.model.currentScene.sceneImageName)
                        {
                            this.GetComponent<Image>().overrideSprite = Value;
                            LastScene = GameManage.instance.app.model.currentScene.sceneImageName;
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {

        }

    }
}
