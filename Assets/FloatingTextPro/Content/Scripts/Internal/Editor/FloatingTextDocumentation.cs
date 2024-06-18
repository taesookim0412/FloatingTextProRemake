using UnityEngine;
using UnityEditor;
using LovattoFloatingText.TutorialWizard;

public class FloatingTextDocumentation : TutorialWizard
{
    //required//////////////////////////////////////////////////////
    public string FolderPath = "floating-text-pro/editor/";
    public NetworkImages[] m_ServerImages = new NetworkImages[]
    {
        new NetworkImages{Name = "img-0.png", Image = null},
        new NetworkImages{Name = "img-1.png", Image = null},
        new NetworkImages{Name = "img-2.png", Image = null},
        new NetworkImages{Name = "img-3.png", Image = null},
        new NetworkImages{Name = "img-4.png", Image = null},
    };
    public Steps[] AllSteps = new Steps[] {
    new Steps { Name = "Get Started", StepsLenght = 0 , DrawFunctionName = nameof(GetStartedDoc)},
    new Steps { Name = "Usage", StepsLenght = 0, DrawFunctionName = nameof(UsageDoc) },
    new Steps { Name = "FloatingText()", StepsLenght = 0, DrawFunctionName = nameof(FloatingTextDoc) },   
    new Steps { Name = "Custom Floating", StepsLenght = 0, DrawFunctionName = nameof(CustomSequenceDoc) },
    new Steps { Name = "Floating Text Settings", StepsLenght = 0, DrawFunctionName = nameof(FloatingTextSettingsDoc) },
    new Steps { Name = "Font", StepsLenght = 0, DrawFunctionName = nameof(FontDoc) },
    new Steps { Name = "Player Camera", StepsLenght = 0, DrawFunctionName = nameof(PlayerCameraDoc) },
    new Steps { Name = "Text Mesh Pro", StepsLenght = 0, DrawFunctionName = nameof(TextMeshProDoc) },
    };
    private readonly GifData[] AnimatedImages = new GifData[]
   {
        new GifData{ Path = "name.gif" },
   };

    public override void OnEnable()
    {
        base.OnEnable();
        base.Initizalized(m_ServerImages, AllSteps, FolderPath, AnimatedImages);
        Style.highlightColor = ("#3700B3").ToUnityColor();
        allowTextSuggestions = true;
    }

    public override void WindowArea(int window)
    {
        AutoDrawWindows();
    }
    //final required////////////////////////////////////////////////

    void GetStartedDoc()
    {
        DrawText("<b><size=16>Require:</size></b>\n\nUnity 2019.4++\n\n<b><size=16>Test:</size></b>\n\nYou can preview the example floating texts in the demo scene located in the Example folder (<i>Assets ➔ FloatingTextPro ➔ Example ➔ Scene➔*</i>)\n\nClick the purple balls to preview the samples floating texts that you can use, keep in mind that these are just some examples of what you can do with the floating text system, but you aren't limited to these, you can create your own designs with your custom floating sequence.\n\n<b><size=16>Text Mesh Pro:</size></b>\n\nIn order to use the Text Mesh Pro prefabs, you first have to import the TMP .unitypackage included in the root of the FloatingTextPro folder, for more details check the respective section.");
    }

    void UsageDoc()
    {
        DrawHyperlinkText("In each scene where you want to use the floating texts, do the following:\n\n1. Drag the <link=asset:Assets/FloatingTextPro/Content/Prefabs/Main/Floating Text Manager.prefab>Floating Text Manager</link> <i><size=8><color=#76767694>(click to ping)</color></size></i> prefab in the scene hierarchy.\n\n2. Select the instanced <b>Floating Text Manager</b> prefab and in the inspector window ➔ property <b>Player Camera</b> ➔ Assign your <b>Player camera or the scene's Main camera</b>.\n\n3. Now in the wherever script where you have the event that will show the floating text <b>e.g</b> <i>your enemy health manager script in the \"OnDamage()\"</i> function ➔ you can add the following code to create and show the floating text:");
        DrawCodeText("new FloatingText(\"TEXT_OR_NUMBER_HERE\")\n              .SetSettings(\"default\")\n              .SetTarget(ray.transform)\n              .SetPosition(ray.point)\n              .SetTextColor(Color.red)\n              .SetOutlineColor(Color.black)\n              .SetOutlineSize(2)\n              .StickAtOriginWorldPosition()\n              .SetReuses(3)\n              .Show();");
        DrawText("this sample code uses various of the available <b>builder options</b>, you can check the full list o available builder options and what each one of them does in the <b>FloatingText()</b> section.\n \nMany of these builder options are non required, the minimal required code to show a floating text is the following:");
        DrawCodeText("new FloatingText(\"TEXT_OR_NUMBER_HERE\")\n              .SetTarget(TargetTransform)\n              .Show();");
        Space(10);
        DrawHorizontalSeparator();

    }

