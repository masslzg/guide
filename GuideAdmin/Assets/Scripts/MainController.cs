using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class MainController : MonoBehaviour
{
    public static string ROOT_DIR = @"/Users/Jonas/Dropbox\ \(個人\)/國安局/GuideDataOld/";
    public static string CHINESE_FOLDER = "中文/";
    public static string ENGLISH_FOLDER = "English/";
    public static string SUBJECTS_FOLDER = "主題/";
    public static string CONTROLLER_FOLDER = "觸控輪播/";
    public static string PROJECTOR_FOLDER = "投影輪播/";

    public static string CURRENT_LANGUAGE_FOLDER=CHINESE_FOLDER;

    public static Program[] programs;

    
    void Start()
    {
        ReadSettings ();
        InvokeRepeating("CheckResolution", 1, 60);
        SetInitial();
    }
        

    public void SetInitial(){
        new DirectoryInfo(ROOT_DIR).Create();
        new DirectoryInfo(ROOT_DIR + CHINESE_FOLDER).Create();
        new DirectoryInfo(ROOT_DIR + ENGLISH_FOLDER).Create();
        new DirectoryInfo(ROOT_DIR + SUBJECTS_FOLDER).Create();
        new DirectoryInfo(ROOT_DIR + CONTROLLER_FOLDER).Create();
        new DirectoryInfo(ROOT_DIR + PROJECTOR_FOLDER).Create();
    }
    
    void ReadSettings()
    {
        Dictionary<string, string> settings = Settings.Read();
        if (settings.ContainsKey("ROOT_DIR"))
            ROOT_DIR = settings["ROOT_DIR"];

    }

    void CheckResolution()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        Resources.UnloadUnusedAssets();
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            if (i == 0 && (Display.displays[i].renderingWidth != 1080 || Display.displays[i].renderingHeight != 1920))
            {
                Display.displays[i].SetRenderingResolution(1080, 1920);
            }
            else if (i != 0 && (Display.displays[i].renderingWidth != 1920 || Display.displays[i].renderingHeight != 1080))
            {
                Display.displays[i].SetRenderingResolution(1920, 1080);
            }
        }
    }

    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Texture2D SpriteTexture = LoadTexture(FilePath);
        //FixTransparency(SpriteTexture);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100);
        return NewSprite;
    }

    
    public static Texture2D LoadTexture(string FilePath)
    {
        byte[] FileData;
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);         // Create new "empty" texture                  
            Texture2D Tex2D = new Texture2D(2, 2, TextureFormat.RGB24, false);
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)            
                return Tex2D;                 // If data = readable -> return texture

        }
        return null;                     // Return null if load failed
    }


}