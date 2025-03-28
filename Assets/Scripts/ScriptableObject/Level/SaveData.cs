using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 position;
    public Vector3 rotation;
    public int Id;

    public SaveData(Vector3 pos,Vector3 rot,int id  = -1)
    {
        position = pos;
        rotation = rot;
        Id = id;
    }
    
}