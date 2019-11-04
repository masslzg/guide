using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public delegate void OnSelectProgram(int program_index);

    public Sprite MenuBtnDefaultSprite;
    public Sprite MenuBtnPressSprite;

    public GameObject[] SubMenus;
    public Image[] MenuButtons;

    public Transform ProgramListGroup;
    public GameObject ProgramListItem;

    public DrawController mDrawController;

    OnSelectProgram mOnSelectProgram;

    public void SetOnSelectProgram(OnSelectProgram listener) {
        mOnSelectProgram = listener;
    }
    
    public void RefreshProgramList()
    {
        Program[] programs = MainController.programs;
        foreach (Transform child in ProgramListGroup)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < programs.Length; i++)
        {
            Program p = programs[i];
            GameObject item = Instantiate(ProgramListItem) as GameObject;            
            item.GetComponent<ProgramListItemController>().SetInfo(i, programs[i].name, SelectProgram);
            item.transform.SetParent(ProgramListGroup);
            item.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    void SelectProgram(int index)
    {
        if (mOnSelectProgram != null)
            mOnSelectProgram(index);
    }

    public void ToggleSubMenu(int sub_menu_index)
    {
        for (int i = 0; i < SubMenus.Length; i++)
        {
            if (i == sub_menu_index && !SubMenus[i].activeSelf)
            {
                SubMenus[i].SetActive(true);
                MenuButtons[i].sprite = MenuBtnPressSprite;
            }
            else
            {
                SubMenus[i].SetActive(false);
                MenuButtons[i].sprite = MenuBtnDefaultSprite;
            }
        }

        if (sub_menu_index == 2)
            mDrawController.Reset();
    }

    /** Close Guide Mode Start **/
    public GameObject HomePage;
    public GameObject ProjectSlide;
    public void CloseGuideMode()
    {
        HomePage.SetActive(true);
        ProjectSlide.SetActive(true);
    }
    /** Close Guide Mode end **/
}
