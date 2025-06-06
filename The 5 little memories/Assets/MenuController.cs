using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private TabController tabController;
 
    
    [Header("Configurações")]
    [SerializeField] private bool autoOpenMapTab = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] private bool pauseGameWhenOpen = true;

    private void Awake()
    {
        // Auto-referência se não atribuído
        if (menuCanvas == null) menuCanvas = GetComponentInChildren<Canvas>()?.gameObject;
        if (tabController == null) tabController = GetComponentInChildren<TabController>();
        
    }

    private void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        bool willActivate = !menuCanvas.activeSelf;
        
        menuCanvas.SetActive(willActivate);
        
        if (pauseGameWhenOpen)
        {
            Time.timeScale = willActivate ? 0f : 1f;
        }
        
        if (willActivate)
        {
            // Chamada atualizada para usar métodos existentes
            
            if (autoOpenMapTab && tabController != null)
            {
                tabController.ActivateTab(2); // Assumindo que 2 é o índice do mapa
            }
        }
    }

    public void OpenMenu()
    {
        if (!menuCanvas.activeSelf)
        {
            ToggleMenu();
        }
    }

    public void CloseMenu()
    {
        if (menuCanvas.activeSelf)
        {
            ToggleMenu();
        }
    }
}