using DG.Tweening;
using UnityEngine;

public class StickManMenuItem : MenuItem
{
    public override void OnSelect()
    {
        base.OnSelect();
        
        var passages = GameObject.FindGameObjectsWithTag("Passage");
        foreach (var passage in passages)
        {
            passage.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public override void OnDeSelect()
    {
        base.OnDeSelect();
        
        var passages = GameObject.FindGameObjectsWithTag("Passage");
        foreach (var passage in passages)
        {
            passage.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }

    public override void ToPlaceItem(RaycastHit _hit)
    {
        base.ToPlaceItem(_hit);
        
        if(_hit.collider.tag != "Passage") return;
        
        GameObject stickMan = Instantiate(ItemPrefab, _hit.transform.GetChild(0));
        stickMan.transform.position = _hit.transform.GetChild(1).GetChild(_hit.transform.GetChild(0).childCount).position;
        stickMan.transform.rotation = _hit.transform.GetChild(1).GetChild(_hit.transform.GetChild(0).childCount).rotation;
    }
}