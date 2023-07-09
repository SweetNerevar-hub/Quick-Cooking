using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_PersistentCanvas : MonoBehaviour
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("References to the ingredient slots associated with the scene-persistent canvas.")]
    [SerializeField] private IngredientSlot[] inventorySlots;
    [Tooltip("Reference to the inventory scroll view.")]
    [SerializeField] private RectTransform inventoryScrollView;
    [Tooltip("Reference to the confirm ingredients button game object.")]
    [SerializeField] private GameObject confirmIngredientsButton;
    [SerializeField] private Image progressFillImage;

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
        progressFillImage.fillAmount = GameState.Experience / GameState.NextUnlockTarget;
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
        inventoryScrollView.gameObject.SetActive(false);
        SceneManager.LoadScene(3);
    }

    public void FinishCookingPieces(List<IngredientPiece> pieces)
    {
        foreach(IngredientPiece piece in pieces)
        {
            piece.transform.SetParent(null);
            piece.Cooked = true;
            DontDestroyOnLoad(piece);
        }
        foreach (IngredientPiece piece in pieces)
        {
            GameState.IngredientPieces.Add(piece);
        }
        SceneManager.LoadScene(4);
    }

    public void FinishedMeal()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(gameObject);
        GameState.Ingredients.Clear();
        GameState.Experience += 100;
        if(GameState.Experience >= GameState.NextUnlockTarget)
        {
            GameState.Experience -= GameState.NextUnlockTarget;
            GameState.NextUnlockTarget = Mathf.Abs(GameState.NextUnlockTarget * 1.1f);
            foreach (KeyValuePair<FoodGroupType, bool> pair in GameState.UnlockedFoodGroups)
            {
                Debug.Log(pair.Key + " - " + pair.Value);
                if (GameState.UnlockedFoodGroups[pair.Key] == false)
                {
                    GameState.UnlockedFoodGroups[pair.Key] = true;
                    Debug.Log(pair.Key + " unlocked");
                    break;
                }
            }
        }
        progressFillImage.fillAmount = GameState.Experience / GameState.NextUnlockTarget;
        SceneManager.LoadScene(1);
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
