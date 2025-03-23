using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public Transform firePoint; // Where the projectile is instantiated
    public GameObject projectilePrefab; // Assign a Capsule prefab
    public GameObject shieldPrefab; // Assign a Shield prefab
    public Transform shieldSpawnPoint; // Position for the shield
    private Transform target; // Dynamically set target

    public float projectileSpeed = 10f;
    public float shieldDuration = 3f;

    void Update()
    {
        LookAtTarget();
    }

    void LookAtTarget()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep rotation only in the Y-axis
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void Shoot()
    {
        if (projectilePrefab != null && firePoint != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = (target.position - firePoint.position).normalized * projectileSpeed;
            }
            Debug.Log("Player shot at " + target.name);
        }
    }

    public void CreateShield()
    {
        if (shieldPrefab != null && shieldSpawnPoint != null)
        {
            GameObject shield = Instantiate(shieldPrefab, shieldSpawnPoint.position, Quaternion.identity, transform);
            Destroy(shield, shieldDuration); // Destroy shield after a few seconds
            Debug.Log("Shield Activated");
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget.transform;
            Debug.Log($"Player target set to: {target.name}");
        }
    }
}
