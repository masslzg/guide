using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class MainController : MonoBehaviour
{
    public static string ROOT_DIR = @"C:/GuideDataOld/";
    public static string CHINESE_FOLDER = "中文/";
    public static string ENGLISH_FOLDER = "English/";
    public static string CONTROLLER_FOLDER = "觸控輪播/";
    public static string PROJECTOR_FOLDER = "投影輪播/";

    string current_language_folder=CHINESE_FOLDER;

    public static Program[] programs;

    public PPTController mPPTcontroller;
    public MediaController mMediaController;
    public MenuController mMenuController;

    public GameObject PPTModeMenu;
    public GameObject MeidaModeButton;

    public Text ProgramNameText;
    public Text SubjectNameTest;

    public ImageController mImageController1;
    public ImageController mImageController2;

    Program currentProgram;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        ReadSettings ();
        InvokeRepeating("CheckResolution", 1, 10);
        ReadData();
        ReadSlideData();
    }

    public void InitialView()
    {
        ProgramNameText.text = "";
        SubjectNameTest.text = "";

        mPPTcontroller.gameObject.SetActive(false);

        mMenuController.SetOnSelectProgram(OnSelectProgram);
        mMenuController.RefreshProgramList();
        //mMenuController.ToggleSubMenu(0);
    }

    void OnSelectProgram(int program_index)
    {
        currentProgram = programs[program_index];
        if ("簡報模式".Equals(currentProgram.default_mode))
        {
            mPPTcontroller.gameObject.SetActive(true);
            mPPTcontroller.SetProgram(currentProgram);
            PPTModeMenu.SetActive(true);
            MeidaModeButton.SetActive(false);
            mMediaController.gameObject.SetActive(false);
        }
        else {
            mMediaController.gameObject.SetActive(true);
            mMediaController.SetProgram(currentProgram);
            PPTModeMenu.SetActive(false);
            MeidaModeButton.SetActive(true);
            mPPTcontroller.gameObject.SetActive(false);
        }
        mMenuController.ToggleSubMenu(0);

    }

    public void ChangeMode()
    {
        if (!mPPTcontroller.gameObject.activeSelf)
        {
            mPPTcontroller.gameObject.SetActive(true);
            mPPTcontroller.SetProgram(currentProgram);
            PPTModeMenu.SetActive(true);
            MeidaModeButton.SetActive(false);
            mMediaController.gameObject.SetActive(false);
        }
        else
        {
            mMediaController.gameObject.SetActive(true);
            mMediaController.SetProgram(currentProgram);
            PPTModeMenu.SetActive(false);
            MeidaModeButton.SetActive(true);
            mPPTcontroller.gameObject.SetActive(false);
        }
    }

     public void ChangeToChinese()
    {
        current_language_folder = CHINESE_FOLDER;
        ReadData();
    }

    public void ChangeToEnglish()
    {
        current_language_folder = ENGLISH_FOLDER;
        ReadData();
    }

    public void ReadData(){
        programs = null;

        DirectoryInfo root = new DirectoryInfo(ROOT_DIR+current_language_folder);

        DirectoryInfo[] program_folders=root.GetDirectories().Where(f => f.Name.Contains("__")).OrderBy(f => f.Name).ToArray();
        programs = new Program[program_folders.Length];

        string[] splitString = { "__","." };
        DirectoryInfo[] subject_folders;
        FileInfo[] slide_files;

        Program program;
        Subject subject;
        Slide slide;
        string[] tmp;
        string[] allowedExtensions = { ".jpg", ".png" };
        for (int i=0;i<programs.Length;i++){
            program = new Program();
            tmp = program_folders[i].Name.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);
            program.name = tmp[1];
            program.created_date = tmp[2];
            program.default_mode = tmp[3];

            subject_folders= program_folders[i].GetDirectories().Where(f => f.Name.Contains("__")).OrderBy(f => f.Name).ToArray();
            program.subjects = new Subject[subject_folders.Length];
            for (int j = 0; j < program.subjects.Length; j++) {
                subject = new Subject();
                tmp = subject_folders[j].Name.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);
                subject.name = tmp[1];
                subject.created_date = tmp[2];
                subject.mode = tmp[3];

                
                slide_files =subject_folders[j].GetFiles().Where(f => f.Name.Contains("__") && allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
                subject.slides = new Slide[slide_files.Length];
                for (int k = 0; k < slide_files.Length; k++)
                {
                    slide = new Slide();
                    slide.file_path = slide_files[k].FullName;
                    tmp=slide_files[k].Name.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);
                    slide.name = tmp[1];
                    slide.type = tmp[2];

                    subject.slides[k] = slide;
                }
                program.subjects[j] = subject;
            }
            programs[i] = program;
        }

        InitialView();

        
    }

    void ReadSlideData()
    {
        string[] allowedExtensions = { ".jpg", ".png" };

        DirectoryInfo controllSlideFolder = new DirectoryInfo(ROOT_DIR + CONTROLLER_FOLDER);
        FileInfo[] controllSlideImages = controllSlideFolder.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).ToArray();
        mImageController1.SetFiles(controllSlideImages);

        DirectoryInfo projectorSlideFolder = new DirectoryInfo(ROOT_DIR + PROJECTOR_FOLDER);
        FileInfo[] projectorSlideImages = projectorSlideFolder.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).ToArray();
        mImageController2.SetFiles(projectorSlideImages);
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
            Display.displays[i].Activate(1080, 1920, 50);
        }
        Resources.UnloadUnusedAssets();
        
        /*
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
        }*/
    }

    
}