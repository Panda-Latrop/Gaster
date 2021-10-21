using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    [SerializeField]
    protected UIDialog uiDialog;
    [SerializeField]
    protected UIChoice uiChoice;

    public UIDialog UIDialog => uiDialog; //{ get { if (uiChoice.enabled) { uiChoice.Hide(); } return uiDialog; } }
    public UIChoice UIChoice => uiChoice;
    protected void Awake()
    {
        uiDialog.enabled = false;
        uiChoice.enabled = false;
    }
    protected void Update()
    {
        if (uiDialog.enabled)
            uiDialog.OnUpdate();
    }
}
