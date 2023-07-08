using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PersistentCanvas : MonoBehaviour
{
    [Tooltip("References to the ingredient slots associated with the scene-persistent canvas.")]
    [SerializeField] private IngredientSlot[] inventorySlots;
    [Tooltip("Reference to the 'Content' object for the inventory scroll view.")]
    [SerializeField] private RectTransform scrollViewContent;
    [Tooltip("Reference to the confirm ingredients button game object.")]
    [SerializeField] private GameObject confirmIngredientsButton;

    public int CurrentItemCount { get; private set; } = 0;

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
        CurrentItemCount = invCount;
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

    public void FinishSlicingIngredients()
    {
        scrollViewContent.transform.parent.parent.gameObject.SetActive(false);
        SceneManager.LoadScene(3);
        gameObject.SetActive(false);
    }
}
