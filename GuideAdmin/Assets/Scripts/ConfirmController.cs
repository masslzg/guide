using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmController : MonoBehaviour
{
    public delegate void OnConfirm(string filepath);
    static OnConfirm onConfirm;
    static string filepath;
    public static void SetOnConfirm(OnConfirm listener,string path)
    {
        onConfirm = listener;
        filepath = path;
    }

    public void Confirm()
    {
        if (onConfirm != null)
        {
            onConfirm(filepath);
        }

        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

}
