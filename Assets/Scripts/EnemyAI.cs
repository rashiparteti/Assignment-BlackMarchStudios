using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, AI
{
    public float moveSpeed = 2.0f;
    public GridObstacleData obstacleData;
    private List<Vector3> path;
    private bool isMoving;

    private PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        obstacleData = FindObjectOfType<ObstacleManager>().obstacleData;
    }

    void Update()
    {
        if (!isMoving && !TurnManager.instance.isPlayerTurn)
        {
            MoveTowards(playerController.transform.position);
        }
        else if (isMoving)
        {
            MoveAlongPath();
        }
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        Vector3Int enemyGridPos = WorldToGridPosition(transform.position);
        Vector3Int playerGridPos = WorldToGridPosition(targetPosition);

        Vector3Int[] adjacentPositions = new Vector3Int[]
        {
            playerGridPos + Vector3Int.left,
            playerGridPos + Vector3Int.right,
            playerGridPos + Vector3Int.forward,
            playerGridPos + Vector3Int.back
        };

        Vector3Int bestTarget = playerGridPos;
        float bestDistance = float.MaxValue;

        foreach (var pos in adjacentPositions)
        {
            if (IsWithinGrid(pos) && !IsObstacle(pos) && !IsPlayerPosition(pos))
            {
                float distance = Vector3Int.Distance(enemyGridPos, pos);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = pos;
                }
            }
        }

        path = GridPathfinder.FindPath(transform.position, GridToWorldPosition(bestTarget), obstacleData, playerController.transform.position);
        isMoving = true;
    }

    private void MoveAlongPath()
    {
        if (path != null && path.Count > 0)
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
                TurnManager.instance.ToggleTurn();  // Switch to player's turn
            }
        }
        else
        {
            isMoving = false;
        }
    }

    private Vector3Int WorldToGridPosition(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x), 0, Mathf.RoundToInt(position.z));
    }

    private Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0.5f, gridPosition.z);
    }

    private bool IsWithinGrid(Vector3Int gridPosition)
    {
        int gridSize = 10;
        return gridPosition.x >= 0 && gridPosition.x < gridSize && gridPosition.z >= 0 && gridPosition.z < gridSize;
    }

    private bool IsObstacle(Vector3Int gridPosition)
    {
        int gridSize = 10;
        int index = gridPosition.x * gridSize + gridPosition.z;
        return obstacleData.obstacleGrid[index];
    }

    private bool IsPlayerPosition(Vector3Int gridPosition)
    {
        Vector3Int playerGridPos = WorldToGridPosition(playerController.transform.position);
        return gridPosition == playerGridPos;
    }
}
