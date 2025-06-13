using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text damageText;
    public float floatSpeed = 1f;
    public float duration = 1f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    public void Setup(int amount, bool isHeal)
    {
        damageText.text = amount.ToString();
        damageText.color = isHeal ? Color.green : Color.red;
    }
}
