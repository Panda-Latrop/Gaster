using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : MonoBehaviour
{
    public CharacterAttacker character;
    
    [ContextMenu("Spawn")]
    protected void Spawn()
    {
        IPoolCharacterAttacker ipca = GameInstance.Instance.PoolManager.Pop(character) as IPoolCharacterAttacker;
        ipca.SetPosition(transform.position);
        ipca.Perception.ForceDetection(GameInstance.Instance.PlayerController.ControlledPawn);
    }
}
