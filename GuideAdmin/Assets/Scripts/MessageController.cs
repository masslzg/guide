using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public Text MessageText;
    public void SetMessage(string mes)
    {
        MessageText.text = mes;
        gameObject.SetActive(true);
    }
}
