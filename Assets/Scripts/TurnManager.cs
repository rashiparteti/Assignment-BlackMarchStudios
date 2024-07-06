using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public bool isPlayerTurn = true;

    void Awake()
    {
        // Ensure a single instance of GameManager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method to toggle turns
    public void ToggleTurn()
    {
        isPlayerTurn = !isPlayerTurn;
    }
}
