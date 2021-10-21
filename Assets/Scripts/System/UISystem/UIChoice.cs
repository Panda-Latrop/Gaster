using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChoice : UIBaseWidget
{
    [SerializeField]
    protected Image box;
    [SerializeField]
    protected TextMeshProUGUI text, selected, help;
    [SerializeField]
    protected int lineInBox = 3;
    [SerializeField]
    protected AudioSource source;
    [SerializeField]
    protected AudioClip select, enter;
    protected int localPosition = 0, lastPosition;

    public override void Hide()
    {
        base.Hide();
        localPosition = 0;
        lastPosition = 0;
    }
    public void Enter()
    {
        Hide();
        source.clip = enter;
        source.Stop();
        source.time = 0;
        source.Play();
    }
    public void SetText(int position, List<string> texts)
    {
        Show();
        StringBuilder builder = new StringBuilder();
        int direction = position - lastPosition;
        localPosition = Mathf.Clamp(localPosition + direction, 0, lineInBox-1);
        SetSelect(localPosition);
        SetHelp(position, texts.Count);
        for (int i = position - localPosition; i < position - localPosition + lineInBox && i < texts.Count; i++)
            builder.Append(texts[i]).Append("\n"); ;
        if (direction != 0)
        {
            source.clip = select;
            source.Stop();
            source.time = 0;
            source.Play();
        }
        lastPosition = position;
        text.text = builder.ToString();
    }
    protected void SetSelect(int position)
    {
        StringBuilder builder = new StringBuilder();
        if (position == 0)
            builder.Append(">\n");
        else
            builder.Append(" \n");
        if (position == 1)
            builder.Append(">\n");
        else
            builder.Append(" \n");
        if (position == 2)
            builder.Append(">");
        else
            builder.Append(" ");
        selected.text = builder.ToString();
    }
    protected void SetHelp(int position, int length)
    {
        StringBuilder builder = new StringBuilder();

        if (length <= 3)
        {
            builder.Append("\n\n\n");
        }
        else
        {
            if (position > localPosition)
                builder.Append("•\n");
            else
                builder.Append(" \n");
            builder.Append(" \n");
            if (position < length- lineInBox+ localPosition)
                builder.Append("•");
            else
                builder.Append(" ");
        }
        help.text = builder.ToString();
    }
}
