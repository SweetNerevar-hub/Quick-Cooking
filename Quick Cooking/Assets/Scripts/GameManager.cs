using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("Database of all ingredient assets.")]
    [SerializeField] private List<Ingredient> ingredientDatabase;
    [Header("Game Start Properties")]
    [Tooltip("Defines how many ingredients from each group get unlocked when the game starts.")]
    [SerializeField] private int unlockedIngredientsPerGroup = 3;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            //unlock random ingredients for each group
            unlockedIngredientsPerGroup = unlockedIngredientsPerGroup <= 0 ? 1 : unlockedIngredientsPerGroup; //make sure default ingredients is at least 1
            Log($"Unlocked random ingredients in {FoodGroupType.dairy.ToString().ToUpper()}: {UnlockRandomIngredientInGroup(FoodGroupType.dairy, unlockedIngredientsPerGroup).ToString().ToUpper()}");
            Log($"Unlocked random ingredients in {FoodGroupType.fruit.ToString().ToUpper()}: {UnlockRandomIngredientInGroup(FoodGroupType.fruit, unlockedIngredientsPerGroup).ToString().ToUpper()}");
            Log($"Unlocked random ingredients in {FoodGroupType.grain.ToString().ToUpper()}: {UnlockRandomIngredientInGroup(FoodGroupType.grain, unlockedIngredientsPerGroup).ToString().ToUpper()}");
            Log($"Unlocked random ingredients in {FoodGroupType.protein.ToString().ToUpper()}: {UnlockRandomIngredientInGroup(FoodGroupType.protein, unlockedIngredientsPerGroup).ToString().ToUpper()}");
            Log($"Unlocked random ingredients in {FoodGroupType.vegetable.ToString().ToUpper()}: {UnlockRandomIngredientInGroup(FoodGroupType.vegetable, unlockedIngredientsPerGroup).ToString().ToUpper()}");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Returns all unlocked ingredients within a given food group.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<Ingredient> GetFoodGroupIngredients(List<Ingredient> ingredients, FoodGroupType type)
    {
        List<Ingredient> groupIngredients = new List<Ingredient>();
        foreach (Ingredient ingredient in ingredients)
        {
            if (ingredient.FoodGroup == type)
            {
                groupIngredients.Add(ingredient);
            }
        }
        Log($"Found {groupIngredients.Count} ingredients in the group {type.ToString().ToUpper()}.");
        return groupIngredients.Count > 0 ? groupIngredients : null;
    }

    /// <summary>
    /// Returns the given amount of randomly selected of ingredients from the provided list of ingredients.
    /// </summary>
    /// <param name="ingredients"></param>
    /// <param name="maxAmount"></param>
    /// <returns></returns>
    public List<Ingredient> GetRandomIngredients(List<Ingredient> ingredients, int maxAmount)
    {
        if (ingredients.Count == 0) { return null; }
        List<Ingredient> randomIngredients = ingredients;
        while (randomIngredients.Count > maxAmount)
        {
            int ingredientIndex = Random.Range(0, ingredients.Count);
            randomIngredients.RemoveAt(ingredientIndex);
            Log($"Removed {ingredientIndex} from the current ingredient population.");
        }
        Log($"Generated a list with a maximum of {maxAmount} random ingredients from the provided list of ingredients.");
        return randomIngredients;
    }

    /// <summary>
    /// Unlocks the given amount of randomly selected ingredients from the specified food group.
    /// </summary>
    private bool UnlockRandomIngredientInGroup(FoodGroupType type, int amount = 1)
    {
        if (GameState.UnlockedFoodGroups[type] == true)
        {
            Log($"Getting all ingredients for food group: {type.ToString().ToUpper()}");
            List<Ingredient> groupIngredients = GetFoodGroupIngredients(ingredientDatabase, type);
            if (groupIngredients == null)
            {
                Log($"Database does not contain any ingredients in the group {type.ToString().ToUpper()}", 1);
                return false;
            }
            Log($"Getting unlocked ingredients for food group: {type.ToString().ToUpper()}");
            List<Ingredient> unlockedGroupIngredients = GetFoodGroupIngredients(GameState.UnlockedIngredients, type);
            if (unlockedGroupIngredients != null && groupIngredients.Count == unlockedGroupIngredients.Count)
            {
                Log($"All ingredients in group {type.ToString().ToUpper()} have been unlocked!", 1);
                return false;
            }
            else if(unlockedGroupIngredients == null)
            {
                unlockedGroupIngredients= new List<Ingredient>();
            }
            int index = 0;
            while (index < amount)
            {
                if(unlockedGroupIngredients != null && groupIngredients.Count == unlockedGroupIngredients.Count)
                {
                    Log($"All ingredients in group {type.ToString().ToUpper()} have been unlocked!", 1);
                    break;
                }
                int random = Random.Range(0, groupIngredients.Count);
                if (unlockedGroupIngredients != null)
                {
                    while (unlockedGroupIngredients.Contains(groupIngredients[random]) == true) //already have this ingredient, try another random index
                    {
                        random = Random.Range(0, groupIngredients.Count);
                    }
                }
                GameState.UnlockedIngredients.Add(groupIngredients[random]);
                unlockedGroupIngredients.Add(groupIngredients[random]);
                Log($"Unlocked group {type.ToString().ToUpper()} ingredient: {groupIngredients[random].name.ToString().ToUpper()}");
                index++;
            }
            return true;
        }
        Log($"Cannot unlock new foods in group {type.ToString().ToUpper()} - food group has not been unlocked!", 1);
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
                    Debug.Log($"[GAME MANAGER] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[GAME MANAGER] {message}");
                    break;
                case 2:
                    Debug.LogError($"[GAME MANAGER] {message}");
                    break;
            }
        }
    }
}

public interface ILoggable
{
    void Log(string message, int level = 0);
}

public static class GameState
{
    public static readonly Dictionary<FoodGroupType, bool> UnlockedFoodGroups = new Dictionary<FoodGroupType, bool>
    {
        { FoodGroupType.dairy, false},
        { FoodGroupType.fruit, false},
        { FoodGroupType.grain, false},
        { FoodGroupType.protein, false},
        { FoodGroupType.vegetable, true}
    };
    public static readonly List<Ingredient> UnlockedIngredients = new List<Ingredient>();
    public static List<Ingredient> Ingredients { get; private set; } = new List<Ingredient>();
    public static List<IngredientPiece> IngredientPieces { get; private set; } = new List<IngredientPiece>();
    public static float Experience { get; set; } = 0;
    public static float NextUnlockTarget { get; set; } = 100;
}