using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject menuCanvas;
    public TabController tabController;
    
    [Header("Settings")]
    public bool autoOpenMapTab = true;
    public int inventoryTabIndex = 1;
    public int mapTabIndex = 2;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }
    
    public void ToggleMenu()
    {
        bool willActivate = !menuCanvas.activeSelf;
        menuCanvas.SetActive(willActivate);
        PauseController.SetPause(willActivate);
        
        if (willActivate)
        {
            UpdateMapPosition();
            tabController.ActivateTab(autoOpenMapTab ? mapTabIndex : inventoryTabIndex);
        }
    }
    
    void UpdateMapPosition()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        MapTracker.Instance?.UpdateMapPosition(currentSceneIndex);
    }
}
    
