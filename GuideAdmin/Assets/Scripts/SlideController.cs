using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class SlideController : MonoBehaviour
{
    public GameObject imageItem;
    public Transform controllSlideContent;
    public Transform projectorSlideContent;
    public ConfirmController confirmController;

    string ControllSlideDirPath = MainController.ROOT_DIR + MainController.CONTROLLER_FOLDER;
    string ProjectorSlideDirPath = MainController.ROOT_DIR + MainController.PROJECTOR_FOLDER;
    string[] allowedExtensions = { ".jpg", ".png" , ".jpeg" };
    DirectoryInfo controllSlideDir;
    DirectoryInfo projectorSlideDir;
    FileInfo[] controllSlideFiles;
    FileInfo[] projectorSlideFiles;

    void OnEnable()
    {
        controllSlideDir = new DirectoryInfo(ControllSlideDirPath);
        controllSlideFiles = RefreshNames(controllSlideDir);
        StartCoroutine(SetSlideItems(controllSlideFiles, controllSlideContent));

        projectorSlideDir = new DirectoryInfo(ProjectorSlideDirPath);
        projectorSlideFiles = RefreshNames(projectorSlideDir);
        StartCoroutine(SetSlideItems(projectorSlideFiles, projectorSlideContent));
    }

    private FileInfo[] RefreshNames(DirectoryInfo dir)
    {
        FileInfo[] files = dir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        foreach (FileInfo file in files)
        {
            System.IO.File.Move(file.FullName, dir.FullName + "_" + file.Name);
        }
        files = dir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        int i = 0;
        foreach (FileInfo file in files)
        {
            System.IO.File.Move(file.FullName, dir.FullName + (("" + i).PadLeft(3, '0')) + "__img" + file.Extension);
            i++;
        }
        return dir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
    }

    IEnumerator SetSlideItems(FileInfo[] files, Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);


        for (int i = 0; i < files.Length; i++)
        {
            AddSlideItem(i + 1, files[i], parent);
            yield return new WaitForSeconds(0.001f);
        }

    }

    private void AddSlideItem(int no, FileInfo file, Transform parent)
    {
        GameObject imageListItem = Instantiate(imageItem) as GameObject;
        imageListItem.name = file.Name;
        imageListItem.transform.SetParent(parent);
                
        imageListItem.transform.Find("Index/No").GetComponent<Text>().text = ("" + no).PadLeft(2, '0');
        imageListItem.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if ("ControllSlideContent".Equals(parent.gameObject.name))
                ConfirmController.SetOnConfirm(DeleteControllSlideFile, imageListItem.name);
            else
                ConfirmController.SetOnConfirm(DeleteProjectorSlideFile, imageListItem.name);
            confirmController.gameObject.SetActive(true);
        });
        imageListItem.transform.Find("ForwardButton").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if ("ControllSlideContent".Equals(parent.gameObject.name))
                SwapSlides(imageListItem.transform.GetSiblingIndex(), imageListItem.transform.GetSiblingIndex() - 1, controllSlideContent, ControllSlideDirPath);
            else
                SwapSlides(imageListItem.transform.GetSiblingIndex(), imageListItem.transform.GetSiblingIndex() - 1, projectorSlideContent, ProjectorSlideDirPath);

        });
        imageListItem.transform.Find("BackwardButton").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if ("ControllSlideContent".Equals(parent.gameObject.name))
                SwapSlides(imageListItem.transform.GetSiblingIndex(), imageListItem.transform.GetSiblingIndex() + 1, controllSlideContent, ControllSlideDirPath);
            else
                SwapSlides(imageListItem.transform.GetSiblingIndex(), imageListItem.transform.GetSiblingIndex() + 1, projectorSlideContent, ProjectorSlideDirPath);
        });

        Transform image = imageListItem.transform.Find("Mask/Image");

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


    public void SelectControllSlideFile()
    {
        FileDialogController.SelectImageFile(OnSelectControllSlideFile,"選擇圖片");
    }

    public void SelectProjectorSlideFile()
    {
        FileDialogController.SelectImageFile(OnSelectProjectSlideFile, "選擇圖片");
    }

    private void OnSelectControllSlideFile(string filepath)
    {
        FileInfo fileInfo = new FileInfo(filepath);
        string newFilePath = ControllSlideDirPath + (("" + controllSlideFiles.Length).PadLeft(3, '0')) + "__img" + fileInfo.Extension;
        fileInfo.CopyTo(newFilePath);
        controllSlideFiles = controllSlideDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        AddSlideItem(controllSlideFiles.Length, new FileInfo(newFilePath), controllSlideContent);
    }

    private void OnSelectProjectSlideFile(string filepath)
    {
        FileInfo fileInfo = new FileInfo(filepath);
        string newFilePath = ProjectorSlideDirPath + (("" + projectorSlideFiles.Length).PadLeft(3, '0')) + "__img" + fileInfo.Extension;
        fileInfo.CopyTo(newFilePath);
        projectorSlideFiles = projectorSlideDir.GetFiles().Where(f => allowedExtensions.Contains(f.Extension.ToLower())).OrderBy(f => f.Name).ToArray();
        AddSlideItem(projectorSlideFiles.Length, new FileInfo(newFilePath), projectorSlideContent);
    }

    private void DeleteControllSlideFile(string filename)
    {
        FileInfo fileInfo = new FileInfo(ControllSlideDirPath + filename);
        fileInfo.Delete();
        Destroy(controllSlideContent.Find(filename).gameObject);

        controllSlideFiles = RefreshNames(controllSlideDir);
        StartCoroutine(RefreshObjectNames(controllSlideContent, controllSlideFiles));
    }

    private void DeleteProjectorSlideFile(string filename)
    {
        FileInfo fileInfo = new FileInfo(ProjectorSlideDirPath + filename);
        fileInfo.Delete();
        Destroy(projectorSlideContent.Find(filename).gameObject);

        projectorSlideFiles = RefreshNames(projectorSlideDir);
        StartCoroutine(RefreshObjectNames(projectorSlideContent, projectorSlideFiles));
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

    private void SwapSlides(int index, int swap_index, Transform parent, string dirpath)
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
}