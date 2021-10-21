using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class SpriteRendererHolderComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected SpriteRenderer[] renderers;
    [SerializeField]
    protected int blinkCount = 4;
    protected int currentBlinkCount;
    [SerializeField]
    protected float timeToBlink = 0.1f;
    protected float nextBlink;
    protected float solidColor, grayShades, halfColor;
    protected Color grayShadeColor;
    protected Color color;


    public float SolidColor => solidColor;
    public float GrayShades => grayShades;
    public float HalfColor => halfColor;
    public Color GrayShadeColor => grayShadeColor;


    [ContextMenu("ApplyMaterialChange")]
    protected void ApplyMaterialChange()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        renderers[0].GetPropertyBlock(mpb);
        ChangeMaterialPropertyBlock(mpb);
        renderers[0].SetPropertyBlock(mpb);
        for (int i = 1; i < renderers.Length; i++)
        {
            renderers[i].SetPropertyBlock(mpb);
        }
    }

    protected virtual void ChangeMaterialPropertyBlock(MaterialPropertyBlock mpb)
    {
        mpb.SetFloat("_SolidColor", solidColor);
        mpb.SetFloat("_GrayShades", grayShades);
        mpb.SetFloat("_HalfColor", halfColor);
        mpb.SetColor("_GrayShadeColor", grayShadeColor);
    }


    public void OnHurt()
    {
        if (!enabled)
        {
            enabled = true;
            currentBlinkCount = 0;
            color = renderers[0].color;
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = Color.red;
            nextBlink = Time.time + timeToBlink;
            solidColor = 1;
            ApplyMaterialChange();
        }
    }

    public void OnUnhurt()
    {
        if (enabled)
        {
            enabled = false;
            currentBlinkCount = 0;
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = color;
            solidColor = 0;
            ApplyMaterialChange();
        }
    }
    public void OnMorale()
    {
        //Debug.Log("1 ");
        if (grayShades != 1)
        {
            grayShades = halfColor = 1;
            grayShadeColor = Color.white;
            ApplyMaterialChange();
        }
    }
    public void OnHeal()
    {
        if (grayShades == 1)
        {
            grayShades = 1;
            halfColor = 0;
            grayShadeColor = Color.green;
            ApplyMaterialChange();
        }
    }
    public void OnRage()
    {
        if (grayShades == 1)
        {
            grayShades = 1;
            halfColor = 0;
            grayShadeColor = Color.red;
            ApplyMaterialChange();
        }
    }
    public void OnBomb()
    {
        if (grayShades == 1)
        {
            grayShades = 1;
            halfColor = 0;
            grayShadeColor = new Color(1.0f,0.5f,0.0f,1.0f);
            ApplyMaterialChange();
        }
    }
    public void OnLightning()
    {
        if (grayShades == 1)
        {
            grayShades = 1;
            halfColor = 0;
            grayShadeColor = Color.cyan;
            ApplyMaterialChange();
        }
    }
    public void Clear()
    {
        enabled = false;
        currentBlinkCount = 0;
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].color = Color.white;
        solidColor = grayShades = halfColor = 0;
        grayShadeColor = Color.white;
        ApplyMaterialChange();
    }


    protected void Update()
    {
        if (currentBlinkCount >= blinkCount)
        {
            OnUnhurt();
        }
        else
        {
            if (Time.time >= nextBlink)
            {
                currentBlinkCount++;
                for (int i = 0; i < renderers.Length; i++)
                    renderers[i].color = (currentBlinkCount % 2 == 0 ? Color.red : Color.white);

                nextBlink = Time.time + timeToBlink;
            }
        }
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("enabled", new JSONBool(enabled));
        jsonObject.Add("curBlink", new JSONNumber(currentBlinkCount));
        SaveSystem.TimerSave(jsonObject, "blink", nextBlink);
        jsonObject.Add("solid", new JSONNumber(solidColor));
        jsonObject.Add("gray", new JSONNumber(grayShades));
        jsonObject.Add("half", new JSONNumber(halfColor));
        JSONArray grayColorJArray = new JSONArray();
        {
            grayColorJArray.Add(new JSONNumber(grayShadeColor.r));
            grayColorJArray.Add(new JSONNumber(grayShadeColor.g));
            grayColorJArray.Add(new JSONNumber(grayShadeColor.b));
            grayColorJArray.Add(new JSONNumber(grayShadeColor.a));
        }
        jsonObject.Add("grayColor", grayColorJArray);
        JSONArray colorJArray = new JSONArray();
        {
            colorJArray.Add(new JSONNumber(color.r));
            colorJArray.Add(new JSONNumber(color.g));
            colorJArray.Add(new JSONNumber(color.b));
            colorJArray.Add(new JSONNumber(color.a));
        }
        jsonObject.Add("color", colorJArray);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        enabled = jsonObject["enabled"].AsBool;
        currentBlinkCount = jsonObject["curBlink"].AsInt;
        SaveSystem.TimerLoad(jsonObject, "blink", ref nextBlink);
        solidColor = jsonObject["solid"].AsFloat;
        grayShades = jsonObject["gray"].AsFloat;
        halfColor = jsonObject["half"].AsFloat;
        JSONArray grayColorJArray = jsonObject["grayColor"].AsArray;
        {
            grayShadeColor.r = grayColorJArray[0];
            grayShadeColor.g = grayColorJArray[1];
            grayShadeColor.b = grayColorJArray[2];
            grayShadeColor.a = grayColorJArray[3];           
        }
        JSONArray colorJArray = jsonObject["color"].AsArray;
        {
            color.r = colorJArray[0];
            color.g = colorJArray[1];
            color.b = colorJArray[2];
            color.a = colorJArray[3];
        }
        if (solidColor > 0)
        {
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].color = (currentBlinkCount % 2 == 0 ? Color.red : Color.white);
        }
        ApplyMaterialChange();
        return jsonObject;
    }
}