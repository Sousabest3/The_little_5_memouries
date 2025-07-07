using UnityEngine;
using UnityEngine.UI;

public class MapTracker : MonoBehaviour
{
    public static MapTracker Instance;

    [Header("Referências")]
    public RectTransform mapPage; // Referência à MapPage
    public Image playerLocationMarker;

    [Header("Configurações do Mapa")]
    public Vector2[] scenePositions; // Posições para cada cena
    public float markerScale = 1f;

    [Header("Comportamento do Marcador")]
    public bool showMarker = false; // ⬅️ Novo: controla se mostra o marcador

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Não use DontDestroyOnLoad se o mapa está no canvas de pausa
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMapPosition(int sceneIndex)
    {
        if (mapPage == null || playerLocationMarker == null) return;

        if (sceneIndex >= 0 && sceneIndex < scenePositions.Length)
        {
            if (!mapPage.gameObject.activeSelf)
                mapPage.gameObject.SetActive(true);

            playerLocationMarker.rectTransform.anchoredPosition = scenePositions[sceneIndex];
            playerLocationMarker.rectTransform.localScale = Vector3.one * markerScale;

            // Ativa ou desativa o marcador baseado na flag
            playerLocationMarker.gameObject.SetActive(showMarker);
        }
    }
}
