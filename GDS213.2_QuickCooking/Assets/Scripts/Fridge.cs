using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, ILoggable
{
    [SerializeField] private bool debug = false;
    [SerializeField] private GameObject fridge;
    [SerializeField] private int startingIngredientsPerGroup = 3;

    private int ingredientsPerGroup = 0;
    private List<Ingredient> currentIngredients;
    private IngredientSlot[] ingredientIcons;

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
            ingredientIcons = fridge.GetComponentsInChildren<IngredientSlot>();
        }
    }

    void Start()
    {
        ingredientsPerGroup = startingIngredientsPerGroup;
        foreach (IngredientSlot icon in ingredientIcons)
        {
            icon.UpdateIcon(null);
        }
        PopulateFridge();
    }

    public void PopulateFridge()
    {
        Log($"Populating the fridge with random ingredients.");
        currentIngredients = new List<Ingredient>();
        foreach(KeyValuePair<FoodGroupType, bool> group in GameManager.Instance.UnlockedFoodGroups)
        {
            if (group.Value == true)
            {
                List<Ingredient> randomIngredients = GameManager.Instance.GetRandomIngredients(GameManager.Instance.GetFoodGroupIngredients(GameManager.Instance.UnlockedIngredients, group.Key), ingredientsPerGroup);
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
        for (int i = 0; i < ingredientIcons.Length; i++)
        {
            if (i < currentIngredients.Count)
            {
                int r = Random.Range(0, ingredientIcons.Length);
                while (ingredientIcons[r].gameObject.activeSelf == true)
                {
                    r = Random.Range(0, ingredientIcons.Length);
                }
                ingredientIcons[r].UpdateIcon(currentIngredients[i]);
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
        int random = Random.Range(0, ingredientIcons.Length);
        while (ingredientIcons[random].CurrentIngredient != null)
        {
            random = Random.Range(0, ingredientIcons.Length);
        }
        ingredientIcons[random].UpdateIcon(ingredient);
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
