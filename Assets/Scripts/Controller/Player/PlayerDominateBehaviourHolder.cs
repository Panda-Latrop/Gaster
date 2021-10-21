using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDominateBehaviourHolder : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected PlayerDominateController controller;
    [SerializeField]
    protected List<PlayerDominateBehaviourBaseComponent> slots = new List<PlayerDominateBehaviourBaseComponent>();
    [SerializeField]
    protected List<bool> slotEquips = new List<bool>();
    [SerializeField]
    protected int currentSlot = 0;

    protected virtual void Start()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetPlayerController(controller);
        }
    }
    public int CurrentSlot => currentSlot;
    public PlayerDominateBehaviourBaseComponent AddBehaviour(PlayerDominateBehaviourBaseComponent prefab, bool change = true, bool needOverrideSlot = false, int overrideSlot = -1)
    {
        int slot;
        if (!needOverrideSlot)
            slot = prefab.Slot;
        else
            slot = overrideSlot;
        PlayerDominateBehaviourBaseComponent behaviour = Instantiate(prefab);
        behaviour.transform.SetParent(transform);
        behaviour.transform.localPosition = Vector3.zero;
        behaviour.SetPlayerController(controller);
        if (slots.Count > slot)
            slots[slot] = behaviour;
        else
            slots.Insert(slot, behaviour);
        if (slotEquips.Count > slot)
            slotEquips[slot] = true;
        else
            slotEquips.Insert(slot, true);
        return behaviour;
    }
   
    public virtual bool ChangeSlot(int toSlot)
    {
        if (toSlot >= 0 && toSlot < slotEquips.Count && slotEquips[toSlot])
        {
            currentSlot = toSlot;
            return true;
        }
        return false;
    }
    public virtual bool ChangeSlotPrev()
    {
        int slot = currentSlot - 1;
        if (slot < 0)
            slot = 0;
        return ChangeSlot(slot);
    }
    public virtual bool ChangeSlotNext()
    {
        int slot = currentSlot + 1;
        if(slot >= slotEquips.Count)

            slot = 0;
        return ChangeSlot(slot);
    }

    public void Execute(int action)
    {
        if (currentSlot >= 0 && slotEquips[currentSlot] && !slots[currentSlot].CanUpdate())
            slots[currentSlot].Execute(action);
    }

    public void OnUpdate()
    {
        for (int i = 0; i < slots.Count && i < slotEquips.Count; i++)
        {
            if(slotEquips[i] && slots[i].CanUpdate())
            {
                slots[i].OnUpdate();
            }
        }
    }

    public void OnLateUpdate()
    {
        for (int i = 0; i < slots.Count && i < slotEquips.Count; i++)
        {
            if (slotEquips[i] && slots[i].CanUpdate())
            {
                slots[i].OnLateUpdate();
            }
        }
    }

    public virtual JSONObject Save(JSONObject jsonObject)
    {
        jsonObject.Add("current", new JSONNumber(currentSlot));
        JSONArray behavioursJArray = new JSONArray();
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slotEquips[i] && slots[i] != null)
                {
                    JSONObject slotJObject = new JSONObject();
                    slotJObject.Add("slot", new JSONNumber(i));
                    behavioursJArray.Add(slots[i].Save(slotJObject));
                }
            }
        }
        jsonObject.Add("behaviours", behavioursJArray);
        return jsonObject;
    }
    public virtual JSONObject Load(JSONObject jsonObject)
    {
        currentSlot = jsonObject["current"].AsInt;
        JSONArray behavioursJArray = jsonObject["behaviours"].AsArray;
        {
            for (int i = 0; i < behavioursJArray.Count; i++)
            {
                JSONObject slotJObject = behavioursJArray[i].AsObject;
                int slot = slotJObject["slot"].AsInt;
                if (slotEquips.Count <= slot || !slotEquips[slot])
                    AddBehaviour(Resources.Load<PlayerDominateBehaviourBaseComponent>(slotJObject["prefab"]), false, true, slot);
                slots[slot].Load(slotJObject);
            }
        }
        return jsonObject;
    }
}
