using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    private void Start()
    {
        if(GameManager.Instance != null)  GameManager.Instance.AddBlockPos(gameObject.transform);
    }
}