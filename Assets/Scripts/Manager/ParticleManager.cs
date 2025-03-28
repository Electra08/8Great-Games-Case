using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
   public static ParticleManager Instance;
   
   [SerializeField] private GameObject prefab;
   [SerializeField] private int poolSize = 10;
   private Queue<GameObject> pool = new Queue<GameObject>();

   private void Awake()
   {
      Instance = this;
   }
   
   private void Start()
   {
      for (int i = 0; i < poolSize; i++)
      {
         GameObject obj = Instantiate(prefab);
         obj.SetActive(false);
         pool.Enqueue(obj);
      }
   }

   private GameObject GetFromPool()
   {
      if (pool.Count > 0)
      {
         GameObject obj = pool.Dequeue();
         obj.SetActive(true);
         return obj;
      }
      else
      {
         GameObject newObj = Instantiate(prefab);
         return newObj;
      }
   }

   private void ReturnToPool(GameObject obj)
   {
      obj.SetActive(false);
      pool.Enqueue(obj);
   }

   public void GetParticle(Transform pos)
   {
      GameObject targetParticle = GetFromPool();
      targetParticle.transform.position = pos.position;
      targetParticle.GetComponent<ParticleSystem>().Play();
      DOVirtual.DelayedCall(1f, () =>
      {
         ReturnToPool(targetParticle);
      });
   }
   
}
