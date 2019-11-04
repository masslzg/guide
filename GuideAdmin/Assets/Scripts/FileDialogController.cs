using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;

public class FileDialogController : MonoBehaviour
{
    public delegate void OnSelectFile(string filepath);

    public static void SelectMediaFile(OnSelectFile onSelectFile,string title)
    {
        var extensions = new[] {
            new ExtensionFilter("Media Files", "png", "jpg", "jpeg","mp4")            
        };
        string[] path = StandaloneFileBrowser.OpenFilePanel(title, "", extensions, false);
        if (onSelectFile != null && path.Length > 0)
        {
            onSelectFile(path[0]);
        }
    }

    public static void SelectImageFile(OnSelectFile onSelectFile, string title)
    {
        var extensions = new[] {
            new ExtensionFilter("Media Files", "png", "jpg", "jpeg")
        };
        string[] path = StandaloneFileBrowser.OpenFilePanel("選擇圖片", "", extensions, false);        
        if (onSelectFile != null && path.Length>0)
        {            
            onSelectFile(path[0]);
        }
    }
}
