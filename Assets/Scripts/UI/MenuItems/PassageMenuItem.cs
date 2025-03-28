using DG.Tweening;
using UnityEngine;

public class PassageMenuItem : MenuItem
{
    [SerializeField] private GameObject objPrefab;
    private GameObject obj;
    
    public override void OnSelect()
    {
        base.OnSelect();
        obj = Instantiate(objPrefab, new Vector3(99,99,99), objPrefab.transform.rotation);
    }

    public override void OnDeSelect()
    {
        base.OnDeSelect();
        Destroy(obj);
    }

    public override void ToPlaceItem(RaycastHit _hit)
    {
        base.ToPlaceItem(_hit);
        
        if(_hit.collider.tag != "Grid") return;
        
        GameObject blockObj = Instantiate(ItemPrefab, obj.transform.position, obj.transform.rotation);
        blockObj.transform.SetParent(ItemParent);
    }

    public override void ToPlacing(RaycastHit _hit)
    {
        base.ToPlacing(_hit);
        
        if(_hit.collider.tag != "Grid")
        {
            obj.transform.position = new Vector3(99, 99, 99);
            return;
        }

        obj.transform.position = _hit.transform.position + obj.transform.forward * -0.8f;
    }

    public override void RotateObj()
    {
        base.RotateObj();

        var rot = obj.transform.eulerAngles;
        rot.y += 90;
        obj.transform.DORotate(rot, 0.1f);
    }
}