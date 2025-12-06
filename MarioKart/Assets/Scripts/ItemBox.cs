using System;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        KartItemHandler itemHandler = other.GetComponent<KartItemHandler>();
        if (itemHandler != null)
        {
            itemHandler.GiveRandomItem();
            Destroy(gameObject);
        }
    }
}
