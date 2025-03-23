using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float Speed;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float stoppingDistance;
    [SerializeField]
    private Transform[] Waypoints;
    [SerializeField]
    float PredectionTime;

    [SerializeField]
    bool evade=false;

    private bool isBeingFollowed = false;
    private int waypointsCounter = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //-------------------------------------------------------------------------------Task01
        //if (Vector3.Distance(rb.position, target.position) < stoppingDistance)
        //{
        //   rb.linearVelocity = Flee(target);
        //    //rb.linearVelocity = Seek(target);
        //}
        //else if (!isBeingFollowed)
        //{
        //    Patrol();
        //}
        //float distanceToTarget = Vector3.Distance(rb.position, target.position);

        //if (distanceToTarget < stoppingDistance)
        //{
        //    isBeingFollowed = true;
        //}
        //----------------------------------------------------------------------------------Task02
        //if (isBeingFollowed)
        //{
        //    if (evade)
        //    {
        //        rb.linearVelocity = Evade(target.GetComponent<Rigidbody>());
        //    }
        //    else
        //    {
        //          rb.linearVelocity = Pursue(target.GetComponent<Rigidbody>());
        //    }


        //}
        //else
        //{
        //    Patrol();
        //}
        //----------------------------------------------------------------------------------------- Task01 + Task02
        float distanceToTarget = Vector3.SqrMagnitude(rb.position- target.position);

        if (distanceToTarget < stoppingDistance)
        {
            if (evade)
            {
                rb.linearVelocity = Evade(target.GetComponent<Rigidbody>());
            }
            else
            {
                rb.linearVelocity = Pursue(target.GetComponent<Rigidbody>());
            }
        }
        else
        {
           
            Patrol();
        }
    }

    private void Patrol()
    {
        if (Vector3.SqrMagnitude(rb.position- Waypoints[waypointsCounter].position) < stoppingDistance)
        {
            waypointsCounter++;
            if (waypointsCounter >= Waypoints.Length)
            {
                waypointsCounter = 0;
            }
        }

        if (Vector3.SqrMagnitude(rb.position - target.position) < stoppingDistance)
        {
            isBeingFollowed = true;
            rb.linearVelocity = Seek(target);
        }
        else
        {
            if (!isBeingFollowed)
            {
                rb.linearVelocity = Seek(Waypoints[waypointsCounter]);
            }
        }
    }

    private Vector3 Seek(Transform target)
    {
        var direction = (target.position - this.transform.position).normalized;
        return direction * Speed;
    }

    private Vector3 Flee(Transform target)
    {
        var direction = (target.position - this.transform.position).normalized;
        return (-1 * direction * Speed);
    }

    private Vector3 Pursue(Rigidbody targetRB)
    {
        var newTarget = target.position + (targetRB.linearVelocity * PredectionTime);

        var direction =(newTarget - this.transform.position).normalized;
        return direction * Speed;
    }

    private Vector3 Evade(Rigidbody targetRB)
    {
        var newTarget = target.position + (targetRB.linearVelocity * PredectionTime);

        var direction = (this.transform.position-newTarget).normalized;
        return direction * Speed;
    }
}
