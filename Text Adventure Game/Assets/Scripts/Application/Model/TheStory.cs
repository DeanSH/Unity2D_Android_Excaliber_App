using System.Collections.Generic;

// Contains everything that constructs the story/paths/help/images/items etc for the game!
// You really only need to change the story to suit your own here and here only!!
// Only modify code at other scripts if your sure you know what your doing.
public class TheStory
{
    public List<Scenes> lstScenes = new List<Scenes>(); // List of scenes story builder will store scenes in temporarily
    private int lstIndex = 0;
    // Here is where the whole story is consructed when method is called inside data base build section of data service!
    public void BuildStory()
    {

        //Scene 1 
        //BuildScene is a Method used to construct the scene into a SceneData class before adding to the list of Scenes!
        lstScenes.Add(BuildScene(
            "start", //Scene Identifier: if player types this command leads to this scene, but only if current scenes HELP allows the command.
            "scene1", //Image Identifier for this scene, which must match the name of an image in the Resources\Images Folder found in the Unity Assests!
            "left,right,find,pickup,items,load,chat/", // HELP: Controls possible commands at each scene, and shown when players type HELP command!
            "Stick", //Scene Item\s: Can be Empty or hold One or more Items available for pickup at a Scene, Multiple Items are seperated by a Comma. EG "Bow,Arrows"
            "You awake in the forest and see two paths ahead, which will you take?", //The Scene Story Line!
            "", //Redirection Scene: Place holder for the name of a Scene that player will be redirected to when this scene trys to load, if player holds the following Redirection Item in there Inventory! This is useful to redirect players to other scenes when player are holding particular items (if desired)
            "")); //Redirection Item: Place holder for the name of certain item, that if player holds this item in their inventory when this scene loads, they get redirected to the above Redirection Scene instead of this scene!

        ///////////////// If took left route at first scene /////////////////////////////

        //Scene 2 : This is the General Structure i preffer >>
        lstScenes.Add(BuildScene("left", "scene2", // Identifier , Image name
            "fight bandit,run from bandit,back,find,pickup,items,chat/", "", // Help , Items
            "Oh no its a Bandit! Will you stand and fight the Bandit or run away?", // Story
            "","")); // Redirection Scene , Redirection Trigger Item

        //Scene 4 : Notice the Identifier here "fight bandit" existed in the Help section of Scene 2 making it possible to come here from that scene!
        lstScenes.Add(BuildScene("fight bandit", "scene4",
            "fight gang,run from gang,back,find,pickup,items,chat/", "Bone",
            "You fight and defeat the bandit! But his Gang is coming to kill you! Do you fight the Gang or run away?",
            "",""));

        //Scene 4.1 - You Die
        lstScenes.Add(BuildScene("fight gang", "scene4",
            "exit,load,chat/", "",
            "You fight and kill 3 more bandits before his Gang get the better of you, cutting your head off! GAME OVER!!!!",
            "", ""));

        //Scene 4.2
        lstScenes.Add(BuildScene("run from bandit", "scene4",
            "fight gang,run from gang,back,find,pickup,items,chat/", "",
            "You try to run from the bandit and get cut off by his gang! They want to kill you! Do you fight the Gang or try run again?",
            "", ""));

        //Scene 12.1
        lstScenes.Add(BuildScene("run from gang", "scene12",
            "check it out,avoid it,back,find,pickup,items,chat/", "",
            "You see a cave entrance in the distance, looks like a safe place! Do you want to check it out or avoid it?",
            "", ""));

        //Scene 11
        lstScenes.Add(BuildScene("check it out", "scene11",
            "enter cave,back,find,pickup,items,save,chat/", "helmet",
            "You reach the cave entrance and wonder if its really safe to enter? But is anywhere truely safe! Do you enter?",
            "", ""));

        //Scene 8
        lstScenes.Add(BuildScene("enter cave", "scene8",
            "swim past,wait hiding,back,find,pickup,items,chat/", "",
            "You enter the cave and see a goblin city! Swim past through the water or wait hiding for them to sleep then sneak through?",
            "", ""));

        //Scene 10 - You die
        lstScenes.Add(BuildScene("wait hiding", "scene10",
            "exit,load,chat/", "",
            "You wait hiding and fall asleep, and evil goblin stumbles upon you and kills you as you sleep! GAME OVER!!!",
            "goblin", "Bone"));

        //Scene 9
        lstScenes.Add(BuildScene("swim past", "scene9",
            "glow worms,north cave,back,find,pickup,items,save,chat/", "",
            "You come upon a Dragon, it speaks saying there is a way out to the north, but recommends you visit the glow worms first!",
            "", ""));

        //Scene 10.1 - You live
        lstScenes.Add(BuildScene("goblin", "scene10",
            "run away,back,find,pickup,items,chat/", "",
            "As you wait a Goblin finds you and attacks, you defend yourself using the Bone you picked up, smashing his skull open! Now you must run!",
            "", ""));

        //Scene 9.1 - Redirect to scene 9 dragon
        lstScenes.Add(BuildScene("run away", "scene9",
            "", "",
            "",
            "swim past", "Bone"));

        //Scene 10.2 - You die
        lstScenes.Add(BuildScene("north cave", "scene10",
            "exit,load,chat/", "",
            "As you travel the north cave path, you get attacked by an evil goblin who kills you! GAME OVER!!!",
            "", ""));

        //Scene 13
        lstScenes.Add(BuildScene("glow worms", "scene13",
            "exit cave,back,find,pickup,items,save,chat/", "Sword",
            "As your admiring the Glow worms something shiney reflects light into your eyes, and then you spot an Exit to the north of the cave!",
            "", ""));

        //Scene 3.2 - story merges to the right route
        lstScenes.Add(BuildScene("exit cave", "scene3",
            "enter town,sneak around,back,find,pickup,items,chat/", "Chest Armor",
            "You arrive to a small town surrounded by graves, will you enter the town?",
            "", ""));

        /////////////////// If took the Right route at start scene /////////////////

        //Scene 3
        lstScenes.Add(BuildScene("right", "scene3",
            "enter town,sneak around,back,find,pickup,items,chat/", "Chest Armor",
            "You arrive to a small town surrounded by graves, will you enter the town?",
            "", ""));

        //Scene 3.1
        lstScenes.Add(BuildScene("avoid it", "scene3",
            "enter town,sneak around,back,find,pickup,items,chat/", "Chest Armor",
            "You arrive to a small town surrounded by graves, will you enter the town?",
            "", ""));

        //Scene 5
        lstScenes.Add(BuildScene("enter town", "scene5",
            "deny king,join king,back,find,pickup,items,chat/", "",
            "Before you can enter you are stopped by King Aurthur and asked to Join the quest to find Excalibur!",
            "", ""));

        //Scene 6
        lstScenes.Add(BuildScene("join king", "scene6",
            "find lake,back,find,pickup,items,save,chat/", "Sword",
            "A soldier throws a sword on the ground for you to pick up! Now you travel in search of the lady in the lake!",
            "joined", "Sword"));

        //Scene 6.1
        lstScenes.Add(BuildScene("joined", "scene6",
            "find lake,back,find,pickup,items,save,chat/", "",
            "Now the quest begins and with King Arthur you travel in search of the lady in the lake!",
            "", ""));

        //Scene 7
        lstScenes.Add(BuildScene("deny king", "scene7",
            "i happily accept,run from amazons,back,find,pickup,items,save,chat/", "",
            "You go through the town and after some travel, 4 beautiful amazon woman approach asking to lay with you?",
            "", ""));

        //Scene 7.1
        lstScenes.Add(BuildScene("sneak around", "scene7",
            "i happily accept,run from amazons,back,find,pickup,items,save,chat/", "",
            "You sneaked around the town and after some travel, 4 beautiful amazon woman approach asking to lay with you?",
            "", ""));

        //Scene 7.2 - You die
        lstScenes.Add(BuildScene("i happily accept", "scene7",
            "exit,load,chat/", "",
            "You fool, Amazon woman only want to mate, and kill you immediately after the pleasures! GAME OVER!!!",
            "armor", "Chest Armor"));

        //Scene 1.1
        lstScenes.Add(BuildScene("run from amazons", "scene1",
            "left,right,back,find,pickup,items,save,chat/", "Stick",
            "Your back in the forest and see two paths again, which will you take?",
            "",""));

        //Scene 7.3
        lstScenes.Add(BuildScene("armor", "scene7",
            "move on,back,find,pickup,items,chat/", "",
            "Your shiney Chest Armor and efforts to pleasure the Amazons impressed them and your not killed!" +
            " Now its time to move on!",
            "", ""));

        //Scene 1.2 - redirect to scene 1.1
        lstScenes.Add(BuildScene("move on", "scene1",
            "", "",
            "",
            "run from amazons", "Chest Armor"));

        //Scene 18
        lstScenes.Add(BuildScene("find lake", "scene18",
            "camelot,back,find,pickup,items,chat/", "",
            "After days of searching you find the lake, King Arthur enters to get Excalibur from the lady of the lake!" + 
            " Now its time to return and defend Camelot!",
            "", ""));

        //Scene 14
        lstScenes.Add(BuildScene("camelot", "scene14",
            "meet vikings,back,find,pickup,items,save,chat/", "",
            "Your now in camelot but the vikings have arrived to attack, thankfully King Arthur now has Excalibur!" +
            " Defeating the Vikings will be easy and together you go out to meet them!",
            "", ""));

        //Scene 15
        lstScenes.Add(BuildScene("meet vikings", "scene15",
            "fight vikings,back,find,pickup,items,save,chat/", "Axe",
            "You come face to face with the Vikings leader King Ragnar, he spits in your face and calls you a coward!",
            "", ""));

        //Scene 16 - you die fighting
        lstScenes.Add(BuildScene("fight vikings", "scene16",
            "exit,load,chat/", "",
            "You fight bravely but stupidly never picked up any sword and are killed in battle! GAME OVER!!!",
            "lived", "Sword"));

        //Scene 16.1
        lstScenes.Add(BuildScene("lived", "scene16",
            "finish,find,pickup,items,save,chat/", "Excalibur",
            "You fight bravely and the Vikings are defeated, Camelot is saved! Its time to finish this Epic Journey!",
            "", ""));

        //Scene 17 - The End Happy ending
        lstScenes.Add(BuildScene("finish", "scene17",
            "exit,load,chat/", "",
            "You and the Kingdom of Camelot lived happily ever after! You Win - The End!!!",
            "idiot", "Excalibur"));

        //Scene 17.1 - The End Sad ending
        lstScenes.Add(BuildScene("idiot", "scene17",
            "exit,load,chat/", "",
            "Camelot lives Happily ever after, but your are caught an killed for stealing Excalibur! GAME OVER!!!",
            "", ""));
    }

    //BuildScene is a Method used to construct the scene into a SceneData class before adding to the list of Scenes!
    private Scenes BuildScene(string prIdentifier,
                                 string prImage,
                                 string prHelp,
                                 string prItems,
                                 string prStory,
                                 string prRedirectionScene,
                                 string prRedirectionItem)
    {
        // 5 parameter values are passed in, and this temporary local variable is created to construct a scene with
        Scenes lcScene = new Scenes();
        lstIndex += 1;
        lcScene.Id = lstIndex;
        // Adding 7 of the passed values into the new constructed scene DTO
        lcScene.sceneIdentifier = prIdentifier; // Identifies scene
        lcScene.sceneImageName = prImage; // Identifies scenes image name
        lcScene.sceneHelp = prHelp; // Scenes help
        lcScene.sceneStory = prStory; // scenes story
        lcScene.RedirectionScene = prRedirectionScene; // scenes redirect scene identifier
        lcScene.RedirectionItem = prRedirectionItem; // scenes redirection item trigger
        lcScene.Item = prItems; // scene items available if any

        return lcScene; // Return the temporary constructed scene back to caller, where it then gets added to Scenes List.
    }
}
