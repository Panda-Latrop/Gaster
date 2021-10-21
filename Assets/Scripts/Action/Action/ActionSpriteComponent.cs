using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpriteComponent : ActionBaseComponent
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    protected Sprite sprite;
    public override void OnEnter()
    {
        base.OnEnter();
        spriteRenderer.sprite = sprite;
    }
    public override bool OnUpdate()
    {
        return true;
    }
}