using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerConstraintSO : ScriptableObject
{
    protected Dictionary<PlayerController, object> _instanceData = new();

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

    public abstract bool IsValid(PlayerController target);
}