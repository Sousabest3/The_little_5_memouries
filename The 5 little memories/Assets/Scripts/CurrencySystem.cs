using UnityEngine;
using TMPro;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance { get; private set; }

    [SerializeField] private int startingGold = 0;
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
        Debug.Log($"💰 Recebeu {amount} Gold. Total: {currentGold}");
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

        Debug.Log("⚠️ Dinheiro insuficiente.");
        return false;
    }

    public void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {currentGold}";
    }

    public int GetGold() => currentGold;

    // ✅ Adicionado para permitir carregar gold do save
    public void SetGold(int value)
    {
        currentGold = value;
        UpdateUI();
    }
}
