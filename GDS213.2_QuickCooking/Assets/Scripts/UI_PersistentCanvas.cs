using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PersistentCanvas : MonoBehaviour
{
    [SerializeField] private IngredientSlot[] inventorySlots;
    [SerializeField] private RectTransform scrollViewContent;
    [SerializeField] private GameObject confirmIngredientsButton;

    public static UI_PersistentCanvas Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        foreach (IngredientSlot slot in inventorySlots)
        {
            slot.UpdateIcon(null);
        }
        confirmIngredientsButton.SetActive(false);
    }

    public bool AddIngredientToInventory(Ingredient ingredient)
    {
        foreach (IngredientSlot slot in inventorySlots)
        {
            if (slot.gameObject.activeSelf == false)
            {
                slot.UpdateIcon(ingredient);
                UpdateIngredientConfirmationButtonStatus();
                return true;
            }
            else if (slot.CurrentIngredient == ingredient)
            {
                return false;
            }
        }
        return false;
    }

    public void UpdateIngredientConfirmationButtonStatus()
    {
        int invCount = 0;
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].gameObject.activeSelf == true)
            {
                invCount++;
            }
        }
        if (invCount >= 3)
        {
            confirmIngredientsButton.SetActive(true);
        }
        else
        {
            confirmIngredientsButton.SetActive(false);
        }
    }

    public void ConfirmIngredientSelection()
    {
        confirmIngredientsButton.SetActive(false);
        SceneManager.LoadScene(2);
    }
}
