using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("Defines the amount of ingredients per group that will populate the fridge.")]
    [SerializeField] private int startingIngredientsPerGroup = 3;

    private int ingredientsPerGroup = 0;
    private List<Ingredient> currentIngredients;
    private List<IngredientSlot> fridgeIngredientSlots;

    public static Fridge Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            fridgeIngredientSlots = new List<IngredientSlot>(GetComponentsInChildren<IngredientSlot>());
            IngredientSlot.OnIngredientSelected += OnIngredientSelected;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            IngredientSlot.OnIngredientSelected -= OnIngredientSelected;
        }
    }

    void Start()
    {
        ingredientsPerGroup = startingIngredientsPerGroup;
        foreach (IngredientSlot icon in fridgeIngredientSlots)
        {
            icon.UpdateIcon(null);
        }
        PopulateFridge();
    }

    public void PopulateFridge()
    {
        Log($"Populating the fridge with random ingredients.");
        currentIngredients = new List<Ingredient>();
        foreach(KeyValuePair<FoodGroupType, bool> group in GameState.UnlockedFoodGroups)
        {
            if (group.Value == true)
            {
                List<Ingredient> randomIngredients = GameManager.Instance.GetRandomIngredients(GameManager.Instance.GetFoodGroupIngredients(GameState.UnlockedIngredients, group.Key), ingredientsPerGroup);
                if (randomIngredients != null)
                {
                    currentIngredients.AddRange(randomIngredients);
                    Log($"Added {randomIngredients.Count} from the group {group.Key} to the fridge.");
                }
            }
        }
        UpdateFridgeIngredientIcons();
    }

    private void UpdateFridgeIngredientIcons()
    {
        for (int i = 0; i < fridgeIngredientSlots.Count; i++)
        {
            if (i < currentIngredients.Count)
            {
                int r = Random.Range(0, fridgeIngredientSlots.Count);
                while (fridgeIngredientSlots[r].gameObject.activeSelf == true)
                {
                    r = Random.Range(0, fridgeIngredientSlots.Count);
                }
                fridgeIngredientSlots[r].UpdateIcon(currentIngredients[i]);
                Log($"Updated fridge ingredient at index {r} to {currentIngredients[i].ID}.");
            }
            else
            {
                break;
            }
        }
    }

    public bool PutIngredientBack(Ingredient ingredient)
    {
        int random = Random.Range(0, fridgeIngredientSlots.Count);
        while (fridgeIngredientSlots[random].CurrentIngredient != null)
        {
            random = Random.Range(0, fridgeIngredientSlots.Count);
        }
        fridgeIngredientSlots[random].UpdateIcon(ingredient);
        GameState.Ingredients.Remove(ingredient);
        Log($"Put {ingredient.ID} back in the fridge.");
        return true;
    }

    public bool OnIngredientSelected(IngredientSlot ingredientSlot)
    {
        if (ingredientSlot.IsInventoryIcon == true)
        {
            if (ingredientSlot.CurrentIngredient != null)
            {
                return PutIngredientBack(ingredientSlot.CurrentIngredient);
            }
        }
        return true;
    }

    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[FRIDGE] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[FRIDGE] {message}");
                    break;
                case 2:
                    Debug.LogError($"[FRIDGE] {message}");
                    break;
            }
        }
    }
}
