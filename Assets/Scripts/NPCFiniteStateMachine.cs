using System.IO;
using UnityEngine;


public class NPCFiniteStateMachine : MonoBehaviour
{

    [SerializeField]
    private BoxCollider area;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float speed;

    public NPCStates CurrentState;

    private Rigidbody rb;
    private Vector3 PatrolTargetPos;
    private bool isExpanded;


    private void Start()
    {
        isExpanded = false;

        rb = GetComponent<Rigidbody>();
        CurrentState = NPCStates.Patrol;

        PatrolTargetPos = PickRandomPoint(area);
    }

    private void Update()
    {
        rb.linearVelocity = Seek(PatrolTargetPos);
    }

    private void FixedUpdate()
    {
        ChooseCurrentState();
    }

    private void ChooseCurrentState()
    {

        CheckDistanceWithPlayer();
        switch (CurrentState)
        {
            case NPCStates.Patrol:
                this.transform.localScale = Vector3.one;
                isExpanded = false;
                Patrol();
                break;
            case NPCStates.Seek:
                this.transform.localScale = Vector3.one;
                isExpanded = false;
                rb.linearVelocity = Seek(player.position);
                break;
            case NPCStates.Alert:
                this.transform.localScale = Vector3.one;
                isExpanded = false;
                Alert(player.transform);
                break;
            case NPCStates.Attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        if (Vector3.SqrMagnitude(PatrolTargetPos - this.transform.position) < 0.2f)
        {
            PatrolTargetPos = PickRandomPoint(area);
        }
        else
        {
            rb.linearVelocity = Seek(PatrolTargetPos);
        }

    }

    private Vector3 Seek(Vector3 Target)
    {
        var direction = (Target - this.transform.position).normalized;
        return (direction * speed);


      
    }

    private Vector3 PickRandomPoint(BoxCollider patrolArea)
    {
        Bounds bounds = patrolArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        var targetPosition = new Vector3(randomX, this.transform.position.y, randomZ);

        return targetPosition;
    }

    private void Attack()
    {
        if (!isExpanded)
        {
// mmkn nstkhdm 7aga esmha lossyScale 3lshan msln lw kont bt3aml m3 child object le parent badal ma 22asr 3ala el parent aw ba2y el children
            this.transform.localScale *= 10; 
        }
        isExpanded = true;
    }

    private void Alert(Transform player)
    {
        rb.linearVelocity = Vector3.zero;
        this.transform.LookAt(player);
    }

    private void CheckDistanceWithPlayer()
    {
        var distance = Vector3.Distance(this.transform.position, player.position);

        if (distance < 10)
        {
            CurrentState = NPCStates.Alert;
            if (distance < 7)
            {
                CurrentState = NPCStates.Seek;
                if (distance < 2)
                {
                    CurrentState = NPCStates.Attack;
                }
            }
        }

        if (distance > 10)
        {
           CurrentState= NPCStates.Patrol;
        }

        

    }
  

}
public enum NPCStates
{
    Patrol,
    Alert,
    Seek,
    Attack
}
