using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public GridObstacleData obstacleData;
    public Transform enemy;
    private List<Vector3> path;
    private bool isMoving;

    public int startRow = 0;
    public int startColumn = 0;

    void Start()
    {
        InitializePlayerPosition(startRow, startColumn);
    }

    void Update()
    {
        if (TurnManager.instance.isPlayerTurn)
        {
            HandleInput();
            if (isMoving)
            {
                MoveAlongPath();
            }
        }
    }

    private void InitializePlayerPosition(int row, int column)
    {
        if (row < 0 || row >= GridPathfinder.gridSize || column < 0 || column >= GridPathfinder.gridSize)
        {
            Debug.LogError("Specified row or column is out of grid bounds!");
            return;
        }

        int index = row * GridPathfinder.gridSize + column;
        if (obstacleData.obstacleGrid[index] || IsObstructedByEnemy(row, column))
        {
            Debug.LogError("Cannot spawn player on an obstacle or enemy!");
            return;
        }

        Vector3 initialPosition = new Vector3(row, 0.5f, column);
        transform.position = initialPosition;
        Debug.Log($"Player spawned at row {row}, column {column}");
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPosition = hit.point;
                path = GridPathfinder.FindPath(transform.position, targetPosition, obstacleData, enemy.position);

                if (path != null && path.Count > 0)
                {
                    isMoving = true;
                }
                else
                {
                    Debug.Log("No valid path found! Player cannot move.");
                }
            }
        }
    }


    private void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            Vector3 targetPosition = path[0];
            Vector3 direction = targetPosition - transform.position;
            float distance = moveSpeed * Time.deltaTime;

            if (direction.magnitude <= distance)
            {
                transform.position = targetPosition;
                path.RemoveAt(0);
            }
            else
            {
                transform.Translate(direction.normalized * distance, Space.World);
            }

            if (path.Count == 0)
            {
                isMoving = false;
                TurnManager.instance.ToggleTurn();  // Switch to enemy's turn
            }
        }
        else
        {
            isMoving = false;
        }
    }


    private bool IsObstructedByEnemy(int row, int column)
    {
        Vector3 enemyGridPosition = WorldToGridPosition(enemy.position);
        return enemyGridPosition.x == row && enemyGridPosition.z == column;
    }

    private Vector3Int WorldToGridPosition(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), 0, Mathf.RoundToInt(position.z));
    }

    private bool IsObstacle(Vector3Int gridPosition, GridObstacleData obstacleData)
    {
        int index = gridPosition.x * GridPathfinder.gridSize + gridPosition.z;
        return obstacleData.obstacleGrid[index];
    }
}
