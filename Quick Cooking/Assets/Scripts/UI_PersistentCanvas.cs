using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PersistentCanvas : MonoBehaviour
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("References to the ingredient slots associated with the scene-persistent canvas.")]
    [SerializeField] private IngredientSlot[] inventorySlots;
    [Tooltip("Reference to the 'Content' object for the inventory scroll view.")]
    [SerializeField] private RectTransform scrollViewContent;
    [Tooltip("Reference to the confirm ingredients button game object.")]
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
        IngredientSlot.OnIngredientSelected += OnIngredientSelected;
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
                GameState.Ingredients.Add(ingredient);
                UpdateIngredientConfirmationButtonStatus();
                Log($"Added {ingredient.ID} to the inventory.");
                return true;
            }
            else if (slot.CurrentIngredient == ingredient)
            {
                Log($"{ingredient.ID} is already in the inventory.", 1);
                return false;
            }
        }
        Log($"Could not add {ingredient.ID} to the inventory. Inventory is full.", 1);
        return false;
    }

    public void UpdateIngredientConfirmationButtonStatus()
    {
        if (GameState.Ingredients.Count >= 3)
        {
            confirmIngredientsButton.SetActive(true);
            Log($"Inventory confirmation button enabled ({GameState.Ingredients.Count} ingredients).");
        }
        else
        {
            confirmIngredientsButton.SetActive(false);
            Log($"Inventory confirmation button disabled ({GameState.Ingredients.Count} ingredients).");
        }
    }

    public void ConfirmIngredientSelection()
    {
        confirmIngredientsButton.SetActive(false);
        IngredientSlot.OnIngredientSelected -= OnIngredientSelected;
        SceneManager.LoadScene(2);
    }

    public void FinishSlicingIngredients()
    {
        scrollViewContent.transform.parent.parent.gameObject.SetActive(false);
        SceneManager.LoadScene(3);
        gameObject.SetActive(false);
    }

    public bool OnIngredientSelected(IngredientSlot ingredientSlot)
    {
        if (ingredientSlot.CurrentIngredient != null)
        {
            if (ingredientSlot.IsInventoryIcon == false)
            {
                return AddIngredientToInventory(ingredientSlot.CurrentIngredient);
            }
            else
            {
                UpdateIngredientConfirmationButtonStatus();
                return true;
            }
        }
        return false;
    }

    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[PERSISTENT UI] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[PERSISTENT UI] {message}");
                    break;
                case 2:
                    Debug.LogError($"[PERSISTENT UI] {message}");
                    break;
            }
        }
    }
}
