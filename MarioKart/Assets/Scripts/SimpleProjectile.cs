using System;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
   {
      var kart = collision.collider.GetComponent<KartStatusEffects>();
      if (kart != null)
      {
         kart.KnockBack(transform.position);
      }
      
      Destroy(gameObject);
   }
}
