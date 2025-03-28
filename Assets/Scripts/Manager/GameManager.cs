using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Time")]
    [SerializeField] private int minute;
    [Space]
    [SerializeField] private List<Vector3> blockPositions;
    public List<GameObject> RollerCoasters;
    public LayerMask _RollerCoasterLayer;
    [Header("Grid Boundaries")]
    public Vector2 gridSize = new Vector2(10, 10);
    public Vector3 gridOrigin = Vector3.zero;

    private float time;
    private bool isStart;
    private bool Control;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        RollerCoasters = new List<GameObject>();
        LevelManager.Instance.currentLevel++;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.LevelData.Levels[LevelManager.Instance.currentLevel]);
        UIManager.Instance.HandleOpenBlackScreen(false);
    }

    private void Update()
    {
        if(!isStart) return;
        
        if(minute < 1)
        {
            isStart = false;
            UIManager.Instance.HandleOpenBlackScreen(true);
            UIManager.Instance.HandleGameOverPanelOpen();
            return;
        }

        if (time >= 1)
        {
            minute--;
            UIManager.Instance.HandleTimeText(minute);
            time = 0;
        }
        else time += Time.deltaTime;
    }

    public void AddBlockPos(Transform pos)
    {
        blockPositions.Add(pos.position);
    }
    
    public bool IsPositionBlocked(Vector3 position)
    {
        return blockPositions.Contains(position);
    }

    public void SetTime(int _minute)
    {
        minute = _minute;
        UIManager.Instance.HandleTimeText(minute);
        isStart = true;
        Control = false;
    }

    public void CheckRollerCoaster()
    {
        DOVirtual.DelayedCall(1, () =>
        {
            for (int i = 0; i < RollerCoasters.Count; i++)
            {
                if (RollerCoasters[i] == null) RollerCoasters.RemoveAt(i);
            }
            
            if(RollerCoasters.Count > 0) return;
            if(Control) return;
            
            Control = true;
            UIManager.Instance.HandleOpenBlackScreen(true);
            UIManager.Instance.HandleLevelCompletePanelOpen();
        });
    }
    
}
