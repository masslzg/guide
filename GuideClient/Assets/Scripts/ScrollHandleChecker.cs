using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//繼承兩個UNITY的事件，並實作界面
public class ScrollHandleChecker : MonoBehaviour, IPointerDownHandler, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        VideoPlayerCtrl.ctrl.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        VideoPlayerCtrl.ctrl.Pause();
    }

}