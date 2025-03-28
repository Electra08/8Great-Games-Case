using UnityEngine;

public class ColorMenuItem : MenuItem
{
    [SerializeField] private int Id;

    public override void ToPlaceItem(RaycastHit _hit)
    {
        base.ToPlaceItem(_hit);
        
        var tempMonoArray = _hit.collider.GetComponents<MonoBehaviour>();

        foreach (var monoBehaviour in tempMonoArray)
        {
            var tempCollidable = monoBehaviour as IChangeID;

            if (tempCollidable == null) continue;
            _hit.collider.GetComponent<IChangeID>().ChangeId(Id);
            break;
        }   
    }
}