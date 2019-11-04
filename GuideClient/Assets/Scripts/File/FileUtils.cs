using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using System.Text;

public class FileUtils {

    public static string FormatDateTime(DateTime dt)
    {
        //return "" + dt.Year + "" + dt.Month + "" + dt.Day + "" + dt.Hour + "" + dt.Minute + "" + dt.Second;
        return dt.ToString("yyyyMMddHHmmss");
    }


    public static byte[] LoadFile(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        return null;
    }

    
    public static string CheckNewFile(String dir_path)
    {
        try
        {
            DirectoryInfo info = new DirectoryInfo(dir_path);
            FileInfo[] files = info.GetFiles().Where(f => !f.Name.StartsWith("_")).OrderBy(p => p.CreationTime).ToArray();

            if (files.Length > 0)
                return files[0].Name;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
        }

        return "";
    }

    public static FileInfo[] CheckOldFiles(String dir_path)
    {
        try
        {
            DirectoryInfo info = new DirectoryInfo(dir_path);
            return info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();            
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
            UnityEngine.Debug.Log(e.StackTrace);
        }

        return new FileInfo[] { };
    }
}
