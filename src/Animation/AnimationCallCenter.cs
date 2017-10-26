using System.Collections.Generic;
using UnityEngine;

public class AnimationCallCenter
{
    private Dictionary<string, System.Action> _finishCallbacks = new Dictionary<string, System.Action>();
    private string _recieverFunctionName = "";

    public AnimationCallCenter(string recieverFunctionName)
    {
        _recieverFunctionName = recieverFunctionName;
    }


    public void Call(string animationName)
    {
        if (!_finishCallbacks.ContainsKey(animationName))
            return;

        _finishCallbacks[animationName]();
    }

    public void Register(AnimationState animationState, System.Action finishCallback)
    {
        MakeFinishEvent(animationState);
        _finishCallbacks[animationState.name] = finishCallback;
    }

    private void MakeFinishEvent(AnimationState animationState)
    {
        if (HasFinishEvent(animationState))
            return;

        var finishEvent = new AnimationEvent();
        finishEvent.functionName = _recieverFunctionName;
        finishEvent.stringParameter = animationState.name;
        finishEvent.time = animationState.length;
        animationState.clip.AddEvent(finishEvent);
    }

    private bool HasFinishEvent(AnimationState animationState)
    {
        var animationEvents = animationState.clip.events;
        foreach (var aimationEvent in animationEvents)
        {
            if (aimationEvent.functionName == _recieverFunctionName)
                return true;
        }
        return false;
    }
}
