using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmyControllerComponent : MonoBehaviour, ISaveableComponent
{
    [SerializeField]
    protected PlayerController playerController;
    protected Character player;

    [SerializeField]
    protected List<CharacterAttacker> characters = new List<CharacterAttacker>();

    [Header("Heal")]
    [SerializeField]
    protected float distanceToHeal = 3.0f;
    protected bool hasToHeal;
    protected CharacterAttacker characterToHeal;
    [SerializeField]
    protected float timeToKillHeal;
    protected float nextKillHeal;
    [Header("Bomb")]
    protected bool hasToBomb;
    protected CharacterAttacker characterToBomb;
    protected Vector3 pointToBomb;
    [SerializeField]
    protected float timeToKillBomb;
    protected float nextKillBomb;
    [Header("Teleport")]
    protected bool hasToTeleport;
    protected Vector2 sizeTeleport;
    protected Vector3 pointToTeleport;
    [Header("Lightning")]
    protected bool hasToLightning;
    protected CharacterAttacker[] charactersToLightning = new CharacterAttacker[2];
    protected Vector3[] pointsToLightning = new Vector3[2];
    [SerializeField]
    protected float timeToLightning, timeToKillLightning;
    protected float nextLightning, nextKillLightning;

    public void SetPlayer(Character player)
    {
        this.player = player;
    }
    public void Add(CharacterAttacker character)
    {
        character.Health.Team = player.Health.Team;
        characters.Add(character);
    }
    protected bool ClosestCharacter(ref CharacterAttacker characterAttacker)
    {
        float minM = float.MaxValue;
        float minTmp;
        bool hasCharacter = false;
        for (int i = 0; i < characters.Count; i++)
        {
            minTmp = (characters[i].transform.position - player.transform.position).sqrMagnitude;
            if (minM > minTmp)
            {
                hasCharacter = true;
                minM = minTmp;
                characterAttacker = characters[i];
            }
        }
        return hasCharacter;
    }

    #region Heal
    public void MarkToHeal()
    {
        if(!hasToHeal && characters.Count > 0)
        {
            ChagneToHeal();
        }
    }
    protected void ChagneToHeal()
    {
        ClosestCharacter(ref characterToHeal);
        while (!characterToHeal.Health.IsAlive)
        {
            characters.Remove(characterToHeal);
            if (!ClosestCharacter(ref characterToHeal))
            {
                hasToHeal = false;
                return;
            }
        }
        characterToHeal.Controller.Unpossess();
        characterToHeal.OnPossess(playerController);
        characterToHeal.SpriteRendererHolder.OnHeal();
        hasToHeal = true;
        return;
    }
    #endregion
    protected void Update()
    {
        if (hasToHeal && characterToHeal.Health.IsAlive)
        {
            float distance = (player.transform.position - characterToHeal.transform.position).sqrMagnitude;
            Vector3 direction = (player.transform.position - characterToHeal.transform.position).normalized;

            if (distance > distanceToHeal * distanceToHeal)
            {
                characterToHeal.CharacterMovement.speedMultiply = 4.0f;
                characterToHeal.CharacterMovement.FollowTarget(player, distance, direction);
            }
            else
            {
                player.Health.Heal(25);
                characterToHeal.Health.Kill();
                hasToHeal = false;
                characterToHeal = null;
            }

        }
    }
    protected void LateUpdate()
    {
        if (hasToHeal && !characterToHeal.Health.IsAlive)
        {
            hasToHeal = false;
            characterToHeal = null;
        }
        for (int i = 0; i < characters.Count; i++)
        {
            if (!characters[i].Health.IsAlive)
            {
                characters.RemoveAt(i);
                i--;
            }
        }
    }
    public virtual JSONObject Save( JSONObject jsonObject)
    {
        JSONObject armyController = new JSONObject();
        armyController.Add("hasToHeal", new JSONBool(hasToHeal));
        if (hasToHeal)
            SaveSystem.ComponentReferenceSave(armyController, "toHeal", characterToHeal);
        JSONArray targetsJArray = new JSONArray();
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].Health.IsAlive)
                {
                    JSONObject targetJObject = new JSONObject();
                    SaveSystem.ComponentReferenceSave(targetJObject, "character", characters[i]);
                    targetsJArray.Add(targetJObject);
                }
            }
        }
        armyController.Add("characters", targetsJArray);
        jsonObject.Add("armyController", armyController);
        return jsonObject;
    }
    public virtual JSONObject Load( JSONObject jsonObject)
    {
        JSONObject armyController = jsonObject["armyController"].AsObject;
        hasToHeal = armyController["hasToHeal"].AsBool;
        if (hasToHeal)
            SaveSystem.ComponentReferenceLoad(armyController, "toHeal", ref characterToHeal);
        JSONArray targetsJArray = armyController["characters"].AsArray;
        {
            for (int i = 0; i < targetsJArray.Count; i++)
            {
                JSONObject targetJObject = targetsJArray[i].AsObject;
                CharacterAttacker character = default;
                SaveSystem.ComponentReferenceLoad(targetJObject, "character", ref character);
                characters.Add(character);
            }
        }
        return jsonObject;
    }
}
