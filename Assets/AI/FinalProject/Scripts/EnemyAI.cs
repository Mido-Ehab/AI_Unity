//using UnityEngine;

//public class EnemyAI : MonoBehaviour
//{
//    public Transform target; // Assign opponent in the Inspector
//    public GameObject projectilePrefab; // Assign a Capsule prefab
//    public Transform firePoint; // Where the projectile will be instantiated
//    public GameObject shieldPrefab; // Assign a Shield prefab
//    public Transform shieldSpawnPoint; // Position for the shield

//    public float projectileSpeed = 10f;
//    public float shieldDuration = 3f;

//    void Update()
//    {
//        LookAtTarget();
//    }

//    void LookAtTarget()
//    {
//        if (target != null)
//        {
//            Vector3 direction = target.position - transform.position;
//            direction.y = 0; // Keep rotation only in the Y-axis
//            transform.rotation = Quaternion.LookRotation(direction);
//        }
//    }

//    public void Shoot()
//    {
//        if (projectilePrefab != null && firePoint != null)
//        {
//            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
//            Rigidbody rb = projectile.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                rb.linearVelocity = (target.position - firePoint.position).normalized * projectileSpeed;
//            }
//        }
//    }

//    public void CreateShield()
//    {
//        if (shieldPrefab != null && shieldSpawnPoint != null)
//        {
//            GameObject shield = Instantiate(shieldPrefab, shieldSpawnPoint.position, Quaternion.identity, transform);
//            Destroy(shield, shieldDuration); // Destroy the shield after a few seconds
//        }
//    }
//}

using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [TextArea] public string personalityDescription; // Describe target's behavior
    public string[] responses; // Unique responses per target

    public string GetRandomResponse()
    {
        if (responses.Length > 0)
        {
            return responses[Random.Range(0, responses.Length)];
        }
        return "No response available.";
    }
}
