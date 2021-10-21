using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : MonoBehaviour
{
    [SerializeField]
    protected string path;
    public string Path { get => path; }
}
