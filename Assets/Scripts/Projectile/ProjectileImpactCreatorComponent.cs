using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileImpactCreatorComponent : MonoBehaviour
{
    [SerializeField]
    protected List<string> surfaces = new List<string>();
    [SerializeField]
    protected List<DynamicExecutor> impacts = new List<DynamicExecutor>();
    private string surface = string.Empty;
    Dictionary<string, DynamicExecutor> dictionary = new Dictionary<string, DynamicExecutor>();

    protected void Start()
    {
        for (int i = 0; i < impacts.Count; i++)
        {
            dictionary.Add(surfaces[i], impacts[i]);
        }
    }

    public void SetPhysicsMaterial(PhysicsMaterial2D physicsMaterial)
    {
        //Debug.Log(physicsMaterial.name);
        if (physicsMaterial != null)
            surface = physicsMaterial.name.Split('_')[1].ToLower();
        else
            surface = string.Empty;
    }
    protected bool SurfaceSwitch(out int i)
    {
        switch (surface)
        {
            case "concrete":
                i = 0;
                return true;
            case "body":
                i = 1;
                return true;
            default:
                i = 0;
                return false;
        }
    }
    public void CreateImpact(Vector3 position, Quaternion rotation, bool attach = false, Transform parent = default)
    {
        IPoolObject po;
        if (dictionary.ContainsKey(surface))
        {
            po = GameInstance.Instance.PoolManager.Pop(dictionary[surface]);
            po.SetPosition(position);
            po.SetRotation(rotation);
            if (attach)
                po.SetParent(parent);

        }
    }
}
