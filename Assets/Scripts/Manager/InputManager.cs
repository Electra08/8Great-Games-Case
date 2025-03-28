using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private RollerCoaster selectedRollerCoaster;
    [SerializeField] private bool isFront = false;
    [SerializeField] private LayerMask _layer;
    
    private RaycastHit _raycastHit;

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#endif
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height) return;
        if (Input.mousePosition == Vector3.zero) return;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _raycastHit, Mathf.Infinity, _layer, QueryTriggerInteraction.Ignore)) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_raycastHit.collider.tag == "WagonHead" && selectedRollerCoaster == null)
            {
                selectedRollerCoaster = _raycastHit.collider.gameObject.GetComponentInParent<RollerCoaster>();
                selectedRollerCoaster.OnSelect();
                isFront = true;
            }
            else if (_raycastHit.collider.tag == "WagonTail" && selectedRollerCoaster == null)
            {
                selectedRollerCoaster = _raycastHit.collider.gameObject.GetComponentInParent<RollerCoaster>();
                selectedRollerCoaster.OnSelect();
                isFront = false;
            }
            else if (_raycastHit.collider.tag == "Wagon" && selectedRollerCoaster != null)
            {
                selectedRollerCoaster.OnDeSelect();
                selectedRollerCoaster = null;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (_raycastHit.collider.tag == "Wagon" && selectedRollerCoaster != null)
            {
                selectedRollerCoaster.OnDeSelect();
                selectedRollerCoaster = null;
            }
            if (_raycastHit.collider.tag != "Grid" || selectedRollerCoaster == null) return;
            
            if (isFront) selectedRollerCoaster.MoveFromFront(_raycastHit.collider.transform);
            else selectedRollerCoaster.MoveFromBack(_raycastHit.collider.transform);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedRollerCoaster == null) return;
            selectedRollerCoaster.OnDeSelect();
            selectedRollerCoaster = null;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out _raycastHit, Mathf.Infinity, _layer, QueryTriggerInteraction.Ignore)) return;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (_raycastHit.collider.tag == "WagonHead" && selectedRollerCoaster == null)
                {
                    selectedRollerCoaster = _raycastHit.collider.gameObject.GetComponentInParent<RollerCoaster>();
                    selectedRollerCoaster.OnSelect();
                    isFront = true;
                }
                else if (_raycastHit.collider.tag == "WagonTail" && selectedRollerCoaster == null)
                {
                    selectedRollerCoaster = _raycastHit.collider.gameObject.GetComponentInParent<RollerCoaster>();
                    selectedRollerCoaster.OnSelect();
                    isFront = false;
                }
                else if (_raycastHit.collider.tag == "Wagon" && selectedRollerCoaster != null)
                {
                    selectedRollerCoaster.OnDeSelect();
                    selectedRollerCoaster = null;
                }
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (_raycastHit.collider.tag == "Wagon" && selectedRollerCoaster != null)
                {
                    selectedRollerCoaster.OnDeSelect();
                    selectedRollerCoaster = null;
                }
                if (_raycastHit.collider.tag != "Grid" || selectedRollerCoaster == null) return;
                
                if (isFront) selectedRollerCoaster.MoveFromFront(_raycastHit.collider.transform);
                else selectedRollerCoaster.MoveFromBack(_raycastHit.collider.transform);
                break;

            case TouchPhase.Ended:
                if (selectedRollerCoaster == null) return;
                selectedRollerCoaster.OnDeSelect();
                selectedRollerCoaster = null;
                break;
        }
    }
}