using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject BoidPrefab;

    [Range(0f, 10f)]
    public float speed = 5f;

    [SerializeField]
    public float SeparationDistance = 2f;

    [SerializeField]
    private int count = 10;

    [SerializeField]
    private Vector3 range = new Vector3(10, 10, 10);

    public GameObject[] Boids;

    public static BoidsManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Boids = new GameObject[count];
        InstantiateBoids();
    }

    private void InstantiateBoids()
    {
        for (int i = 0; i < Boids.Length; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(-range.x, range.x),
                Random.Range(-range.y, range.y),
                Random.Range(-range.z, range.z)
            );

            Boids[i] = Instantiate(BoidPrefab, spawnPos, Quaternion.identity, this.transform);
        }
    }
}
