using UnityEngine;
using TMPro;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance { get; private set; }

    [Header("Configuração")]
    [SerializeField] private int startingGold = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goldText;

    private int currentGold;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentGold = startingGold;
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateUI();
            return true;
        }

        Debug.Log("Não há dinheiro suficiente!");
        return false;
    }

    public int GetGold()
    {
        return currentGold;
    }

    private void UpdateUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {currentGold}";
        }
    }
}
