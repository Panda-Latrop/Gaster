using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISaveable : ISaveableComponent
{
    PrefabHolder Prefab { get; }
    GameObject GameObject { get; }
    string SaveTag { get; set; }
   // SaveObjectStruct Save();
   // void Load(SaveObjectStruct saveStruct);
}
public interface ISaveableComponent
{
    JSONObject Save( JSONObject jsonObject);
    JSONObject Load( JSONObject jsonObject);
}
[System.Serializable]
public class SaveData
{
    protected JSONObject data = new JSONObject();

    public string save { get => data["save"]; set => data.Add("save", new JSONString(value)); }

    public void Set(string json)
    {
        data = JSON.Parse(json) as JSONObject;
    }
    public override string ToString()
    {
        return data.ToString();
    }
    public bool GetObjects(string level, out JSONArray objects)
    {
        if (data.HasKey(level)) {
            objects = data[level].AsArray;
            return true;
        }
        else
        {
            objects = new JSONArray();
            objects.Add(level, objects);
            return false;
        }
    }
    public void SetObjects(string level, JSONArray objects)
    {
        if (data.HasKey(level))
        {
            data[level] = objects;
        }
        else
        {
            data.Add(level, objects);
        }
    }
}
/*[System.Serializable]
public struct SaveObjectStruct
{
    public string prefab;
    public string name;
    public string json;
}*/
public class SaveSystem : FileMaster
{
    protected SaveData data = new SaveData();


    protected List<ISaveable> saveableObjects = new List<ISaveable>();
    protected Dictionary<string, ISaveable> sceneObjects = new Dictionary<string, ISaveable>();
    protected Dictionary<string, ISaveable> loadedObjects = new Dictionary<string, ISaveable>();


    protected string directory => Application.persistentDataPath + "/saves/";
    public SaveData Data => data;


