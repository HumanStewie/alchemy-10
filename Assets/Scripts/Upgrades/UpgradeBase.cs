using UnityEngine;

public abstract class UpgradeBase : ScriptableObject
{
    public string Name;
    public string Description;

    public abstract void Upgrade();
}