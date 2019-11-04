using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageListItemController : MonoBehaviour
{
    public delegate void OnImageListItemClickListener(int subject_index,int slide_index,Sprite sprite,GameObject border);
    public Image image;
    public GameObject border;
    public int subject_index;
    public int slide_index;

    OnImageListItemClickListener onImageListItemClickListener;
    public void SetInfo(int subject_index,int slide_index, OnImageListItemClickListener listener)
    {
        this.subject_index = subject_index;
        this.slide_index = slide_index;
        onImageListItemClickListener = listener;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (onImageListItemClickListener != null)
            onImageListItemClickListener(subject_index,slide_index,image.sprite, border);

        border.SetActive(true);
    }


}
