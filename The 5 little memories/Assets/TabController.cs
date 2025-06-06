using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TabController : MonoBehaviour
{
    [Header("Referências")]
    public Image[] tabImages;
    public GameObject[] pages;
    public int defaultTab = 0;
    
    [Header("Configurações")]
    public Color inactiveTabColor = Color.grey;
    public Color activeTabColor = Color.white;
    
    public int CurrentTab { get; private set; }

    void Start()
    {
        ActivateTab(defaultTab);
    }

    public void ActivateTab(int tabNo)
    {
        if (tabNo < 0 || tabNo >= pages.Length) return;
        
        CurrentTab = tabNo;
        
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == tabNo);
            
            if (i < tabImages.Length && tabImages[i] != null)
                tabImages[i].color = i == tabNo ? activeTabColor : inactiveTabColor;
        }
        
        if (pages[tabNo].name.Contains("Map") || tabNo == 2)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            MapTracker.Instance?.UpdateMapPosition(currentSceneIndex);
        }
    }
}