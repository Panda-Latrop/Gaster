using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : UIBaseWidget
{
    [SerializeField]
    protected DialogPersonAtlas atlas;
    [SerializeField]
    protected Image box, face;
    [SerializeField]
    protected TextMeshProUGUI text;
    [SerializeField]
    protected AudioSource source;

    protected int currentChar = 0;
    protected string rawText;
    protected StringBuilder builder;
    [SerializeField]
    protected bool useTyping;
    [SerializeField]
    protected float timeToTyping = 0.05f;
    protected float nextTyping;

    public void Set(string text,string person)
    {
        Show();
        Sprite face;
        AudioClip sound;
        this.text.text = "";
        currentChar = 0;
        builder = new StringBuilder();
        rawText = text;
        atlas.GetPerson(person,out face, out sound);
        this.face.sprite = face;
        source.clip = sound;
        if (!useTyping)
        {
            source.Stop();
            source.time = 0;
            source.Play();
            this.text.text = text;
            enabled = false;
        }
    }
    public void Skip()
    {
        text.text = rawText;
        enabled = false;
    }
    public override void OnUpdate()
    {
        if(Time.time >= nextTyping)
        {
            builder.Append(rawText[currentChar]);
            text.text = builder.ToString();
            currentChar++;
            source.Stop();
            source.time = 0;
            source.Play();
            nextTyping = Time.time + timeToTyping;
        }
        if (currentChar >= rawText.Length)
            enabled = false;
    }

}
