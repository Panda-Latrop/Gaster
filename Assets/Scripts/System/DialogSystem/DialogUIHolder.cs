using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class DialogUIHolder : MonoBehaviour
{
    [SerializeField]
    protected Canvas canvas;
    [SerializeField]
    protected Image box,face;
    [SerializeField]
    protected TextMeshProUGUI text;

    protected int selected;
    protected List<string> texts = new List<string>();
    protected List<string> to = new List<string>();
    StringBuilder builder = new StringBuilder();

    public string Selected => to[selected];

    public void Show(string text, Sprite face)
    {
        this.face.enabled = true;
        this.text.SetText(text);
        this.face.sprite = face;
        enabled = canvas.enabled = true;
    }
    public void Show(List<string> texts, List<string> to)
    {
        this.face.enabled = false;
        this.texts = texts;
        this.to = to;
        selected = 0;
        Select(0);
        enabled = canvas.enabled = true;

    }
    public void Select(int direction)
    {
        builder.Clear();
        selected = Mathf.Clamp(selected + direction, 0, texts.Count - 1);
        for (int i = 0; i < selected; i++)
        {
            builder.Append(" ");
            builder.AppendLine(texts[i]);
        }
        builder.Append("*");
        builder.AppendLine(texts[selected]);
        for (int i = selected+1; i < texts.Count; i++)
        {
            builder.Append(" ");
            builder.AppendLine(texts[i]);
        }
        text.SetText(builder.ToString());
    }
    
    public void Hide()
    {
        enabled = canvas.enabled = false;
    }
}