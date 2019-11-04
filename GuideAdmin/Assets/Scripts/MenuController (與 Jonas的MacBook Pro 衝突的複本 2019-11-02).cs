using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject[] List;

    public Button[] ListButtons;
    public Sprite[] ListDefault;
    public Sprite[] ListPress;

    public Button[] LanguageButtons;
    public Sprite[] LanguageDefault;
    public Sprite[] LanguagePress;

    public void ChangeLanguage(int language)
    {
        foreach(GameObject obj in List)
        {
            obj.SetActive(false);
        }

        for (int i = 0; i < ListButtons.Length; i++)
        {
            ListButtons[i].image.sprite = ListDefault[i];
        }

        for (int i = 0; i < LanguageButtons.Length; i++)
        {
            if(i!=language)
                LanguageButtons[i].image.sprite = LanguageDefault[i];
            else
                LanguageButtons[i].image.sprite = LanguagePress[i];
        }
    }

    public void ShowList(int list)
    {
        for (int i = 0; i < ListButtons.Length; i++)
        {
            if (i != list)
            {
                List[i].SetActive(false);
                ListButtons[i].image.sprite = ListDefault[i];
            }
            else
            {
                List[i].SetActive(true);
                ListButtons[i].image.sprite = ListPress[i];
            }
        }
    }

    
}
