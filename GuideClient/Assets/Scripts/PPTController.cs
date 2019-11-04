using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PPTController : MonoBehaviour
{
    public Text ProgramNameText;
	public Text SubjectNameText;
	public Image DisplayImage;
	public Transform ListContent;
    public GameObject SubjectListItem;
    public GameObject ImageListItem;
    public Text PageIndexText;
    public Image NextButtonImage;
    public Image PrevButtonImage;
    public VideoPlayerCtrl mVideoPlayerCtrl;

    Program currentProgram;
    GameObject lastImageBorder;
    Subject currentSubject;
    int currentSubjectIndex = 0;
    int currentSlideIndex=0;

    public void SetProgram(Program program) {
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
        DisplayImage.gameObject.SetActive(false);

        foreach (Transform obj in ListContent)
            Destroy(obj.gameObject);

        PageIndexText.text = "0/0";
        NextButtonImage.color = Color.gray;
        PrevButtonImage.color = Color.gray;

        Resources.UnloadUnusedAssets();

        yield return new WaitForSeconds(0.001f);

        int subject_index = 0;
        foreach (Subject subject in program.subjects)
        {
            GameObject subjectListItem = Instantiate(SubjectListItem) as GameObject;
            subjectListItem.name = "Subject_"+subject_index;
            subjectListItem.GetComponentInChildren<Text>().text = subject.name;
            subjectListItem.transform.SetParent(ListContent);

            Transform imageContent = subjectListItem.transform.Find("ListScrollArea/Content");
            int slide_index = 0;
            foreach (Slide slide in subject.slides)
            {
                GameObject imageListItem = Instantiate(ImageListItem) as GameObject;
                imageListItem.name = "Slide_" + slide_index;
                imageListItem.transform.SetParent(imageContent);

                Transform image = imageListItem.transform.Find("Image");
                
                Texture2D SpriteTexture = LoadTexture(slide.file_path);
                int width = SpriteTexture.width;
                int height = SpriteTexture.height;
                slide.width = width;
                slide.height = height;
                Sprite sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, width,height), new Vector2(0.5f, 0.5f), 100);
                image.GetComponent<Image>().sprite = sprite;
                RectTransform rt=image.GetComponent<RectTransform>();

                if ("自由模式".Equals(subject.mode) && "直向模式".Equals(subject.mode))
                {
                    int new_width = 200;
                    int new_height = (int)(1f*new_width * height / width);
                    if (new_height < 113)
                    {
                        new_height = 113;
                        new_width = (int)(1f * new_height * width / height);
                    }
                    rt.sizeDelta = new Vector2(new_width, new_height); 
                }
                else
                {
                    int new_height = 113;
                    int new_width = (int)(1f * new_height * width / height);
                    if (new_width < 200)
                    {
                        new_width = 200;
                        new_height = (int)(1f * new_width * height / width);
                    }
                    rt.sizeDelta = new Vector2(new_width, new_height);
                }
                imageListItem.GetComponent<ImageListItemController>().SetInfo(subject_index, slide_index, OnImageListItemClickListener);
                
                slide_index++;
                yield return new WaitForSeconds(0.001f);
            }
            subject_index++;
            yield return new WaitForSeconds(0.001f);
        }

        
    }

    void OnImageListItemClickListener(int subject_index, int slide_index, Sprite sprite,GameObject border)
    {
        mVideoPlayerCtrl.gameObject.SetActive(false);

        currentSubjectIndex = subject_index;
        currentSlideIndex = slide_index;

        currentSubject = currentProgram.subjects[subject_index];
        SubjectNameText.text = currentSubject.name;        
        
        Slide slide = currentSubject.slides[slide_index];
        if ("img".Equals(slide.type))
        {
            DisplayImage.sprite = sprite;
            RectTransform rt = DisplayImage.gameObject.GetComponent<RectTransform>();
            if ("自由模式".Equals(currentSubject.mode))
            {
                rt.sizeDelta = new Vector2(slide.width, slide.height);
            }
            else if ("直向模式".Equals(currentSubject.mode))
            {
                int new_width = 1080;
                int new_height = (int)(1f * new_width * slide.height / slide.width);
                rt.sizeDelta = new Vector2(new_width, new_height);
            }
            else
            {
                int new_height = 608;
                int new_width = (int)(1f * new_height * slide.width / slide.height);
                rt.sizeDelta = new Vector2(new_width, new_height);
            }
            DisplayImage.gameObject.SetActive(true);
        }
        else
        {
            mVideoPlayerCtrl.gameObject.SetActive(true);
            mVideoPlayerCtrl.SetVideoPath(slide.file_path.ToLower().Replace(".png",".mp4").Replace(".jpg", ".mp4"));
        }
        if (lastImageBorder != null) lastImageBorder.SetActive(false);
        lastImageBorder = border;

        PageIndexText.text = (slide_index + 1) + "/" + currentSubject.slides.Length;

        NextButtonImage.color = slide_index >= currentSubject.slides.Length-1? Color.gray:Color.white;
        PrevButtonImage.color = slide_index == 0 ? Color.gray : Color.white;
    }
        
    public void NextSlide()
    {
        
        if (currentSlideIndex+1 < currentSubject.slides.Length)
        {
            currentSlideIndex++;
            ImageListItemController controller = transform.Find("SelectScrollArea/Content/Subject_" + currentSubjectIndex +
                "/ListScrollArea/Content/Slide_" + currentSlideIndex).GetComponent<ImageListItemController>();             
            controller.OnClick();
        }
        
    }

    public void PrevSlide()
    {

        if (currentSlideIndex - 1 >= 0)
        {
            currentSlideIndex--;
            ImageListItemController controller = transform.Find("SelectScrollArea/Content/Subject_" + currentSubjectIndex +
                "/ListScrollArea/Content/Slide_" + currentSlideIndex).GetComponent<ImageListItemController>();
            controller.OnClick();
        }

    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Texture2D SpriteTexture = LoadTexture(FilePath);
        //FixTransparency(SpriteTexture);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100);
        return NewSprite;
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
