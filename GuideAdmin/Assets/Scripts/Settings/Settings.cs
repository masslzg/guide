using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Settings {

    private static readonly char[] InvalidFilenameChars = Path.GetInvalidFileNameChars();

    
    public static Dictionary<string, string> Read()
    {
        

        Dictionary<string, string> settings = new Dictionary<string, string>();
        string setting_file = Application.dataPath + "/../" + "Settings.txt";
        // Handle any problems that might arise when reading the text
        try
        {
            if (setting_file.IndexOfAny(InvalidFilenameChars) >= 0)
                return settings;
            string line;
            StreamReader theReader = new StreamReader(setting_file, Encoding.Default);
            using (theReader)
            {
                string[] columns;
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();
                    if (line != null)
                    {
                        columns = line.Split('=');
                        settings.Add(columns[0], columns[1]);
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            System.IO.FileStream oFileStream = null;
            oFileStream = new System.IO.FileStream(setting_file, System.IO.FileMode.Create);
            oFileStream.Close();
            Debug.Log(e.Message);
        }

        return settings;
    }
}
