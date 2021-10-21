using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStartDialog : MonoBehaviour
{
    public string file;
    int local = 0;
    [ContextMenu("Start")]
    public void OnStart()
    {
        GameInstance.Instance.Dialog.Prepare(file, OnMessege).Next(true, local);
    }
    [ContextMenu("Prepare")]
    public void Prepare()
    {
        GameInstance.Instance.Dialog.Prepare(file);
    }
    [ContextMenu("Next")]
    public void Next()
    {
        GameInstance.Instance.Dialog.Next(true, local);
    }
    protected void OnMessege(int messege)
    {
        local = messege;
    }
}
