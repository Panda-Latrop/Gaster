using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PuzzleExecutor : ComplexExecutor
{
    [SerializeField]
    protected string puzzleKey;
    protected StringBuilder solution = new StringBuilder();


    public void SetSolution(string set)
    {
        if (!isClosed)
        {
            solution.Clear();
            AddSolution(set);
        }
    }
    public void AddSolution(string add)
    {
        if (!isClosed)
        {
            solution.Append(add);
        }
    }
    public bool CheckSolution()
    {
        bool answer = false;
        if (!IsClosed)
        {
            answer = puzzleKey.Equals(solution.ToString());
            solution.Clear();
            if (answer)
            {
                if (enabled && currentExecutor != 0)
                    Stop();
                currentExecutor = 0;
                Execute();
                if (closeOnExecute)
                    Close();
            }
            else
            {
                if (enabled && currentExecutor != 1)
                    Stop();
                currentExecutor = 1;
                Execute();
                Open();
            }
        }
        return answer;
    }

    public override JSONObject Save( JSONObject jsonObject)
    {

        base.Save( jsonObject);
        jsonObject.Add("solution", new JSONString(solution.ToString()));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        string jo = jsonObject["solution"];
        solution.Clear();
        solution.Append(jo);   
        return jsonObject;
    }
}
