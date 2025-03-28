using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelList", fileName = "LevelList")]
public class LeveListSO : ScriptableObject
{
   public List<LevelSO> Levels;
}
