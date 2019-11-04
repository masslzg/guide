using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridItemController : MonoBehaviour
{
    public delegate void OnSubjectSelectedListener(int subject_index,Image buttonImage);
    public Image buttonImage;    
    public Sprite selectedSprite;
    public int subject_index;

    OnSubjectSelectedListener onSubjectSelectedListener;
    public void SetInfo(int subject_index, OnSubjectSelectedListener listener)
    {
        this.subject_index = subject_index;
        onSubjectSelectedListener = listener;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (onSubjectSelectedListener != null)
            onSubjectSelectedListener(subject_index, buttonImage);

        buttonImage.sprite = selectedSprite;
    }

}
