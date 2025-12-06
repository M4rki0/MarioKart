using UnityEngine;

public class KartItemHandler : MonoBehaviour
{
    public enum ItemType {None, Boost, Banana, Projectile }

    public ItemType currentItem = ItemType.None;

    public GameObject bananaPrefab;

    public GameObject projectilePrefab;

    public float boostForce = 1000f;

    private Rigidbody rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentItem != ItemType.None && Input.GetKeyDown(KeyCode.Space))
        {
            UseItem();
        }
    }

    void UseItem()
    {
        switch (currentItem)
        {
            case ItemType.Boost:
                rb.AddForce(transform.forward * boostForce, ForceMode.Impulse);
                break;
            case ItemType.Banana:
                Instantiate(bananaPrefab, transform.position - transform.forward * 2f, Quaternion.identity);
                break;
            case ItemType.Projectile:
                GameObject proj = Instantiate(projectilePrefab, transform.position + transform.forward * 2f, transform.rotation);
                
                proj.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
                break;
        }

        currentItem = ItemType.None;
    }

    public void GiveRandomItem()
    {
        int roll = Random.Range(1, 4);
        currentItem = (ItemType)roll;
        Debug.Log("Got item: " + currentItem);
    }
}
