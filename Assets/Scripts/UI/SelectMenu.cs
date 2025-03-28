using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] private GameObject selectMenu;
    
    public void OpenCloseSelectMenu()
    {
        selectMenu.SetActive(!selectMenu.activeSelf);    
    }
}
