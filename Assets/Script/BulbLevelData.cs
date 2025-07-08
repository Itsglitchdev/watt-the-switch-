using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulbLevels", menuName = "BulbGame/BulbLevels")]
public class BulbLevelData : ScriptableObject
{
    public List<LevelData> Levels = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    public BulbState[] bulbs = new BulbState[8];
}


public enum BulbState
{
    On,
    Off
}
