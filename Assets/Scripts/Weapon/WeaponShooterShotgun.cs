using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShooterShotgun : WeaponShooter
{
    [SerializeField]
    protected int projectileCount = 4;
    protected int projectileCountHalf;
    protected bool evenProjectileCount;


    protected override void Awake()
    {
        evenProjectileCount = projectileCount % 2 == 0;
        projectileCountHalf = projectileCount / 2;
        base.Awake();
    }
    protected override RaycastHit2D CreateProjectile(Vector3 position, Vector3 direction)
    {
        IPoolProjectile ipp;
        Vector3 directionIpp;
        float angleStep = (spread + spread) / projectileCount;
        float currentStep = -angleStep * projectileCountHalf + (evenProjectileCount? angleStep*0.5f: 0.0f);
        for (int i = 0; i < projectileCount; i++)
        {
            ipp = GameInstance.Instance.PoolManager.Pop(projectile) as IPoolProjectile;
            ipp.SetPosition(position);
            ipp.SetDamage(owner.gameObject, owner.Health.Team, damage * damageMultiply, power, speed);
            float miniSpread = spread * 0.3f;
            directionIpp = Quaternion.AngleAxis(currentStep + Random.Range(-miniSpread, miniSpread), Vector3.forward) * direction;
            currentStep += angleStep;
            ipp.SetDirection(directionIpp, 0);          
        }
        return default;
    }
}
