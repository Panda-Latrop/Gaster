using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObject
{
    protected Collider2D[] hits = new Collider2D[5];
    public ExplosionObject(Vector3 position, float radius, GameObject owner, Team team, float damage, float power)
    {
        int count = Physics2D.OverlapBoxNonAlloc(position, Vector3.one * radius, 0.0f, hits, LayerMask.GetMask("Pawn"));
        for (int i = 0; i < count; i++)
        {
            IHealth health = hits[i].GetComponent<IHealth>();
            if(health != null && !health.Team.Equals(team))
            {
                health.Hurt(new DamageStruct(owner,team,damage, (hits[i].transform.position - position).normalized , power),default);
            }
        }
    }
}
