using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the data and functionality associated with the persistent UI elements.
/// </summary>
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
    [Tooltip("Reference to the foreground fill image of the progress bar.")]
    [SerializeField] private Image progressFillImage;
    [Tooltip("The amount of experience the player earns at the end of each stage.")]
    [SerializeField] private float experienceTransitionTime = 1f;
    [Tooltip("The amount of experience the player earns at the end of each stage.")]
    [SerializeField] private float experiencedPerStage = 25;

    private float progressBarStartValue = 0;
    private float progressBarTargetValue = 0;
    private float progressBarTimer = -1f;

    private AudioSource audioSource;    //written by Cameron Moore.
    [SerializeField] private AudioClip UIMouseClick;    //written by Cameron Moore.

    /// <summary>
    /// The current instance of the persistent UI.
    /// </summary>
    public static UI_PersistentCanvas Instance { get; private set; }

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        if(Instance != null)    //a persistent UI already exists
        {
            Destroy(this);  //destroy this instance of the script
        }
        else    //setup singleton
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    /// <summary>
    /// Executes when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Executes on the first frame of the game.
    /// </summary>
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  //written by Cameron Moore.

        IngredientSlot.OnIngredientSelected += OnIngredientSelected;
        foreach (IngredientSlot slot in inventorySlots)
        {
            slot.UpdateIcon(null);
        }
        confirmIngredientsButton.SetActive(false);
        progressFillImage.fillAmount = GameState.Experience / GameState.NextUnlockTarget;
    }

    private void Update()
    {
        if(progressBarTimer >= 0)
        {
            progressBarTimer += Time.deltaTime;
            float step = Mathf.Lerp(progressBarStartValue, progressBarTargetValue, progressBarTimer / experienceTransitionTime);
            if(step > 1)
            {
                progressBarStartValue = 0;
                progressBarTargetValue -= 1;
                step = Mathf.Lerp(progressBarStartValue, progressBarTargetValue, progressBarTimer / experienceTransitionTime);
            }
            progressFillImage.fillAmount = step;
            if (progressBarTimer > experienceTransitionTime)
            {
                progressBarTimer = -1;
                progressBarStartValue = 0;
            }
        }
    }

    /// <summary>
    /// Adds the passed ingredient to the inventory.
    /// </summary>
    /// <param name="ingredient">The ingredient to add to the inventory.</param>
    /// <returns>Returns true if the passed ingredient was successfully added to the inventory.</returns>
    private bool AddIngredientToInventory(Ingredient ingredient)
    {
        foreach (IngredientSlot slot in inventorySlots)
        {
            if (slot.gameObject.activeSelf == false)
            {
                slot.UpdateIcon(ingredient);
                GameState.CurrentIngredients.Add(ingredient);
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

    /// <summary>
    /// Enables the confirmation button if the current ingredients is greater than or equal to three.
    /// Else disables the confirmation button.
    /// </summary>
    private void UpdateIngredientConfirmationButtonStatus()
    {
        if (GameState.CurrentIngredients.Count >= 3)
        {
            confirmIngredientsButton.SetActive(true);
            Log($"Inventory confirmation button enabled ({GameState.CurrentIngredients.Count} ingredients).");
        }
        else
        {
            confirmIngredientsButton.SetActive(false);
            Log($"Inventory confirmation button disabled ({GameState.CurrentIngredients.Count} ingredients).");
        }
    }

    /// <summary>
    /// Event subscription response to UI ingredient slot selection.
    /// </summary>
    /// <param name="ingredientSlot">The selected ingredient slot.</param>
    /// <returns>Returns false if there is no ingredient associated with the passed slot, 
    /// or if the ingredient could not be added to the inventory (if the ingredient slot is not an inventory icon).</returns>
    private bool OnIngredientSelected(IngredientSlot ingredientSlot)
    {
        if (ingredientSlot.CurrentIngredient != null)
        {
            if (ingredientSlot.IsInventoryIcon == false)    //ingredient slot is not an inventory icon
            {
                return AddIngredientToInventory(ingredientSlot.CurrentIngredient);
            }
            else    //ingredient slot is an inventory icon
            {
                UpdateIngredientConfirmationButtonStatus();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Grants the player the passed amount of experience.
    /// </summary>
    /// <param name="experience">The amount of given experience.</param>
    private void AddExperience(float experience)
    {
        GameState.Experience += experience;
        progressBarTargetValue = GameState.Experience / GameState.NextUnlockTarget;
        if (GameState.Experience >= GameState.NextUnlockTarget) //maximum experience reached
        {
            GameState.Experience -= GameState.NextUnlockTarget;
            GameState.NextUnlockTarget = Mathf.Abs(GameState.NextUnlockTarget * 1.1f);
            foreach (KeyValuePair<FoodGroupType, bool> pair in GameState.UnlockedFoodGroups)    //unlock next food group
            {
                if (GameState.UnlockedFoodGroups[pair.Key] == false)
                {
                    GameState.UnlockedFoodGroups[pair.Key] = true;
                    Log(pair.Key + " unlocked.");
                    break;
                }
            }
        }
        progressBarTimer = 0;
        progressBarStartValue = progressFillImage.fillAmount;
    }

    /// <summary>
    /// Confirms the current scene ingredients and loads the preparation scene.
    /// </summary>
    public void ConfirmIngredientSelection()
    {
        audioSource.PlayOneShot(UIMouseClick);  //written by Cameron Moore.

        confirmIngredientsButton.SetActive(false);
        IngredientSlot.OnIngredientSelected -= OnIngredientSelected;
        AddExperience(experiencedPerStage);
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Loads the frying pan scene.
    /// </summary>
    public void FinishSlicingIngredients()
    {
        inventoryScrollView.gameObject.SetActive(false);
        AddExperience(experiencedPerStage);
        SceneManager.LoadScene(3);
    }

    /// <summary>
    /// Modifies the passed ingredient pieces to be scene-persistent, and lods the next scene.
    /// </summary>
    /// <param name="pieces">The list of ingredient pieces to bring to the meal scene.</param>
    public void FinishCookingPieces(List<IngredientPiece> pieces)
    {
        GetComponent<AudioSource>().Play();

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
        AddExperience(experiencedPerStage);
        SceneManager.LoadScene(4);
    }

    /// <summary>
    /// Cleans up scene-persistent objects, adds experience, and loads the fridge scene.
    /// </summary>
    public void FinishedMeal()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(gameObject);
        GameState.CurrentIngredients.Clear();
        AddExperience(experiencedPerStage);
        SceneManager.LoadScene(1);
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
