using UnityEngine;

[System.Serializable]
public struct OptionalBool
{
    public bool willOverrideBoolValue;   // Should this upgrade modify the bool?
    public bool onForTrue;           // The value to apply if overriding
}
