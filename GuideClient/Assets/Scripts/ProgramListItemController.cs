using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramListItemController : MonoBehaviour
{
    public delegate void OnProgramListItemClickListener(int index);
    public Text ProgramNameText;
    public int program_index;
    OnProgramListItemClickListener onProgramListItemClickListener;
    public void SetInfo(int index,string name, OnProgramListItemClickListener listener)
    {
        program_index = index;
        ProgramNameText.text = name;
        onProgramListItemClickListener = listener;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick() {
        if(onProgramListItemClickListener!=null)
            onProgramListItemClickListener(program_index);
    }
}
