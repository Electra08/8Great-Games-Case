using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int currentLevel = -1;
    public LeveListSO LevelData;
    [Header("Level Data")]
    public Color[] normalColors;
    public Color[] selectedColors;
    public Material[] materials;
    
    [Header("Time")] 
    public int minute;

    [Header("Data")] 
    public Transform gridTransform;
    public Transform wallTransform;
    public Transform wallEdgeTransform;
    public Transform blockTransform;
    public Transform passageTransform;
    public List<Transform> rollerCoasters;
    
    [Header("Prefabs")] 
    public GameObject gridPrefab;
    public GameObject wallPrefab;
    public GameObject wallEdgePrefab;
    public GameObject blockPrefab;
    public GameObject passagePrefab;
    public GameObject wagonHeadPrefab;
    public GameObject wagonMidPrefab;
    public GameObject wagonTailPrefab;
    public GameObject stickManPrefab;

    private void Awake()
    {
        Instance = this;
    }
    
    public void addTime()
    {
        minute += 5;
        UIManager.Instance.HandleTimeText(minute);
    }
    
    public void minusTime()
    {
        minute -= 5;
        UIManager.Instance.HandleTimeText(minute);
    }

    public void RollerCoasterObj(GameObject obj)
    {
        rollerCoasters.Add(obj.transform);
    }
    
    public void ResetLevel()
    {
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in wallTransform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in wallEdgeTransform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in passageTransform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Transform child in blockTransform)
        {
            Destroy(child.gameObject);
        }

        var _rollerCoasters = GameObject.FindObjectsOfType<RollerCoaster>(); 
        foreach (var rollerCoaster in _rollerCoasters)
        {
            Destroy(rollerCoaster.gameObject);
        }
        rollerCoasters.Clear();
        if(GameManager.Instance != null) GameManager.Instance.RollerCoasters.Clear();
        
        minute = 85;
        GameObject baseGrid = Instantiate(gridPrefab,gridTransform);
        baseGrid.transform.localPosition = Vector3.zero;
    }
    
    public void SaveLevel()
    {
#if UNITY_EDITOR
        LevelSO levelSo = ScriptableObject.CreateInstance<LevelSO>();

        levelSo.grids = new List<SaveData>(gridTransform.childCount);
        for (int i = 0; i < gridTransform.childCount; i++)
        {
            Transform child = gridTransform.GetChild(i);
            SaveData saveData = new SaveData(child.position,child.eulerAngles);
            levelSo.grids.Add(saveData);
        }
        
        levelSo.walls = new List<SaveData>(wallTransform.childCount);
        for (int i = 0; i < wallTransform.childCount; i++)
        {
            Transform child = wallTransform.GetChild(i);
            SaveData saveData = new SaveData(child.position,child.eulerAngles);
            levelSo.walls.Add(saveData);
        }
        
        levelSo.wallEdges = new List<SaveData>(wallEdgeTransform.childCount);
        for (int i = 0; i < wallEdgeTransform.childCount; i++)
        {
            Transform child = wallEdgeTransform.GetChild(i);
            SaveData saveData = new SaveData(child.position,child.eulerAngles);
            levelSo.wallEdges.Add(saveData);
        }
        
        levelSo.passages = new List<SaveData>(passageTransform.childCount);
        for (int i = 0; i < passageTransform.childCount; i++)
        {
            Transform child = passageTransform.GetChild(i);
            SaveData saveData = new SaveData(child.position,child.eulerAngles);
            levelSo.passages.Add(saveData);
        }
        
        levelSo.blocks = new List<SaveData>(blockTransform.childCount);
        for (int i = 0; i < blockTransform.childCount; i++)
        {
            Transform child = blockTransform.GetChild(i);
            SaveData saveData = new SaveData(child.position,child.eulerAngles);
            levelSo.blocks.Add(saveData);
        }

        levelSo.wagons = new List<SaveDataList>();
        foreach (var wagon in rollerCoasters)
        {
            SaveDataList wagonList = new SaveDataList();
            wagonList.List = new List<SaveData>();
            for (int j = 0; j < wagon.childCount; j++)
            {
                Transform child = wagon.GetChild(j);
                SaveData saveData = new SaveData(child.position,child.eulerAngles,child.GetComponent<Wagon>().Id);
                wagonList.List.Add(saveData);
            }
            levelSo.wagons.Add(wagonList);
        }

        levelSo.stickMans = new List<SaveDataList>();
        for (int i = 0; i < passageTransform.childCount; i++)
        {
            var passage = passageTransform.GetChild(i);

            SaveDataList saveDataList = new SaveDataList();
            saveDataList.List = new List<SaveData>();
            for (int j = 0; j < passage.GetChild(0).childCount; j++)
            {
                Transform child = passage.GetChild(0).GetChild(j);
                SaveData saveData = new SaveData(child.position,child.eulerAngles,child.GetComponent<StickMan>().Id);
                saveDataList.List.Add(saveData);
            }
            levelSo.stickMans.Add(saveDataList);
        }
        
        levelSo.minute = minute;
        LevelData.Levels.Add(levelSo);
        string filePath = $"Assets/ScriptableObject/Levels/LevelData{LevelData.Levels.Count}.asset";
        AssetDatabase.CreateAsset(levelSo, filePath);
#endif
    }

    public void LoadLevel(LevelSO levelSo)
    {
        ResetLevel();

        DOVirtual.DelayedCall(0.2f, () =>
        {
            foreach (var saveData in levelSo.grids)
            {
                GameObject grid = Instantiate(gridPrefab, saveData.position, Quaternion.Euler(saveData.rotation), gridTransform);
            }

            foreach (var saveData in levelSo.walls)
            {
                GameObject wall = Instantiate(wallPrefab, saveData.position, Quaternion.Euler(saveData.rotation), wallTransform);
            }

            foreach (var saveData in levelSo.wallEdges)
            {
                GameObject wallEdge = Instantiate(wallEdgePrefab, saveData.position, Quaternion.Euler(saveData.rotation), wallEdgeTransform);
            }

            foreach (var saveData in levelSo.passages)
            {
                GameObject passage = Instantiate(passagePrefab, saveData.position, Quaternion.Euler(saveData.rotation), passageTransform);
            }

            foreach (var saveData in levelSo.blocks)
            {
                GameObject block = Instantiate(blockPrefab, saveData.position, Quaternion.Euler(saveData.rotation), blockTransform);
            }

            for (int i = 0; i < levelSo.wagons.Count; i++)
            {
                GameObject rollerCoaster = new GameObject();
                rollerCoaster.name = "RollerCoaster";
                rollerCoaster.AddComponent<RollerCoaster>();
                GameManager.Instance.RollerCoasters.Add(rollerCoaster);

                for (int j = 0; j < levelSo.wagons[i].List.Count; j++)
                {
                    SaveData saveData = levelSo.wagons[i].List[j];
                    GameObject selectedPrefab = wagonMidPrefab;

                    if (j == 0) selectedPrefab = wagonHeadPrefab;
                    else if (j > 0 && j < levelSo.wagons[i].List.Count - 1) selectedPrefab = wagonMidPrefab;
                    else if (j == levelSo.wagons[i].List.Count - 1) selectedPrefab = wagonTailPrefab;

                    GameObject wagon = Instantiate(selectedPrefab, saveData.position, Quaternion.Euler(saveData.rotation), rollerCoaster.transform);
                    wagon.GetComponent<Wagon>().ChangeId(saveData.Id);
                }
            }

            for (int i = 0; i < levelSo.stickMans.Count; i++)
            {
                Transform passage = passageTransform.GetChild(i);

                for (int j = 0; j < levelSo.stickMans[i].List.Count; j++)
                {
                    SaveData saveData = levelSo.stickMans[i].List[j];
                    GameObject stickMan = Instantiate(stickManPrefab, saveData.position, Quaternion.Euler(saveData.rotation), passage.GetChild(0));
                    stickMan.GetComponent<StickMan>().ChangeId(saveData.Id);
                }
            }

            for (int i = 0; i < passageTransform.childCount; i++)
            {
                passageTransform.GetChild(i).GetComponent<Passage>().HandleStickManSettings();
            }

            minute = levelSo.minute;
            GameManager.Instance.SetTime(minute);
        });
    }
    
}