    void FloatingTextDoc()
    {
        DrawText("<color=#00A7FFFF>FloatingText</color> is the constructor with which you build and show a floating text.\n \nThe usage is pretty simple, you create a new instance of the structure and pass the text you wanna show, e.g <i>new FloatingText(\"TEXT_OR_NUMBER\")</i>, then you have the option to customize the text with the builder functions, and finally, you call the '<b>Show()</b>' function to instance the floating text UI.\n \nThe builder options are optional and their purpose is to give you a way to easily modify the floating text design and behave through code, here is the list of all the builder functions:");

        DrawHorizontalColumn("<color=red>*</color> SetPosition(Vector3)", "<b>(REQUIRE)</b>, Pass the world position at which the floating text will use as reference to calculate the screen view position.", 200);
        DrawHorizontalColumn("SetTarget(Transform)", "Pass the floating text target, this is required if you want to reuse the floating text instead of instance one for every call,\nNote: make sure to call this before SetPosition() since this will overwrite the position with the target transform.position.", 200);
        DrawHorizontalColumn("SetTextColor(Color)", "Pass the text main color.", 200);
        DrawHorizontalColumn("SetTextExtraSize(Int)", "Pass an size modifier value that will be added or deducted <i>by passing a negative value)</i> to the text default font size.", 200);
        DrawHorizontalColumn("SetPositionOffset(Vector3)", "Pass an offset position that will be sum to the position, useful if you pass the position with SetTarget() but want to add some offset.", 200);
        DrawHorizontalColumn("SetReuses(Int)", "Set the times that the text instance can be re-used, Re-use = When the floating text is for the same target and instead of create a new floating text for every call you want to re-used a floating text that is already being show.\nIf you don't call this, the floating text won't be re-used.", 200);
        DrawHorizontalColumn("SetSettings(FloatingTextSettings)", "Pass a custom floating text sequence/animation settings.", 200);
        DrawHorizontalColumn("SetSettings(string)", "Pass a custom floating text sequence/animation settings that has been listed in the FloatingTextManagerSettings -> Floating Text Settings list.", 200);
        DrawHorizontalColumn("SetOutlineColor(Color)", "Pass the color of the text outline.", 200);
        DrawHorizontalColumn("ReuseWhileAlive()", "Tell the text that it can be re-used as long as it is still showing in the screen.", 200);
        DrawHorizontalColumn("StickAtOriginWorldPosition()", "Tell the floating text that it have to stick to the target transform world position, if this is not called, the text will float using the screen space position as reference.", 200);
        DrawHorizontalColumn("OnFinish(Action)", "Pass a callback that will be invoked once the floating text complete the sequence.", 200);
        DrawHorizontalColumn("<color=red>*</color> Show()", "<b>(REQUIRE)</b>, Call this at the very end to show the floating text, none of the builder functions that you call after calling this will have effect in the floating text.", 200);
    }

    void CustomSequenceDoc()
    {
        DrawText("<b>FloatingTextSettings</b> are ScriptableObjects that contain all the information of the floating text animation/movement, a series of float and animation curves properties that allow making unique floating simulation movements.\n \nThe asset package comes with some pre-made FloatingTextSettings prefabs that you can use directly or use as a reference to create custom ones, you can found these at: <i>Assets ➔ FloatingTextPro ➔ Content ➔ Prefabs ➔ Presents➔*</i>\n \nIn order to create a custom FloatingTextSettings simply duplicate one of the pre-made ones and customize the properties.");
        DrawTitleText("How to use?");
        DrawText("If you wanna use specific settings for a floating text you simply have to pass the settings reference in the <b>FloatingText</b> builder <i><b>.SetSettings(...)</b></i>, you will notice that there're 2 overlap methods for this, one in which you pass directly the <b>FloatingTextSettings</b> reference and the other where you pass a <b>string</b>; this last one is easier to use but you have to set up something before using it with your own created <i>FloatingTextSettings</i>.\n \nYou have to list the <i>FloatingTextSettings</i> in the <b>Floating Text Settings</b> list of <link=asset:Assets/FloatingTextPro/Content/Resources/FloatingTextManagerSettings.asset>FloatingTextManagerSettings</link> which is located in the <b>Resources</b> folder, simply create a new field in the list ➔ set a short custom name and assign the <i>FloatingTextSettings</i> ScriptableObject ➔ then you can pass the setting in the <i>FloatingText</i> builder <b>.SetSettings(\"my-settings-name\")</b>");
        DrawServerImage(0);
        DrawText("Then you can use it like:");
        DrawCodeText("new FloatingText(\"TEXT_OR_NUMBER\")\n              .SetSettings(\"my-settings-name\")\n              .SetTarget(Target)\n              .Show();");
    }

