using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the functionality related to the fridge mechanics.
/// </summary>
public class Fridge : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("Defines the amount of ingredients per group that will populate the fridge.")]
    [SerializeField] private int startingIngredientsPerGroup = 3;

    [SerializeField] AudioClip[] grabFoodSounds;    //written by Cameron Moore.

    private int ingredientsPerGroup = 0;    //the number of ingredients to be selected from each food group
    private List<Ingredient> currentIngredients;    //the current ingredients
    private List<IngredientSlot> fridgeIngredientSlots;     //the ingredient slots associated with the fridge
    private AudioSource aSrc;   //reference to this object's audio source

    /// <summary>
    /// The current instance of the fridge.
    /// </summary>
    public static Fridge Instance { get; private set; }

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)   //a fridge already exists
        {
            Destroy(this);  //destroy this instance of the script
        }
        else   //setup singleton
        {
            Instance = this;
            fridgeIngredientSlots = new List<IngredientSlot>(GetComponentsInChildren<IngredientSlot>());
            IngredientSlot.OnIngredientSelected += OnIngredientSelected;
            TryGetComponent(out aSrc);
        }
    }

    /// <summary>
    /// Executes when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            IngredientSlot.OnIngredientSelected -= OnIngredientSelected;
        }
    }

    /// <summary>
    /// Executes on the first frame of the game.
    /// </summary>
    private void Start()
    {
        ingredientsPerGroup = startingIngredientsPerGroup;
        foreach (IngredientSlot icon in fridgeIngredientSlots)
        {
            icon.UpdateIcon(null);
        }
        PopulateFridge();
    }

    /// <summary>
    /// Populates the fridge with a number of randomly selected ingredients.
    /// </summary>
    private void PopulateFridge()
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

    /// <summary>
    /// Updates the ingredient slots to match the current ingredients in the fridge.
    /// </summary>
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

    /// <summary>
    /// Puts the passed ingredient back into the fridge via a random ingredient slot.
    /// </summary>
    /// <param name="ingredient">The ingredient being put back into the fridge from the inventory.</param>
    /// <returns>Returns false if there are no referenced fridge ingredient slots.</returns>
    private bool PutIngredientBack(Ingredient ingredient)
    {
        if (fridgeIngredientSlots.Count > 0)
        {
            int random = Random.Range(0, fridgeIngredientSlots.Count);
            while (fridgeIngredientSlots[random].CurrentIngredient != null)
            {
                random = Random.Range(0, fridgeIngredientSlots.Count);
            }
            fridgeIngredientSlots[random].UpdateIcon(ingredient);
            GameState.CurrentIngredients.Remove(ingredient);
            Log($"Put {ingredient.ID} back in the fridge.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Event subscriber for seleting an ingredient from the inventory.
    /// </summary>
    /// <param name="ingredientSlot">The ingredient slot that was selected.</param>
    /// <returns>Returns true.</returns>
    private bool OnIngredientSelected(IngredientSlot ingredientSlot)
    {
        int randomAudioClip = Random.Range(0, grabFoodSounds.Length);   //written by Cameron Moore.
        aSrc.PlayOneShot(grabFoodSounds[randomAudioClip]);   //originally written by Cameron Moore, updated by Josh Ferguson

        if (ingredientSlot.IsInventoryIcon == true)
        {
            if (ingredientSlot.CurrentIngredient != null)
            {
                return PutIngredientBack(ingredientSlot.CurrentIngredient);
            }
        }
        return true;
    }

    /// <summary>
    /// Logs the passed message to the console.
    /// </summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">Defines the alert level for the message. 0 = normal; 1 = warning; 2 = error.</param>
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
