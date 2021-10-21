using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBaseWidget : MonoBehaviour
{
    [SerializeField]
    protected Canvas canvas;
    [SerializeField]
    protected bool isShow = false;

    public bool IsShow => isShow;
    public virtual void Show()
    {
        isShow = enabled = canvas.enabled = true;
    }
    public virtual void Hide()
    {
        isShow = enabled = canvas.enabled = false;
    }
    public virtual void OnUpdate() { }
    // public virtual void OnLateUpdate() { }
}
