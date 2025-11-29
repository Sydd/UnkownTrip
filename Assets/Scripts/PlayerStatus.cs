using UnityEngine;

public class PlayerStatus: MonoBehaviour

{
    public static PlayerStatus Instance { get; private set; }
    public PlayerState currentState;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
public   enum PlayerState
    {
        Idle,
        Moving,
        Attacking,
        Dead,
        Dash
    }
