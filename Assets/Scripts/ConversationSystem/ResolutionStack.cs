using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResolutionStack
{
    // customized stack data structure
    private List<Speech> stack;
    public int memorySize = 5;

    public ResolutionStack()
    {
        stack = new List<Speech>();
    }

    public void Push(Speech speech)
    {
        stack.Add(speech);
        if(stack.Count > memorySize)
        {
            stack.RemoveAt(0);
        }
    }

    public Speech Peek()
    {
        if (stack.Count == 0) return null;
        return stack[stack.Count - 1];
    }

    public Speech Pop()
    {
        Speech last = Peek();
        stack.RemoveAt(stack.Count - 1);
        return last;
    }

    public void Swap(Speech speech)
    {
        Pop();
        Push(speech);
    }
}
