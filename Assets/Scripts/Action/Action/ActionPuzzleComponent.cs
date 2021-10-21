using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPuzzleComponent : ActionBaseComponent
{
    public enum ActionPuzzleType
    {
        set,
        add,
        check,
    }
    [SerializeField]
    protected PuzzleExecutor puzzle;
    [SerializeField]
    protected ActionPuzzleType type;
    [SerializeField]
    protected string solution;
    public override void OnEnter()
    {
        base.OnEnter();
        switch (type)
        {
            case ActionPuzzleType.set:
                puzzle.SetSolution(solution);
                break;
            case ActionPuzzleType.add:
                puzzle.AddSolution(solution);
                break;
            case ActionPuzzleType.check:
                puzzle.CheckSolution();
                break;
            default:
                break;
        }
    }
    public override bool OnUpdate()
    {
        return true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (puzzle != null)
        {
            Gizmos.DrawLine(transform.position, puzzle.transform.position);
        }

    }
}
