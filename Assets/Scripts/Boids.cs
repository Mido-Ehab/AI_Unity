using UnityEngine;

public class Boids : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject[] boids;

    void Start()
    {
        rb = GetComponent<Rigidbody>();


        rb.linearVelocity = Vector3.zero;

        boids = BoidsManager.instance.Boids;
    }

    void FixedUpdate()
    {
        Vector3 movement = Allignment() + Cohesion() + (Separation()*BoidsManager.instance.SeparationDistance);
        movement = SpeedLimit(Allignment()+ Cohesion())+Separation()*BoidsManager.instance.SeparationDistance;

        if (!float.IsNaN(movement.x) && !float.IsNaN(movement.y) && !float.IsNaN(movement.z))
        {
            rb.linearVelocity = movement;
        }
      
    }

    Vector3 SpeedLimit(Vector3 speed)
    {
        if (speed.magnitude > 0 && speed.magnitude > BoidsManager.instance.speed)
        {
            return speed.normalized * BoidsManager.instance.speed;
        }
        return speed;
    }

    private Vector3 Allignment()
    {
        Vector3 allVec = Vector3.zero;
      

        foreach (var boid in boids)
        {
            if (boid != this.gameObject)
            {
                allVec += boid.transform.forward;
                
            }
        }

        
            allVec /= (boids.Length-1);
            return allVec;
        

     
    }

    private Vector3 Cohesion()
    {
        Vector3 cohVec = Vector3.zero;
     

        foreach (var boid in boids)
        {
            if (boid != this.gameObject)
            {
                cohVec += boid.transform.position;
              
            }
        }

      
            cohVec /= (boids.Length-1);
            return cohVec ;
        

       
    }

    private Vector3 Separation()
    {
        Vector3 sepVec = Vector3.zero;
        int boidsCount = 0;

        foreach (var boid in boids)
        {
            if (boid != this.gameObject)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);
                if (distance < BoidsManager.instance.SeparationDistance && distance > 0)
                {
                    sepVec += (transform.position - boid.transform.position).normalized;
                    boidsCount++;
                }
            }
        }

        if (boidsCount > 0)
        {
            sepVec /= boidsCount;
        }

        return sepVec;
    }
}