    public SaveSystem Prepare(bool inactiveToo)
    {
        foreach (Actor a in Object.FindObjectsOfType<Actor>(inactiveToo))
        {
            if (a.tag.Equals("Saveable"))
                saveableObjects.Add(a);
        }
        return this;
    }
    public SaveSystem Close()
    {
        saveableObjects.Clear();
        return this;
    }
    public SaveSystem DataTo(string file)//"save.sv"
    {
        CreateDirectory(directory);
        WriteTo(directory + file, data.ToString());
        return this;
    }
    public SaveSystem DataFrom(string file)
    {
        data.Set(ReadFrom(directory + file));
        return this;
    }
    public SaveSystem SaveTo(string level, bool byTag = false, string tag = "")
    {
        JSONArray objects = new JSONArray();
        foreach (ISaveable saveable in saveableObjects)
        {
            if (!byTag || saveable.SaveTag.Equals(tag))
            {
                JSONObject obj = new JSONObject();
                objects.Add(saveable.Save( obj));
            }
                       
        }
        data.SetObjects(level, objects);
        return this;
    }
    public SaveSystem LoadFrom(string level, bool byTag = false, string tag = "")
    {
        JSONArray objects;
        if (data.GetObjects(level, out objects))
        {
            foreach (ISaveable saveable in saveableObjects)
            {
                if (!byTag || saveable.SaveTag.Equals(tag))
                    sceneObjects.Add(saveable.GameObject.name, saveable);
            }
            for (int i = 0; i < objects.Count; i++)
            {
                JSONObject item = objects[i].AsObject;
                
                string name = item["name"];
                if (!sceneObjects.ContainsKey(name))
                {
                    //Debug.Log(name);
                    GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(item["prefab"]));
                    go.name = name;
                    loadedObjects.Add(name, go.GetComponent<ISaveable>());
                }
                else
                {
                    loadedObjects.Add(name, sceneObjects[name]);
                    sceneObjects.Remove(name);
                }
            }
            for (int i = 0; i < objects.Count; i++)
            {
                var item = objects[i].AsObject;
                loadedObjects[item["name"]].Load( item);
            }
            foreach (var item in sceneObjects)
            {
                item.Value.GameObject.SetActive(false);
            }
            sceneObjects.Clear();
            loadedObjects.Clear();
        }
        return this;
    }

    public bool GetLoadedObject(string name, ref GameObject go)
    {
        if (loadedObjects.ContainsKey(name))
        {
            go = loadedObjects[name].GameObject;
            return true;
        }
        go = default;
        return false;
    }


    #region Static
    public static JSONObject GameObjectSave(JSONObject jsonObject, GameObject gameObject)
    {
        JSONObject gameObjectJObject = new JSONObject();
        gameObjectJObject.Add("active", new JSONBool(gameObject.activeSelf));
        Transform transform = gameObject.transform;
        TransformSave(gameObjectJObject, string.Empty, transform);
        jsonObject.Add("gameObject", gameObjectJObject);
        return jsonObject;
    }
    public static JSONObject GameObjectLoad(JSONObject jsonObject, GameObject gameObject)
    {
        JSONObject gameObjectJObject = jsonObject["gameObject"].AsObject;
        gameObject.SetActive(gameObjectJObject["active"].AsBool);
        Transform transform = gameObject.transform;
        TransformLoad(gameObjectJObject, string.Empty, transform);
        return jsonObject;
    }
    public static void TransformSave(JSONObject jsonObject, string prefix, Transform transform)
    {

        JSONObject transformJObject = new JSONObject();
        {
            JSONArray position = new JSONArray();
            {
                position.Add(new JSONNumber(transform.localPosition.x));
                position.Add(new JSONNumber(transform.localPosition.y));
                position.Add(new JSONNumber(transform.localPosition.z));
            }
            transformJObject.Add("position", position);
            JSONArray rotation = new JSONArray();
            {
                rotation.Add(new JSONNumber(transform.localRotation.x));
                rotation.Add(new JSONNumber(transform.localRotation.y));
                rotation.Add(new JSONNumber(transform.localRotation.z));
                rotation.Add(new JSONNumber(transform.localRotation.w));
            }
            transformJObject.Add("rotation", rotation);
            JSONArray scale = new JSONArray();
            {
                scale.Add(new JSONNumber(transform.localScale.x));
                scale.Add(new JSONNumber(transform.localScale.y));
                scale.Add(new JSONNumber(transform.localScale.z));
            }
            transformJObject.Add("scale", scale);
        }
        jsonObject.Add(prefix+"Transform", transformJObject);
    }
    public static void TransformLoad(JSONObject jsonObject, string prefix, Transform transform)
    {
        JSONObject transformJObject = jsonObject[prefix+"Transform"].AsObject;
        {
            JSONArray position = transformJObject["position"].AsArray;
            {
                Vector3 positionV3 = new Vector3(position[0].AsFloat, position[1].AsFloat, position[2].AsFloat);
                transform.localPosition = positionV3;
            }
            JSONArray rotation = transformJObject["rotation"].AsArray;
            {
                Quaternion rotationQ = new Quaternion(rotation[0].AsFloat, rotation[1].AsFloat, rotation[2].AsFloat, rotation[3].AsFloat);
                transform.localRotation = rotationQ;
            }
            JSONArray scale = transformJObject["scale"].AsArray;
            {
                Vector3 scaleV3 = new Vector3(scale[0].AsFloat, scale[1].AsFloat, scale[2].AsFloat);
                transform.localScale = scale;
            }
        }
    }
    public static void GameObjectReferenceSave(JSONObject jsonObject, string prefix, GameObject reference)
    {
        //Debug.Log("GameObjectReferenceSave " + reference.GetComponentInParent<ISaveable>().GameObject.name);
        //jsonObject.Add(prefix + "gameobject", new JSONString(reference.transform.root.name));
        jsonObject.Add(prefix + "Gameobject", new JSONString(reference.GetComponentInParent<ISaveable>().GameObject.name));
    }
    public static bool GameObjectReferenceLoad(JSONObject jsonObject, string prefix, ref GameObject reference)
    {
        return GameInstance.Instance.GetLoadedObject(jsonObject[prefix + "Gameobject"], ref reference);
    }
    public static JSONObject ComponentReferenceSave(JSONObject jsonObject, string prefix, Component reference)
    {
        //Debug.Log("ComponentReferenceSave " + reference.GetComponentInParent<ISaveable>().GameObject.name);
        //jsonObject.Add(prefix + "reference", new JSONString(reference.transform.root.name));
        //if (reference.GetComponentInParent<ISaveable>() == null)
        //    Debug.LogError("Null " + jsonObject.ToString() + " "+ reference.name);
        jsonObject.Add(prefix + "Reference", new JSONString(reference.GetComponentInParent<ISaveable>().GameObject.name));
        return jsonObject;
    }
    public static bool ComponentReferenceLoad<T>(JSONObject jsonObject, string prefix, ref T reference)
    {
        GameObject go = null;
        
        //Debug.Log("1 " + jsonObject[prefix + "reference"]);
        if (GameInstance.Instance.GetLoadedObject(jsonObject[prefix + "Reference"], ref go))
        {
            //Debug.Log(go.name + " 3 " + jsonObject[prefix + "reference"]);
            reference = go.GetComponentInChildren<T>();
            return true;
        }
        else
        {
            reference = default;
            return false;
        }    
    }
    public static void ColliderSave(JSONObject jsonObject, string prefix, Collider2D collider)
    {
        JSONObject colliderJObject = new JSONObject();
        colliderJObject.Add("enabled", collider.enabled);
        colliderJObject.Add("isTrigger", collider.isTrigger);
        jsonObject.Add(prefix+"Collider", colliderJObject);
    }
    public static void ColliderLoad(JSONObject jsonObject, string prefix, Collider2D collider)
    {
        JSONObject colliderJObject = jsonObject[prefix+"Collider"].AsObject;
        collider.enabled = colliderJObject["enabled"].AsBool;
        collider.isTrigger = colliderJObject["isTrigger"].AsBool;
    }
    public static JSONObject RigidbodySave(JSONObject jsonObject, Rigidbody2D rigidbody)
    {
        JSONObject rigidbodyJObject = new JSONObject();
        rigidbodyJObject.Add("simulated", new JSONBool(rigidbody.simulated));
        JSONArray velocityJArray = new JSONArray();
        velocityJArray.Add(new JSONNumber(rigidbody.velocity.x));
        velocityJArray.Add(new JSONNumber(rigidbody.velocity.y));
        rigidbodyJObject.Add("velocity", velocityJArray);
        rigidbodyJObject.Add("angularVelocity", new JSONNumber(rigidbody.angularVelocity));
        rigidbodyJObject.Add("inertia", new JSONNumber(rigidbody.inertia));
        rigidbodyJObject.Add("sleepMode", new JSONNumber((int)rigidbody.sleepMode));
        jsonObject.Add("rigidbody", rigidbodyJObject);
        return jsonObject;
    }
    public static JSONObject RigidbodyLoad(JSONObject jsonObject, Rigidbody2D rigidbody)
    {
        JSONObject rigidbodyJObject = jsonObject["rigidbody"].AsObject;
        rigidbody.simulated = rigidbodyJObject["simulated"].AsBool;
        JSONArray velocityJArray = rigidbodyJObject["velocity"].AsArray;
        rigidbody.velocity = new Vector2(velocityJArray[0].AsFloat, velocityJArray[1].AsFloat);
        rigidbody.angularVelocity = rigidbodyJObject["angularVelocity"].AsFloat;
        rigidbody.inertia = rigidbodyJObject["inertia"].AsFloat;
        rigidbody.sleepMode = (RigidbodySleepMode2D)rigidbodyJObject["sleepMode"].AsInt;
        return jsonObject;
    }
    public static void TimerSave(JSONObject jsonObject, string prefix, float timer)
    {
        //Debug.Log("Save " + " " +prefix+" " + (timer - Time.time));
        jsonObject.Add(prefix + "Timer", new JSONNumber(timer - Time.time));
    }
    public static void TimerLoad(JSONObject jsonObject, string prefix,ref float timer)
    {
        //Debug.Log("Load " + " " + prefix + " " + jsonObject[prefix + "timer"].AsFloat);
        timer = jsonObject[prefix + "Timer"].AsFloat + Time.time;
    }
    public static void ParticleSystemSave(JSONObject jsonObject, string prefix, ParticleSystem particleSystem)
    {
        JSONObject particleJObject = new JSONObject();
        particleJObject.Add("isPlaying", new JSONBool(particleSystem.isPlaying));
        particleJObject.Add("randomSeed", new JSONNumber(particleSystem.randomSeed));
        particleJObject.Add("time", new JSONNumber(particleSystem.time));
        jsonObject.Add(prefix+"Particle", particleJObject);
    }
    public static void ParticleSystemLoad(JSONObject jsonObject, string prefix, ParticleSystem particleSystem)
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        JSONObject particleJObject = jsonObject[prefix+"Particle"].AsObject;
        particleSystem.randomSeed = particleJObject["randomSeed"].AsUInt;
        if (particleJObject["isPlaying"].AsBool)
        {
            particleSystem.Simulate(particleJObject["time"].AsFloat, true);
            particleSystem.Play(true);
        }               
    }
    public static void ParticleSystemSave(JSONObject jsonObject, string prefix, ParticleSystem particleSystem,List<ParticleSystem> children)
    {

        JSONObject particleJObject = new JSONObject();
        particleJObject.Add("isPlaying", new JSONBool(particleSystem.isPlaying));
        particleJObject.Add("randomSeed", new JSONNumber(particleSystem.randomSeed));
        particleJObject.Add("time", new JSONNumber(particleSystem.time));    
        JSONArray childrenJArray = new JSONArray();
        {
            for (int i = 0; i < children.Count; i++)
            {
                //Debug.Log(children[i].randomSeed);
                childrenJArray.Add(new JSONNumber(children[i].randomSeed));
            }
        }
        particleJObject.Add("children", childrenJArray);
        jsonObject.Add(prefix + "Particle", particleJObject);
    }
    public static void ParticleSystemLoad(JSONObject jsonObject, string prefix, ParticleSystem particleSystem, List<ParticleSystem> children)
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        JSONObject particleJObject = jsonObject[prefix + "Particle"].AsObject;
        particleSystem.randomSeed = particleJObject["randomSeed"].AsUInt;
        JSONArray childrenJArray = particleJObject["children"].AsArray;
        {
            for (int i = 0; i < childrenJArray.Count; i++)
            {
                children[i].randomSeed = childrenJArray[i].AsUInt;
            }
        }
        if (particleJObject["isPlaying"].AsBool)
        {
            particleSystem.Simulate(particleJObject["time"].AsFloat, true);
            particleSystem.Play(true);
        }
    }
    public static void AnimatorSave(JSONObject jsonObject, string prefix, Animator animator)
    {
        JSONObject animatorJObject = new JSONObject();
        JSONArray layersJArray = new JSONArray();
        for (int i = 0; i < animator.layerCount; i++)
        {
            JSONObject layerJArray = new JSONObject();
            var animState = animator.GetCurrentAnimatorStateInfo(i);
            JSONArray animStateJObject = new JSONArray();
            animStateJObject.Add(new JSONNumber(animState.fullPathHash));
            animStateJObject.Add(new JSONNumber(animState.normalizedTime));
            layerJArray.Add("animState", animStateJObject);

            layersJArray.Add(layerJArray);
        }
        animatorJObject.Add("layers", layersJArray);

        JSONArray parametsJArray = new JSONArray();
        for (int i = 0; i < animator.parameterCount; i++)
        {
            int nameHash = animator.GetParameter(i).nameHash;
            AnimatorControllerParameterType type = animator.GetParameter(i).type;
            if (type == AnimatorControllerParameterType.Trigger)
                continue;
            JSONObject parameterJObject = new JSONObject();
            parameterJObject.Add("nameHash", new JSONNumber(nameHash));
            parameterJObject.Add("type", new JSONNumber((int)animator.GetParameter(i).type));
            switch (type)
            {
                case AnimatorControllerParameterType.Float:
                    parameterJObject.Add("value", new JSONNumber(animator.GetFloat(nameHash)));
                    break;
                case AnimatorControllerParameterType.Int:
                    parameterJObject.Add("value", new JSONNumber(animator.GetInteger(nameHash)));
                    break;
                case AnimatorControllerParameterType.Bool:
                    parameterJObject.Add("value", new JSONBool(animator.GetBool(nameHash)));
                    break;
            }
            parametsJArray.Add(parameterJObject);
        }
        jsonObject.Add("parameters", parametsJArray);

        jsonObject.Add(prefix+"Animator", animatorJObject);
    }
    public static void AnimatorLoad(JSONObject jsonObject, string prefix, Animator animator)
    {       
        JSONObject animatorJObject = jsonObject[prefix+"Animator"].AsObject;
        JSONArray layersJArray = animatorJObject["layers"].AsArray;
        for (int i = 0; i < layersJArray.Count; i++)
        {
            JSONObject layerJArray = layersJArray[i].AsObject;
            JSONArray animStateJArray = layerJArray["animState"].AsArray;
            animator.Play(animStateJArray[0].AsInt, i, animStateJArray[1].AsFloat);         
        }
        JSONArray parametsJArray = jsonObject["parameters"].AsArray;
        for (int i = 0; i < parametsJArray.Count; i++)
        {
            JSONObject parameterJObject = parametsJArray[i].AsObject;
            int nameHash = parameterJObject["nameHash"].AsInt;
            AnimatorControllerParameterType type = (AnimatorControllerParameterType)parameterJObject["type"].AsInt; ;         
            switch (type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(nameHash, parameterJObject["value"].AsFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(nameHash, parameterJObject["value"].AsInt);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(nameHash, parameterJObject["value"].AsBool);
                    break;
            }
        }
    }
    public static void AudioSourceSave(JSONObject jsonObject, string prefix, AudioSource audioSource)
    {
        JSONObject audioJObject = new JSONObject();
        //Debug.Log(audioSource.enabled);
        audioJObject.Add("enabled", new JSONBool(audioSource.enabled));
        audioJObject.Add("isPlaying", new JSONBool(audioSource.isPlaying));
        audioJObject.Add("loop", new JSONBool(audioSource.loop));
        audioJObject.Add("mute", new JSONBool(audioSource.mute));
        audioJObject.Add("volume", new JSONNumber(audioSource.volume));
        audioJObject.Add("time", new JSONNumber(audioSource.time));
        jsonObject.Add(prefix+"Audio", audioJObject);
    }
    public static void AudioSourceLoad(JSONObject jsonObject, string prefix, AudioSource audioSource)
    {
        JSONObject audioJObject = jsonObject[prefix+"Audio"].AsObject;
        //Debug.Log(audioJObject["enabled"].AsBool);
        audioSource.enabled = audioJObject["enabled"].AsBool;
        //audioSource.Stop();
        if (audioJObject["isPlaying"].AsBool)
        {
            audioSource.Play();
            audioSource.time = audioJObject["time"].AsFloat;
        }
        else
        {
            audioSource.Stop();
            audioSource.time = 0.0f;
        }
        audioSource.loop = audioJObject["loop"].AsBool;
        audioSource.mute = audioJObject["mute"].AsBool;
        audioSource.volume = audioJObject["volume"].AsFloat;
    }
    public static void LineRendererSave(JSONObject jsonObject, string prefix, LineRenderer lineRenderer)
    {
        JSONObject lineJObject = new JSONObject();
        lineJObject.Add("enabled", new JSONBool(lineRenderer.enabled));
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        JSONArray positionsJArray = new JSONArray();
        {
            for (int i = 0; i < positions.Length; i++)
            {
                JSONArray positionJArray = new JSONArray();
                positionJArray.Add(positions[i].x);
                positionJArray.Add(positions[i].y);
                positionJArray.Add(positions[i].z);
                positionsJArray.Add(positionJArray);
            }
        }
        lineJObject.Add("positions", positionsJArray);        
        jsonObject.Add(prefix + "LineR", lineJObject);
    }
    public static void LineRendererLoad(JSONObject jsonObject, string prefix, LineRenderer lineRenderer)
    {
        JSONObject lineJObject = jsonObject[prefix + "LineR"].AsObject;
        lineRenderer.enabled = lineJObject["enabled"].AsBool;
        JSONArray positionsJArray = lineJObject["positions"].AsArray;
        Vector3[] positions = new Vector3[positionsJArray.Count];
        {
            for (int i = 0; i < positionsJArray.Count; i++)
            {
                JSONArray positionJArray = positionsJArray[i].AsArray;
                positions[i].Set(positionJArray[0].AsFloat, positionJArray[1].AsFloat, positionJArray[2].AsFloat);
            }
        }
        lineRenderer.GetPositions(positions);
    }
    #endregion




}
