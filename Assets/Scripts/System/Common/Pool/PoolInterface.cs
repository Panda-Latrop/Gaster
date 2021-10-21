using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    string PoolTag { get; }
    PrefabHolder Prefab { get; }
    void OnPush();
    void OnPop();
    GameObject GameObject { get; }
    Transform Transform { get; }

    void SetPosition(Vector3 position);
    void SetRotation(Quaternion rotation);
    void SetScale(Vector3 scale);
    void SetParent(Transform parent);
}
public interface IPoolRagdoll : IPoolObject
{
    void AddForce(Vector3 force);
    void AddTorque(float force);
}
public interface IPoolProjectile : IPoolObject
{
    ProjectileBase Projectile { get; }
    [SerializeField]
    Rigidbody2D Rigidbody { get; }
    void SetDamage(GameObject owner, Team team, float damage, float power, float speed);
    void SetDirection(Vector3 direction, float angle);
}
public interface IPoolDynamicLineRenderer : IPoolObject
{
    void SetPoint(int index, GameObject point);
}


public interface IPoolPawn : IPoolObject , IPerceptionTarget
{
}

public interface IPoolCharacterAttacker : IPoolPawn
{
    AIPerceptionComponent Perception { get; }
}