using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public delegate void DialogMessageDelegate(int message);
    public enum XmlDialogElementType
    {
        root,
        switсh,
        сase,
        branch,
        dialog,
        phrase,
        choice,
        option,
        set,
    }
    protected string directory => Application.dataPath + "/StreamingAssets/dialogs/" + GameInstance.language + "/";
    protected string file;
    protected XmlReader reader;
    protected bool hasDialog, overrideInput, skipped;

    protected int readCount, branchCount;
    protected XmlDialogElementType elementType;
    protected bool searchBranch = false;
    protected int toBranch = -1;
    protected int message = -1;

    protected int selected;
    protected List<string> options = new List<string>();
    protected List<string> optionsTo = new List<string>();

    [SerializeField]
    protected float waitBetweenSkip = 0.25f;
    protected float nextSkip;

    public XmlDialogElementType ElementType => elementType;
    public bool Skipped => skipped && !GameInstance.Instance.UISystem.UIDialog.enabled && !GameInstance.Instance.UISystem.UIChoice.enabled;
    public bool HasDialog => hasDialog;
    public bool OverrideInput => overrideInput && (GameInstance.Instance.UISystem.UIDialog.IsShow || GameInstance.Instance.UISystem.UIChoice.IsShow);

    protected Action OnEnd;
    protected DialogMessageDelegate OnMessage;

    [ContextMenu("Default")]
    public void PrepareDefault()
    {
        reader = XmlReader.Create(directory + "baseDialog.xml");
    }
    public DialogSystem Prepare(string file, DialogMessageDelegate action)
    {
        BindOnMessage(action);
        return Prepare(file);
    }
    public DialogSystem Prepare(string file)
    {
        Debug.Log("Load Dialog From : " + directory + file + ".xml");
        reader = XmlReader.Create(directory + file + ".xml");
        readCount = 0;
        branchCount = 0;
        hasDialog = true;
        options.Clear();
        optionsTo.Clear();
        return this;
    }
    [ContextMenu("Close")]
    public void Close()
    {
        if (hasDialog)
        {
            reader.Close();
        }
    }
    public bool Next(bool overrideInput = true, int local = 0)
    {
        this.overrideInput = overrideInput;
        this.skipped = false;

        string gkey = "";
        string gvalue = "";
        string person = "";
        string text = "";
        string to = "";
        string attribute = "";

        while (hasDialog && reader.Read())
        {
            readCount++;
            if (searchBranch)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals("branch"))
                        {
                            elementType = XmlDialogElementType.branch;
                            if (++branchCount == toBranch)
                                searchBranch = false;
                            break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name.Equals("root"))
                        {
                            End();
                            return false;
                        }
                        break;
                    default: break;
                }
            }
            else
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "root":
                                elementType = XmlDialogElementType.root;
                                break;
                            case "switch":
                                elementType = XmlDialogElementType.switсh;
                                break;
                            case "case":
                                elementType = XmlDialogElementType.сase;
                                if (reader.HasAttributes)
                                {
                                    bool gcheck = false, lcheck = false;
                                    if ((attribute = reader.GetAttribute("global")) != null)
                                    {
                                        string[] split = attribute.Split('-');
                                        gkey = split[0];
                                        gvalue = split[1];
                                        gcheck = GameInstance.Instance.GlobalCheck(gkey, int.Parse(gvalue));
                                    }
                                    if ((attribute = reader.GetAttribute("local")) != null)
                                        lcheck = local == int.Parse(attribute);
                                    if ((attribute = reader.GetAttribute("to")) != null)
                                        to = attribute;
                                    if (gcheck || lcheck)
                                    {
                                        searchBranch = true;
                                        toBranch = int.Parse(to);

                                    }
                                }
                                break;
                            case "branch":
                                elementType = XmlDialogElementType.branch;
                                branchCount++;
                                if (searchBranch && branchCount == toBranch)
                                    searchBranch = false;
                                break;
                            case "dialog":
                                elementType = XmlDialogElementType.dialog;
                                break;
                            case "phrase":
                                elementType = XmlDialogElementType.phrase;
                                if ((attribute = reader.GetAttribute("person")) != null)
                                    person = attribute;
                                break;
                            case "choice":
                                elementType = XmlDialogElementType.choice;
                                break;
                            case "option":
                                elementType = XmlDialogElementType.option;
                                if ((attribute = reader.GetAttribute("to")) != null)
                                    optionsTo.Add(attribute);
                                break;
                            case "set":
                                elementType = XmlDialogElementType.set;
                                if (reader.HasAttributes)
                                {
                                    if ((attribute = reader.GetAttribute("global")) != null)
                                    {
                                        string[] split = attribute.Split('-');
                                        gkey = split[0];
                                        gvalue = split[1];
                                        GameInstance.Instance.GlobalSet(gkey, int.Parse(gvalue));
                                    }
                                    if ((attribute = reader.GetAttribute("msg")) != null)
                                    {
                                        message = int.Parse(attribute);
                                        CallOnMessage(message);
                                        ClearOnMessage();
                                    }
                                        
                                    if ((attribute = reader.GetAttribute("to")) != null)
                                    {
                                        to = attribute;
                                        searchBranch = true;
                                        toBranch = int.Parse(to);
                                    }
                                }
                                break;
                            default: break;
                        }
                        break;
                    case XmlNodeType.Text:
                        if (elementType.Equals(XmlDialogElementType.phrase))
                            text = reader.Value;
                        if (elementType.Equals(XmlDialogElementType.option))
                            options.Add(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case "root":
                                End();
                                return false;
                            case "switch":
                                break;
                            case "branch":
                                End();
                                return false;
                            case "dialog":
                                break;
                            case "phrase":
                                GameInstance.Instance.UISystem.UIChoice.Hide();
                                GameInstance.Instance.UISystem.UIDialog.Set(text, person);
                                return true;
                            case "choice":
                                GameInstance.Instance.UISystem.UIDialog.Hide();
                                GameInstance.Instance.UISystem.UIChoice.SetText(selected, options);
                                return true;
                            case "option":
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        return false;
    }

    public void End()
    {
        if (hasDialog)
        {
            hasDialog = false;
            overrideInput = false;
            searchBranch = false;
            toBranch = -1;
            CallOnEnd();
            ClearOnEnd();
            ClearOnMessage();
            Hide();
            Close();
        }
    }
    public void Hide()
    {
        GameInstance.Instance.UISystem.UIDialog.Hide();
        GameInstance.Instance.UISystem.UIChoice.Hide();
    }
    public void Skip(bool withNext = true)
    {
        if (hasDialog && overrideInput && elementType.Equals(XmlDialogElementType.phrase) && Time.time >= nextSkip)
        {
            skipped = true;
            nextSkip = Time.time + waitBetweenSkip;
            if (GameInstance.Instance.UISystem.UIDialog.enabled)
            {
                GameInstance.Instance.UISystem.UIDialog.Skip();
            }
            else
            {
                if (withNext)
                    Next();
            }
        }
        else
        {
            skipped = false;
        }
    }
    public void Enter()
    {
        if (hasDialog && overrideInput && elementType.Equals(XmlDialogElementType.option))
        {
            searchBranch = true;
            toBranch = int.Parse(optionsTo[selected]);
            selected = 0;
            options.Clear();
            optionsTo.Clear();
            GameInstance.Instance.UISystem.UIChoice.Enter();
            Next();
        }
    }
    public void Select(int derection)
    {
        if (hasDialog && overrideInput && elementType.Equals(XmlDialogElementType.option))
        {
            selected = Mathf.Clamp(selected + derection, 0, options.Count - 1);
            GameInstance.Instance.UISystem.UIChoice.SetText(selected, options);
        }
    }
    public void CallOnMessage(int message)
    {
        OnMessage?.Invoke(message);
    }
    public void BindOnMessage(DialogMessageDelegate action)
    {
        OnMessage += action;
    }
    public void UnbindOnMessage(DialogMessageDelegate action)
    {
        OnMessage -= action;
    }
    public void ClearOnMessage()
    {
        OnMessage = null;
    }
    public void CallOnEnd()
    {
        OnEnd?.Invoke();
    }
    public void BindOnOnEnd(Action action)
    {
        OnEnd += action;
    }
    public void UnbindOnEnd(Action action)
    {
        OnEnd -= action;
    }
    public void ClearOnEnd()
    {
        OnEnd = null;
    }
}