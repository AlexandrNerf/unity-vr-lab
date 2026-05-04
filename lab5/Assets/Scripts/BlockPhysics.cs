using System;
using UnityEngine;

public class BlockPhysics : MonoBehaviour
{
    Rigidbody rb;
    bool placed = false;
    public event Action OnPlaced; // событие, когда блок поставлен

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (placed) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.6f))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                PlaceBlock(hit);
            }
        }
    }

    void PlaceBlock(RaycastHit hit)
    {
        placed = true;
        float height = GetComponent<Collider>().bounds.extents.y;
        transform.position = hit.point + Vector3.up * height;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        OnPlaced?.Invoke(); // уведомляем Builder
    }
}