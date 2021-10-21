using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderBaseComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected Character owner;
    [SerializeField]
    protected Transform attachSocket;
    [SerializeField]
    protected List<WeaponBase> slots = new List<WeaponBase>();
    [SerializeField]
    protected List<bool> slotEquips = new List<bool>();
    [SerializeField]
    protected int currentSlot = -1;

    protected virtual void Start()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i >= slotEquips.Count ||  !slotEquips[i])
            {
                WeaponBase weapon =  AddWeapon(slots[i],false,true,i);
                if (i == currentSlot)
                {
                    weapon.gameObject.SetActive(true);
                }
            }
            else
            {
                slots[i].SetOwner(owner);
            }
        }
    }
    public ShootState ShootState => slots[currentSlot].ShootState;
    public void SetDamageMultiply(float multiply = 1.0f)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slotEquips[i])
                slots[i].damageMultiply = multiply;
        }
    }
    public virtual void SetFire(bool fire)
    {
        if (currentSlot >= 0 && slotEquips.Count >= 0 && slotEquips[currentSlot])
        {
            slots[currentSlot].SetFire(fire);
        }
    }
    public virtual void SetTarget(Transform target)
    {
        slots[currentSlot].SetTarget(target);
    }
    public virtual void SetTarget(Vector3 target)
    {
        slots[currentSlot].SetTarget(target);
    }
    public virtual ShootState Shoot(Vector3 position, Vector3 direction)
    {
        if (currentSlot >= 0 && slotEquips[currentSlot])
        {
            return slots[currentSlot].Shoot(position, direction);
        }
        return ShootState.none;
    }
    public WeaponBase AddWeapon(WeaponBase weaponPrefab, bool change = true, bool needOverrideSlot = false, int overrideSlot = -1)
    {
        int slot;
        if (!needOverrideSlot)
            slot = weaponPrefab.Slot;
        else
            slot = overrideSlot;
        if(slotEquips.Count > slot && slotEquips[slot])//??????
        {
            Destroy(slots[slot]);
        }
        WeaponBase weapon = Instantiate(weaponPrefab);
        weapon.SetOwner(owner);
        weapon.transform.SetParent(attachSocket);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        if (slots.Count > slot)
            slots[slot] = weapon;
        else
            slots.Insert(slot, weapon);     
        if (slotEquips.Count > slot)
            slotEquips[slot] = true;
        else
            slotEquips.Insert(slot, true);
        if (currentSlot != slot)
        {
            weapon.gameObject.SetActive(false);
            if (change)
                ChangeSlot(slot);
        }
        return weapon;
    }
    public virtual int ChangeSlot(int toSlot)
    {
        if (toSlot >= 0 && toSlot < slotEquips.Count && slotEquips[toSlot]) {
            if (toSlot != currentSlot)
            {
                if (currentSlot >= 0)
                {
                    slots[currentSlot].gameObject.SetActive(false);
                    slots[currentSlot].SetFire(false);
                }

                currentSlot = toSlot;
                slots[currentSlot].gameObject.SetActive(true);
                return 1;
            }
            return 2;
        }
        if (currentSlot >= 0)
        slots[currentSlot].gameObject.SetActive(false);
        currentSlot = -1;
        return 0;
    }
    public Vector3 GetShootPoint()
    {
        if (currentSlot >= 0 && slotEquips[currentSlot])
        {
            return slots[currentSlot].GetShootPoint().position;
        }
        return Vector2.zero;
    }
    public WeaponBase GetWeapon()
    {
        if (currentSlot >= 0 && slotEquips[currentSlot])
        {
            return slots[currentSlot];
        }
        return default;
    }
    public WeaponBase GetWeapon(int slot)
    {
        if (slot >= 0 && slotEquips[slot])
        {
            return slots[slot];
        }
        return default;
    }
    public void StopAll()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slotEquips[i])
            {
                slots[i].damageMultiply = 1.0f;
                slots[i].SetFire(false);
            }
        }
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        jsonObject.Add("current", new JSONNumber(currentSlot));
        JSONArray weaponsJArray = new JSONArray();
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotEquips[i] && slots[i] != null)
                {
                    JSONObject slotJObject = new JSONObject();
                    slotJObject.Add("slot", new JSONNumber(i));
                    //slotJObject.Add("path", new JSONString(slots[i].Prefab.Path));
                    weaponsJArray.Add(slots[i].Save(slotJObject));
                }            
            }
        }
        jsonObject.Add("weapons", weaponsJArray);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        currentSlot = jsonObject["current"].AsInt;
        JSONArray weaponsJArray = jsonObject["weapons"].AsArray;
        {
            for (int i = 0; i < weaponsJArray.Count; i++)
            {
                JSONObject slotJObject = weaponsJArray[i].AsObject;
                int slot = slotJObject["slot"].AsInt;
                if (slotEquips.Count <= slot || !slotEquips[slot])
                    AddWeapon(Resources.Load<WeaponBase>(slotJObject["prefab"]), false,true, slot);              
                slots[slot].Load( slotJObject);
            }
        }
        if(currentSlot >= 0 && currentSlot < slots.Count)
        {
            slots[currentSlot].gameObject.SetActive(true);
        }
        return jsonObject;
    }
}
