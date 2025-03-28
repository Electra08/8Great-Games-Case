using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Passage : MonoBehaviour,IChangeID
{
    private Queue<StickMan> stickMans;
    private List<Transform> queueTransforms;
    [SerializeField] private GameObject queueObj;
    [SerializeField] private GameObject stickMansObj;
    [SerializeField] private RollerCoaster targetRollerCoaster;
    [SerializeField] private float delayTime = 0.1f;

    private float time;
    private bool isStart = false;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        queueTransforms = new List<Transform>();
        for (int i = 0; i < queueObj.transform.childCount; i++)
        {
            queueTransforms.Add(queueObj.transform.GetChild(i).transform);
        }
        stickMans = new Queue<StickMan>();
        for (int i = 0; i < stickMansObj.transform.childCount; i++)
        {
            stickMans.Enqueue(stickMansObj.transform.GetChild(i).GetComponent<StickMan>());
        }

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void HandleStickManSettings()
    {
        stickMans = new Queue<StickMan>();
        for (int i = 0; i < stickMansObj.transform.childCount; i++)
        {
            stickMans.Enqueue(stickMansObj.transform.GetChild(i).GetComponent<StickMan>());
        }
    }
    

    private void Update()
    {
        if(!isStart) return;
        if(targetRollerCoaster == null) return;
        if(stickMans.Count == 0) return;
        if(!targetRollerCoaster.IsHasIDWagon(stickMans.Peek())) return;
        
        if (time >= delayTime)
        {
            var stickMan = stickMans.Dequeue();
            targetRollerCoaster.FindToSitWagon(stickMan);

            var Index = 0;
            foreach (var targetMan in stickMans)
            {
                targetMan.transform.DOMove(queueTransforms[Index].position, 0.1f).SetEase(Ease.Linear);
                targetMan.transform.DORotateQuaternion(queueTransforms[Index].rotation, 0.1f).SetEase(Ease.Linear);
                Index++;
            }
            
            time = 0;
        }
        else time += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("WagonHead") && !other.gameObject.CompareTag("WagonTail") && !other.gameObject.CompareTag("Wagon")) return;
        isStart = true;
        targetRollerCoaster = other.transform.parent.GetComponent<RollerCoaster>();
    }

    private void OnTriggerExit(Collider other)
    {
        isStart = false;
        targetRollerCoaster = null;
        time = 0;
    }

    public void ChangeId(int _id)
    {
        var mats = _meshRenderer.materials;
        mats[1] = LevelManager.Instance.materials[_id];
        _meshRenderer.materials = mats;
    }
}