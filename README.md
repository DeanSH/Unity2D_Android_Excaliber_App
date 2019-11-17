# Unity2D_Andriod_Excaliber_App
Built in Unity 2D mode &amp; tested as an Android APK! This Excaliber app is a text-based "Pick A Path" style adventure game! Using JSON-Drop for Data storage, MQTT for live chat with other players &amp; the Accelerometer for selecting Paths &amp; executing the text command options at each scene!

This app was design using the AMVCC (Application, Model, View, Controller Components) architecture, and I have provided Screen Shots and a full details Report from this project documenting all aspects of its design and build, including story boards, use case diagrams and more.

## How To Use:
Before opening the C# VS App solution inside of Unity, navigate to "Unity2D_Andriod_Excaliber_App/Text Adventure Game/Library/" and find the "ShaderCache.zip" file, this must be extracted as is in this location, it will put another folder there containing Shader files. After that you can load the project and it should be load fine!

There are MQTT and JSON-Drop folders in the project that connect to a MQTT and JSON-Drop hosted server that no longer exists!! These were provided by my Tutor at the time when creating this for an assessment, so I am unable to provide these. You can either try setup your own servers for these services, or modify the code to use a SQL database to manage Data storage and Live multiplayer chat.

-> Live Chat works by allowing players to see who else is in the same current scene as themself, and chat to the other players there.
