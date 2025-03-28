using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuItem : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform ItemParent;
    public Image _image;

    public virtual void OnSelect()
    {
        _image.color = Color.yellow;
    }

    public virtual void OnDeSelect()
    {
        _image.color = Color.white;
    }

    public virtual void ToPlaceItem(RaycastHit _hit)
    {

    }

    public virtual void ToPlacing(RaycastHit _hit)
    {
        
    }

    public virtual void RotateObj()
    {
        
    }
    
}