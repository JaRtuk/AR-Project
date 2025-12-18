using UnityEngine;

public class SheepController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float smoothTurnTime = 0.5f;
    
    private Vector3 currentDirection;
    private Vector3 targetDirection;
    private float turnProgress;
    private bool isTurning;
    private Bounds movementBounds;
    private float timer;
    private float directionChangeTime;

    public void Initialize(Bounds bounds)
    {
        movementBounds = bounds;
        
        GetNewRandomDirection();
        directionChangeTime = Random.Range(2f, 5f);
    }

    void Start()
    {
        if (movementBounds.size == Vector3.zero)
        {
            movementBounds = new Bounds(transform.position, new Vector3(2f, 0.1f, 2f));
            GetNewRandomDirection();
            directionChangeTime = Random.Range(2f, 5f);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= directionChangeTime)
        {
            GetNewRandomDirection();
            timer = 0;
            directionChangeTime = Random.Range(2f, 5f);
        }
        
        if (isTurning)
        {
            turnProgress += Time.deltaTime / smoothTurnTime;
            currentDirection = Vector3.Slerp(currentDirection, targetDirection, turnProgress);
            
            if (turnProgress >= 1f)
            {
                isTurning = false;
                turnProgress = 0f;
            }
        }
        
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
        
        CheckBounds();
    }

    void CheckBounds()
    {
        Vector3 newPos = transform.position;
        bool changed = false;

        if (newPos.x < movementBounds.min.x)
        {
            newPos.x = movementBounds.min.x;
            changed = true;
        }
        else if (newPos.x > movementBounds.max.x)
        {
            newPos.x = movementBounds.max.x;
            changed = true;
        }
        
        if (newPos.z < movementBounds.min.z)
        {
            newPos.z = movementBounds.min.z;
            changed = true;
        }
        else if (newPos.z > movementBounds.max.z)
        {
            newPos.z = movementBounds.max.z;
            changed = true;
        }
        
        if (changed)
        {
            transform.position = newPos;
            
            Vector3 toCenter = (movementBounds.center - newPos).normalized;
            SetNewDirection(toCenter);
        }
    }

    public void SetNewDirection(Vector3 newDirection)
    {
        if (newDirection == Vector3.zero) return;
        
        targetDirection = newDirection.normalized;
        isTurning = true;
        turnProgress = 0f;
    }

    void GetNewRandomDirection()
    {
        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;
        
        SetNewDirection(randomDir);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            Vector3 awayDirection = (transform.position - other.transform.position).normalized;
            SetNewDirection(awayDirection);
        }
    }
    
    void OnDrawGizmos()
    {
        if (movementBounds.size != Vector3.zero)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f); 
            Gizmos.DrawWireCube(movementBounds.center, movementBounds.size);
        }
    }
}