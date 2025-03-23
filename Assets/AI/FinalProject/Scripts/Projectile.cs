using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f; // Destroy projectile after this time

    void Start()
    {
        Destroy(gameObject, lifetime); // Auto-destroy if it doesn't hit anything
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) // Check if we hit an enemy
        {
            Destroy(collision.gameObject); // Destroy the enemy
            Destroy(gameObject); // Destroy the bullet

            
            Debug.Log("Bullet hit enemy! Enemy destroyed.");
        }
    }
}