    void FloatingTextSettingsDoc()
    {
        DrawServerImage(1);
        Space(10);
        DrawHorizontalColumn("Floating Type", "Define how you wanna setup the text floating direction.");
        DrawHorizontalColumn("Start Sequence Duration", "Define the time that the start sequence will take, the start sequence is the Fade In and Start Scale Animation.");
        DrawHorizontalColumn("Static Duration", "Define the time that floating text will remain static right after the start sequence and before the floating sequence starts.");
        DrawHorizontalColumn("Floating Duration", "Define the time that the floating movement will last before the finish sequence starts.");
        DrawHorizontalColumn("Finish Sequence Duration", "Define the time that the finish sequence will take, the finish sequence is the Fade Out and Finish Scale Animation.");
    }

    void FontDoc()
    {
        DrawText("The package comes with various pre-made floating text designs that you can use.\n \nIn order to use a specific design you simply have to assign the floating text prefab in the <i>(Inspector)</i> <b>bl_FloatingTextManager ➔ Text Template.</b>\n\nYou can find the pre-made templates in the folder: <i>Assets ➔ FloatingTextPro ➔ Content ➔ Prefabs ➔ Texts➔* OR TextTMP for the text mesh pro ones.</i>");
        DrawServerImage(2);
        DrawText("In order to create a custom design, you simply have to use one of the pre-made templates as reference ➔ modify the UI, Font, Color, etc... as you desire ➔ create a prefab of it and assign it as is mentioned above.");
    }

    void PlayerCameraDoc()
    {
        DrawText("As the <i>Usage</i> section mentioned, one of the required steps in order to the floating text system work is a \"<b><b>Player Camera</b></b>\" reference that is assigned in the <b>FloatingTextManager</b> instance ➔ <b>bl_FloatingTextManager</b> ➔ <b>Player Camera</b>, now exist the case where the player is not set by default in the scene hierarchy but it's instantiated in runtime, due this you can't set the Player Camera reference in the inspector, for these case the Player Camera has to be assigned by code with:");
        DrawCodeText("bl_FloatingTextManager.Instance.PlayerCamera = MyPlayerCamera;");
        DrawText("<b>OR</b> an easier way is to simply attach the script <b><i>bl_FloatingTextCamera.cs</i></b> to your player camera ➔ in your player prefab where the <b>Camera</b> component attaches the mentioned script and that's.");
    }

    void TextMeshProDoc()
    {
        DrawHyperlinkText("This package includes support for both Unity UI text solutions <b>(UGUI and Text Mesh Pro)</b>, the asset includes floating texts prefabs for each system with exactly the same design and functionality with the only difference being the Text component.\n \n<b>BUT</b> <b>by default, you only will find the UGUI prefabs</b>, this in order to avoid errors after the importation due to missing the TMP package.\n \nSo in order to use the Text Mesh Pro prefabs, first make sure you have imported and set up the Text Mesh Pro package from the Unity Package Manager, if you don't know how to do this or verify, check this: <link=https://learn.unity.com/tutorial/textmesh-pro-importing-the-package>https://learn.unity.com/tutorial/textmesh-pro-importing-the-package</link>");
        DownArrow();
        DrawText("Once you have made sure to have the <b>Text Mesh Pro</b> package imported and ready, you have to import the <i>Floating Text Pro TMP</i> assets, for this, simply go to the root folder of the asset <b>(Assets ➔ FloatingTextPro ➔ *)</b> ➔ there you will see a .unitypackage called <b>FloatingTextPro TMP</b> ➔ double click on it and import all the content of it in the import wizard window.\n \nAfter that, you will see a new folder under the Prefabs folder <b>TextsTMP</b> where you will find all the Text Mesh Pro floating texts prefabs ready to use.");
    }

    [MenuItem("Window/Documentation/Floating Text Pro")]
    static void Open()
    {
        GetWindow<FloatingTextDocumentation>();
    }
}