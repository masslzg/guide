using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SubjectEditController : MonoBehaviour
{
    public InputField SubjectTitleText;
    public Transform ListContent;
    public GameObject Item;
    public MessageController messageController;
    public ConfirmController confirmController;
    public Toggle[] ModeToggles;

    string[] allowedExtensions = { ".jpg", ".png" };
    string[] splitString = { "__", "." };

    DirectoryInfo subjectDir;
    FileInfo[] itemFiles;
    bool isSelectVideo = false;
    string selectVideoFilePath;
    string mode = "";

    public void InitializeForCreate()
    {
        SubjectTitleText.text = "";
        foreach (Transform child in ListContent)
            Destroy(child.gameObject);

        ModeToggles[0].isOn = true;
        mode = "自由模式";

        subjectDir = null;
        itemFiles = null;
    }

    public void InitializeForEdit(DirectoryInfo dir, string subject_name)
    {
        subjectDir = dir;
        SubjectTitleText.text = subject_name;
        foreach (Transform child in ListContent)
            Destroy(child.gameObject);

        if (dir.Name.Contains("自由模式"))
        {
            ModeToggles[0].isOn = true;
            mode = "自由模式";
        }
        else if (dir.Name.Contains("橫向模式"))
        {
            ModeToggles[1].isOn = true;
            mode = "橫向模式";
        }
        else if (dir.Name.Contains("直向模式"))
        {
            ModeToggles[2].isOn = true;
            mode = "直向模式";
        }

        itemFiles = RefreshNames(subjectDir);
        StartCoroutine(SetItems(itemFiles, ListContent));
    }

    public void ToggleMode()
    {
        bool needChange = false;
        if (ModeToggles[0].isOn && !"自由模式".Equals(mode))
        {
            needChange = true;
            mode = "自由模式";
        }
        else if (ModeToggles[1].isOn && !"橫向模式".Equals(mode))
        {
            needChange = true;
            mode = "橫向模式";
        }
        else if (ModeToggles[2].isOn && !"直向模式".Equals(mode))
        {
            needChange = true;
            mode = "直向模式";
        }

        if (needChange && subjectDir != null)
        {
            string oldPath = subjectDir.FullName;
            string newDirPath = subjectDir.FullName.Replace("自由模式", mode).Replace("橫向模式", mode).Replace("直向模式", mode);
            if (subjectDir.Exists)
            {
                Directory.Move(oldPath, newDirPath);
                subjectDir = new DirectoryInfo(newDirPath);
                itemFiles = subjectDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
            }
            else
            {
                subjectDir = new DirectoryInfo(newDirPath);
                itemFiles = null;
            }
        }
    }

    public void BackToList()
    {

    }

    private FileInfo[] RefreshNames(DirectoryInfo dir)
    {
        FileInfo[] files = dir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        foreach (FileInfo file in files)
        {
            System.IO.File.Move(file.FullName, dir.FullName + "_" + file.Name);
            if (file.Name.Contains("__video"))
            {
                System.IO.File.Move(file.FullName.Replace(file.Extension, ".mp4"), dir.FullName + "_" + file.Name.Replace(file.Extension, ".mp4"));
            }
        }
        files = dir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        int i = 0;
        foreach (FileInfo file in files)
        {
            string[] info = file.Name.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);
            string scale = "100";
            if (info.Length >= 3)
                scale = info[2];
            if (file.Name.Contains("__img"))
                System.IO.File.Move(file.FullName, dir.FullName + (("" + i).PadLeft(3, '0')) + "__img__" + scale + file.Extension);
            else if (file.Name.Contains("__video"))
            {
                System.IO.File.Move(file.FullName, dir.FullName + (("" + i).PadLeft(3, '0')) + "__video__" + scale + file.Extension);
                System.IO.File.Move(file.FullName.Replace(file.Extension, ".mp4"), dir.FullName + (("" + i).PadLeft(3, '0')) + "__video__" + scale + ".mp4");
            }
            i++;
        }
        return dir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
    }


    IEnumerator SetItems(FileInfo[] files, Transform parent)
    {
        for (int i = 0; i < files.Length; i++)
        {
            AddItem(i + 1, files[i], parent);
            yield return new WaitForSeconds(0.001f);
        }
    }

    private void AddItem(int no, FileInfo file, Transform parent)
    {
        string[] info = file.Name.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);
        string scale = "100";
        if (info.Length >= 3)
            scale = info[2];
        GameObject item = Instantiate(Item) as GameObject;
        item.name = file.Name;
        item.transform.SetParent(parent);

        InputField scaleInputField = item.transform.Find("Scale/InputField").GetComponent<InputField>();
        scaleInputField.text = scale;
        scaleInputField.onEndEdit.AddListener(delegate (string result)
        {
            string scale_result = result.Trim();
            if (scale_result.Length == 0) scale_result = "100";
            scaleInputField.text = scale_result;
            string new_filename = info[0] + "__" + info[1] + "__" + scale_result + file.Extension;
            System.IO.File.Move(file.FullName, subjectDir.FullName + new_filename);
            itemFiles=subjectDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
            item.name = new_filename;
        });
        item.transform.Find("Index/No").GetComponent<Text>().text = ("" + no).PadLeft(2, '0');
        item.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            ConfirmController.SetOnConfirm(DeleteFile, item.name);
            confirmController.gameObject.SetActive(true);
        });
        item.transform.Find("ForwardButton").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SwapItems(item.transform.GetSiblingIndex(), item.transform.GetSiblingIndex() - 1, parent, subjectDir.FullName);

        });
        item.transform.Find("BackwardButton").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            SwapItems(item.transform.GetSiblingIndex(), item.transform.GetSiblingIndex() + 1, parent, subjectDir.FullName);
        });

        Transform image = item.transform.Find("Mask/Image");

        Texture2D SpriteTexture = MainController.LoadTexture(file.FullName);
        int width = SpriteTexture.width;
        int height = SpriteTexture.height;

        Sprite sprite = Sprite.Create(SpriteTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
        image.GetComponent<Image>().sprite = sprite;
        RectTransform rt = image.GetComponent<RectTransform>();
        int new_width = 320;
        int new_height = (int)(1f * new_width * height / width);
        if (new_height < 240)
        {
            new_height = 240;
            new_width = (int)(1f * new_height * width / height);
        }
        rt.sizeDelta = new Vector2(new_width, new_height);

        parent.GetComponent<ContentSizeFitter>().enabled = true;
        parent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
        parent.GetComponent<ContentSizeFitter>().enabled = false;
    }

    private void DeleteFile(string filename)
    {
        FileInfo fileInfo = new FileInfo(subjectDir.FullName + filename);
        fileInfo.Delete();
        Destroy(ListContent.Find(filename).gameObject);

        itemFiles = RefreshNames(subjectDir);
        StartCoroutine(RefreshObjectNames(ListContent, itemFiles));
    }

    IEnumerator RefreshObjectNames(Transform parent, FileInfo[] files)
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i].Name);
            parent.GetChild(i).name = files[i].Name;
            parent.GetChild(i).Find("Index/No").GetComponent<Text>().text = ("" + (i + 1)).PadLeft(2, '0');
        }

        parent.GetComponent<ContentSizeFitter>().enabled = true;
        parent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
        parent.GetComponent<ContentSizeFitter>().enabled = false;
    }

    private void SwapItems(int index, int swap_index, Transform parent, string dirpath)
    {
        if (swap_index < 0 || swap_index >= parent.childCount) return;

        string swap = parent.GetChild(swap_index).name;
        string current = parent.GetChild(index).name;

        System.IO.File.Move(dirpath + swap, dirpath + "_" + swap);
        System.IO.File.Move(dirpath + current, dirpath + "_" + current);

        System.IO.File.Move(dirpath + "_" + swap, dirpath + current);
        System.IO.File.Move(dirpath + "_" + current, dirpath + swap);

        parent.GetChild(swap_index).name = current;
        parent.GetChild(index).name = swap;

        Text swapText = parent.GetChild(swap_index).Find("Index/No").GetComponent<Text>();
        Text currentText = parent.GetChild(index).Find("Index/No").GetComponent<Text>();
        string tmp = swapText.text;
        swapText.text = currentText.text;
        currentText.text = tmp;

        parent.GetChild(index).SetSiblingIndex(swap_index);
    }

    public void ChangeSubjectName()
    {
        if (subjectDir == null && SubjectTitleText.text.Trim().Length != 0)
        {
            string subject_name = SubjectTitleText.text.Trim();
            string subject_dir_name = subject_name + "__" + DateTime.Now.ToString("yyyy-MM-dd") + "__" + mode + "/";
            subjectDir = new DirectoryInfo(MainController.ROOT_DIR + MainController.SUBJECTS_FOLDER + subject_dir_name);            
            itemFiles = null;
        }
        else if(subjectDir != null && SubjectTitleText.text.Trim().Length != 0)
        {
            string subject_name = SubjectTitleText.text.Trim();
            string subject_dir_name = subject_name + "__" + DateTime.Now.ToString("yyyy-MM-dd") + "__" + mode + "/";
            string newDirPath = MainController.ROOT_DIR + MainController.SUBJECTS_FOLDER + subject_dir_name;
            Directory.Move(subjectDir.FullName, newDirPath);
            subjectDir = new DirectoryInfo(newDirPath);
            itemFiles = subjectDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        }

        Debug.Log(subjectDir.Name);
    }

    public void AddNewFile()
    {
        if (subjectDir == null && SubjectTitleText.text.Trim().Length == 0)
        {
            messageController.SetMessage("請先填寫主題名稱");
            return;
        }
        FileDialogController.SelectMediaFile(OnSelectFile, "選擇多媒體檔案");
    }

    private void OnSelectFile(string filepath)
    {
        isSelectVideo = filepath.Contains(".mp4");
        if (isSelectVideo)
        {
            selectVideoFilePath = filepath;
            FileDialogController.SelectImageFile(OnSelectVideoImageFile, "選擇影片截圖");
        }
        else
        {
            subjectDir.Create();
            FileInfo fileInfo = new FileInfo(filepath);
            string newFilePath = subjectDir.FullName + (("" + (itemFiles==null?0:itemFiles.Length)).PadLeft(3, '0')) + "__img__100" + fileInfo.Extension;            
            fileInfo.CopyTo(newFilePath);
            itemFiles = subjectDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
            AddItem(itemFiles.Length, new FileInfo(newFilePath), ListContent);
        }
    }

    private void OnSelectVideoImageFile(string filepath)
    {
        subjectDir.Create();

        FileInfo videoFile = new FileInfo(selectVideoFilePath);
        FileInfo videoImage = new FileInfo(filepath);
        string newFilePath = subjectDir.FullName + (("" + (itemFiles == null ? 0 : itemFiles.Length)).PadLeft(3, '0')) + "__video__100";        
        videoFile.CopyTo(newFilePath+ videoFile.Extension);
        videoImage.CopyTo(newFilePath + videoImage.Extension);

        itemFiles = subjectDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        AddItem(itemFiles.Length, new FileInfo(newFilePath + videoImage.Extension), ListContent);
    }




    
}
