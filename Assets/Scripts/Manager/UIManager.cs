using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text time_text;
    [SerializeField] private TMP_Text level_text;
    [SerializeField] private Image blackScreen;
    [SerializeField] private GameObject levelComplete;
    [SerializeField] private GameObject gameOver;
    
    private void Awake()
    {
        Instance = this;
    }

    public void HandleTimeText(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        time_text.text = $"{minutes:00}:{seconds:00}";
    }
    
    private void HandleLevelText(int level)
    {
        level_text.text = "Level" + level;
    }

    public void HandleOpenBlackScreen(bool isOpen)
    {
        float weight = isOpen ? 0 : 1;
        DOTween.To(() => weight, x => weight = x, isOpen ? 1 : 0, 1).OnUpdate(() =>
        {
            blackScreen.color = new Color(0, 0, 0, weight);
        });
    }

    public void HandleLevelCompletePanelOpen()
    {
        levelComplete.SetActive(true);
    }
    
    public void HandleGameOverPanelOpen()
    {
        gameOver.SetActive(true);
    }
    
    public void NextLevel()
    {
        LevelManager.Instance.currentLevel++;
        LevelManager.Instance.LoadLevel(LevelManager.Instance.LevelData.Levels[LevelManager.Instance.currentLevel]);
        HandleOpenBlackScreen(false);
        HandleLevelText(LevelManager.Instance.currentLevel+1);
    }
    
    public void ResetLevel()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.LevelData.Levels[LevelManager.Instance.currentLevel]);
        HandleOpenBlackScreen(false);
        HandleLevelText(LevelManager.Instance.currentLevel+1);
    }
    
}