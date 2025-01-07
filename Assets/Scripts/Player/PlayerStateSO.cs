using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateSO : ScriptableObject
{
    protected Dictionary<PlayerController, object> _instanceData = new Dictionary<PlayerController, object>();

    public T GetInstanceData<T>(PlayerController controller) where T : class, new()
    {
        if (!_instanceData.ContainsKey(controller))
        {
            _instanceData.Add(controller, new T());
        }
        return _instanceData[controller] as T;
    }

    public void RemoveInstanceData(PlayerController controller)
    {
        _instanceData.Remove(controller);
    }

    public void ClearInstanceData()
    {
        _instanceData.Clear();
    }


    public abstract void OnStateBegin(PlayerController target, PlayerStateSO previousState);
    public abstract void OnStateEnd(PlayerController target, PlayerStateSO nextState);
    public abstract void UpdateState(PlayerController target);
    public abstract void PhysicsUpdateState(PlayerController target);
}
