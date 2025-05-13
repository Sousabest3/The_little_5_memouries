using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TabController : MonoBehaviour
{
    [Header("References")]
    public Image[] tabImages;
    public GameObject[] pages;
    
    [Header("Settings")]
    public Color inactiveTabColor = Color.grey;
    public Color activeTabColor = Color.white;
    
    void Start()
    {
        if (pages != null && pages.Length > 0)
            ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        if (tabNo < 0 || tabNo >= pages.Length || pages[tabNo] == null) return;
        
        // Deactivate all pages and set tab colors
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(false);
            
            if (i < tabImages.Length && tabImages[i] != null)
                tabImages[i].color = inactiveTabColor;
        }
        
        // Activate selected page
        pages[tabNo].SetActive(true);
        
        // Highlight active tab
        if (tabNo < tabImages.Length && tabImages[tabNo] != null)
            tabImages[tabNo].color = activeTabColor;
        
        // Special handling for map tab
        if (pages[tabNo].name == "MapPage" || tabNo == 2)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (MapTracker.Instance != null)
                MapTracker.Instance.UpdateMapPosition(currentSceneIndex);
        }
        
        // Special handling for inventory tab
        if (pages[tabNo].name == "InventoryPage" || tabNo == 1)
        {
            InventoryController inventoryController = pages[tabNo].GetComponentInChildren<InventoryController>();
            if (inventoryController != null)
            {
                inventoryController.RefreshInventory();
            }
        }
    }
}