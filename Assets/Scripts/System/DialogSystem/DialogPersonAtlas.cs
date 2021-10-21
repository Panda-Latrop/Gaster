using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Person Atlas", menuName = "System/Dialog/Person Atlas", order = 1)]
public class DialogPersonAtlas : ScriptableObject
{
    [System.Serializable]
    public class DialogPersonClass : IComparable<DialogPersonClass>
    {
        [SerializeField]
        protected string name;
        [SerializeField]
        protected Sprite[] faces;
        [SerializeField]
        protected AudioClip[] sounds;

        public string Name => name;
        public int CompareTo(DialogPersonClass obj)
        {
            return name.CompareTo(obj.name);
        }
        public Sprite Face(int face)
        {
            return faces[face];
        }
        public AudioClip Sound(int sound)
        {
            return sounds[sound];
        }
    }

    [SerializeField]
    protected DialogPersonClass[] persons;

    [ContextMenu("Sort")]
    public void Sort()
    {
        Array.Sort(persons);
    }

    public DialogPersonClass GetPerson(string person, out Sprite face, out AudioClip sound)
    {
        string[] str = person.Split('-');
        DialogPersonClass dpc = Array.Find(persons, p => p.Name.Equals(str[0]));
        if (dpc != null)
        {
            if (str.Length >= 2)
                face = dpc.Face(int.Parse(str[1]));
            else
                face = dpc.Face(0);
            if (str.Length >= 3)
                sound = dpc.Sound(int.Parse(str[2]));
            else
                sound = dpc.Sound(0);
            return dpc;
        }
        else
        {
            dpc = persons[0];
            face = dpc.Face(0);
            sound = dpc.Sound(0);
        }
        return dpc;
    }
}