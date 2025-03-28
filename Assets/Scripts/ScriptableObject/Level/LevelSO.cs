using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level", fileName = "Level",order = 1)]
public class LevelSO : ScriptableObject
{
    public List<SaveData> grids;
    public List<SaveData> walls;
    public List<SaveData> wallEdges;
    public List<SaveData> blocks;
    public List<SaveData> passages;
    public List<SaveDataList> stickMans;
    public List<SaveDataList> wagons;
    [Space] 
    public int minute = 85;
}
