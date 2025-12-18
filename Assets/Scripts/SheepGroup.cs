using UnityEngine;
using System.Collections.Generic;

public class SheepGroup : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] sheepPrefabs;
    
    [Header("Moving Aria")]
    [SerializeField] private Vector2 movementArea = new Vector2(2f, 2f);
    [SerializeField] private float areaHeight = 0.01f;
    
    private List<Transform> sheepList = new List<Transform>();
    private Bounds movementBounds;

    void Start()
    {
        movementBounds = new Bounds(
            transform.position, 
            new Vector3(movementArea.x, areaHeight, movementArea.y)
        );
        
        CreateSheep();
    }

    void CreateSheep()
    {
        if (sheepPrefabs == null || sheepPrefabs.Length == 0)
        {
            Debug.LogError("Don`t added sheep prefabs!");
            return;
        }

        int sheepCount = sheepPrefabs.Length;
        int rows = Mathf.CeilToInt(Mathf.Sqrt(sheepCount));
        int cols = Mathf.CeilToInt((float)sheepCount / rows);
        
        float cellWidth = movementArea.x / (cols + 1);
        float cellDepth = movementArea.y / (rows + 1);
        
        for (int i = 0; i < sheepCount; i++)
        {
            int row = i / cols;
            int col = i % cols;
            
            Vector3 position = new Vector3(
                transform.position.x - movementArea.x/2 + (col + 1) * cellWidth,
                transform.position.y,
                transform.position.z - movementArea.y/2 + (row + 1) * cellDepth
            );
            
            GameObject sheep = Instantiate(sheepPrefabs[i], position, Quaternion.identity, transform);
            sheepList.Add(sheep.transform);
            
            var controller = sheep.GetComponent<SheepController>();
            if (controller != null)
            {
                controller.Initialize(movementBounds);
            }
        }
    }

    void Update()
    {
        foreach (var sheep in sheepList)
        {
            Vector3 sheepPos = sheep.position;
            
            if (Mathf.Abs(sheepPos.x - transform.position.x) > movementArea.x / 2)
            {
                Vector3 toCenter = (transform.position - sheepPos).normalized;
                var controller = sheep.GetComponent<SheepController>();
                if (controller != null)
                {
                    controller.SetNewDirection(toCenter);
                }
            }
            
            if (Mathf.Abs(sheepPos.z - transform.position.z) > movementArea.y / 2)
            {
                Vector3 toCenter = (transform.position - sheepPos).normalized;
                var controller = sheep.GetComponent<SheepController>();
                if (controller != null)
                {
                    controller.SetNewDirection(toCenter);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position, 
            new Vector3(movementArea.x, 0.1f, movementArea.y)
        );
    }
}