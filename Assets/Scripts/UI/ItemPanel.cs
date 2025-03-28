using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
   [Header("Item")] 
   [SerializeField] private List<GameObject> Categories;
   [SerializeField] private List<TMP_Text> buttonTexts;

   [Header("Item Placed")] 
   [SerializeField] private List<MenuItem> placedMenuItems;
   [SerializeField] private int PlacedId = -1;
   [SerializeField] private LayerMask _layer;
   private RaycastHit _raycastHit;

   public void OpenItemPanel(int Index)
   {
      for (int i = 0; i < Categories.Count; i++)
      {
         Categories[i].gameObject.SetActive(false);
         buttonTexts[i].color = Color.white;
      }

      if (PlacedId != -1)
      {
         placedMenuItems[PlacedId].OnDeSelect();
         PlacedId = -1;
      }

      Categories[Index].gameObject.SetActive(true);
      buttonTexts[Index].color = Color.yellow;
   }

   public void SelectMenuItem(int Id)
   {
      foreach (var button in placedMenuItems)
      {
         button.OnDeSelect();
      }
      placedMenuItems[Id].OnSelect();
      PlacedId = Id;
   }

   private void Update()
   {
      if(PlacedId == -1) return;

      if (Input.GetKeyDown(KeyCode.Escape))
      {
         placedMenuItems[PlacedId].OnDeSelect();
         PlacedId = -1;
         return;
      }
      
      Vector3 mousePos = Input.mousePosition;
      if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height) return;
      if (Input.mousePosition == Vector3.zero) return;
      if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _raycastHit, Mathf.Infinity, _layer, QueryTriggerInteraction.Ignore)) return;
      
      placedMenuItems[PlacedId].ToPlacing(_raycastHit);
      if (Input.GetMouseButtonDown(0)) placedMenuItems[PlacedId].ToPlaceItem(_raycastHit);
      if(Input.GetKeyDown(KeyCode.R)) placedMenuItems[PlacedId].RotateObj(); 
   }
   
}
