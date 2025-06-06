using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerMoney;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        Debug.Log($"Você ganhou {amount} moedas! Total: {playerMoney}");
    }
}
