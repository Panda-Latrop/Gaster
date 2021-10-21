using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractiveExecutor
{
    void Execute();
    bool IsClosed { get; }
    void Close();
    void Open();
}
public class InteractiveExecutor : ComplexExecutor, IInteractiveExecutor
{
}
