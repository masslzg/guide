using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MediaController : MonoBehaviour
{
    public Text ProgramNameText;
    public Text SubjectNameText;
    public Transform DisplayArea;
    public Transform GridContent;
    public GameObject SubjectGridItem;
    public Sprite defaultSprite;

    public GameObject DisplayImageItem;
    public GameObject DisplayVideoItem;

    Program currentProgram;
    Image lastButtonImage;
    Subject currentSubject;
    int currentSubjectIndex = 0;

    public void SetProgram(Program program)
    {
        currentProgram = program;
        StartCoroutine(SettingProgram(program));
    }

    IEnumerator SettingProgram(Program program)
    {
        ProgramNameText.text = program.name;
        if (program.subjects.Length > 0)
        {
            SubjectNameText.text = program.subjects[0].name;
        }
        DisplayArea.gameObject.SetActive(false);

        foreach (Transform obj in DisplayArea)
            Destroy(obj.gameObject);

        foreach (Transform obj in GridContent)
            Destroy(obj.gameObject);
                
        Resources.UnloadUnusedAssets();

        yield return new WaitForSeconds(0.001f);

        int subject_index = 0;
        foreach (Subject subject in program.subjects)
        {
            GameObject subjectListItem = Instantiate(SubjectGridItem) as GameObject;
            subjectListItem.name = "Subject_" + subject_index;
            subjectListItem.GetComponentInChildren<Text>().text = subject.name;
            subjectListItem.transform.SetParent(GridContent);
            subjectListItem.GetComponent<GridItemController>().SetInfo(subject_index, OnSubjectSelectedListener);
            subject_index++;
            yield return new WaitForSeconds(0.001f);
        }
    }

    void OnSubjectSelectedListener(int subject_index, Image buttonImage)
    {
        currentSubjectIndex = subject_index;
        currentSubject = currentProgram.subjects[subject_index];
        SubjectNameText.text = currentSubject.name;
                       
        DisplayArea.gameObject.SetActive(true);

        if (lastButtonImage != null) lastButtonImage.sprite = defaultSprite;
        lastButtonImage = buttonImage;

        StartCoroutine(SettingDisplayImage());
        
    }

    IEnumerator SettingDisplayImage()
    {
        foreach (Transform obj in DisplayArea)
            Destroy(obj.gameObject);

        int slide_index = 0;
        foreach (Slide slide in currentSubject.slides)
        {
            if ("img".Equals(slide.type))
            {
                GameObject imageListItem = Instantiate(DisplayImageItem) as GameObject;
                imageListItem.name = "Slide_" + slide_index;
                imageListItem.transform.SetParent(DisplayArea);

                Texture2D SpriteTexture = LoadTexture(slide.file_path);
                int width = SpriteTexture.width;
                int height = SpriteTexture.height;
                slide.width = width;
                slide.height = height;
                Sprite sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
                imageListItem.GetComponent<Image>().sprite = sprite;
                RectTransform rt = imageListItem.GetComponent<RectTransform>();

                if (width > height)
                {
                    int new_width = 200;
                    int new_height = (int)(1f * new_width * height / width);
                    rt.sizeDelta = new Vector2(new_width, new_height);
                }
                else
                {
                    int new_height = 200;
                    int new_width = (int)(1f * new_height * width / height);
                    rt.sizeDelta = new Vector2(new_width, new_height);
                }

                imageListItem.GetComponent<BoxCollider2D>().size = rt.sizeDelta;

                rt.localPosition = new Vector3(Random.Range(200, 880), Random.Range(-200, -400), 1);
                imageListItem.transform.Rotate(new Vector3(0, 0, Random.Range(-10f, 10f)));
            }
            else
            {
                GameObject videoItem = Instantiate(DisplayVideoItem) as GameObject;
                videoItem.name = "Slide_" + slide_index;
                videoItem.transform.SetParent(DisplayArea);
                videoItem.GetComponent<VideoPlayerCtrl>().SetVideoPath(slide.file_path.ToLower().Replace(".png", ".mp4").Replace(".jpg", ".mp4"));

                RectTransform rt = videoItem.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(Random.Range(200, 880), Random.Range(-200, -400), 1);
                videoItem.transform.Rotate(new Vector3(0, 0, Random.Range(-10f, 10f)));
            }
            

            slide_index++;
            yield return new WaitForSeconds(0.001f);
        }                                                                                                       
    }

    public Texture2D LoadTexture(string FilePath)
    {
        byte[] FileData;
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);         // Create new "empty" texture                  
            Texture2D Tex2D = new Texture2D(1, 1);
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)            
                return Tex2D;                 // If data = readable -> return texture

        }
        return null;                     // Return null if load failed
    }
}
