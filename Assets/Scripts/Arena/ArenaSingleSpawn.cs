using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSingleSpawn : ArenaSpawnBase
{
    [SerializeField]
    protected CharacterAttacker character;
    public override void OnSpawn(ref List<Pawn> pawns)
    {
        if (!isFinished)
        {
            if (useSequenceSpawn)
            {
                if (CanSpawn())
                {
                    if (infinitySpawn)
                        Spawn();
                    else
                    {
                        pawns.Add(Spawn());
                        if (SpawnLimit())
                            isFinished = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < maxSpawn; i++)
                    pawns.Add(Spawn());
                isFinished = true;
            }
        }
    }
    protected override Pawn Spawn()
    {
        Vector3 position = RandomPoint();
        IPoolCharacterAttacker ipca = GameInstance.Instance.PoolManager.Pop(character) as IPoolCharacterAttacker;
        ipca.Perception.ForceDetection(GameInstance.Instance.PlayerController.ControlledPawn);      
        ipca.SetPosition(position);
        if(useSpawnEffect)
        {
            IPoolObject ipo = GameInstance.Instance.PoolManager.Pop(spawnEffect);
            ipo.SetPosition(position);           
        }
        spawned++;
        return ipca.GetPawn();
    }
    public override void OnEnd()
    {
        return;
    }
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(spawnPoints[i],0.5f);
            Gizmos.DrawWireSphere(spawnPoints[i], spawnRadius);
        }
    }
}
