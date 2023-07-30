using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the data and functionality related to the cutting board mechanics.
/// </summary>
public class CuttingBoard : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("Reference to the sliceable object in the scene.")]
    [SerializeField] private SliceableIngredient sliceableObject;
    [Tooltip("The amount of time the player has to 'complete' a slice.")]
    [SerializeField] private float sliceTime = 0.5f;
    [Tooltip("The minimum distance for a swipe to qualify as a slice.")]
    [SerializeField] [Range(0, 1)] private float minSliceDistance = 0.35f;

    AudioSource audioSource;    //written by Cameron Moore
    [SerializeField] AudioClip[] slicingFoodSFX;    //written by Cameron Moore

    private Vector2 sliceStart = Vector2.zero;  //the initial position slice input is detected
    private Vector2 sliceEnd = Vector2.zero;    //the final position slice input is detected
    private float sliceTimer = -1;  //timer that checks if the slice is completed quick enough
    private List<Ingredient> preparedIngredients = new List<Ingredient>();  //stores the ingredients that have finished being prepared

    /// <summary>
    /// The current instance of the cutting board.
    /// </summary>
    public static CuttingBoard Instance { get; private set; }

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)   //a cutting board already exists
        {
            Destroy(this);  //destroy this instance of the script
        }
        else   //setup singleton
        {
            Instance = this;
            sliceableObject.gameObject.SetActive(false);
            IngredientSlot.OnIngredientSelected += OnIngredientSelected;

            audioSource = GetComponent<AudioSource>();    //written by Cameron Moore
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
        }
    }

    /// <summary>
    /// Attempts to transfer the passed ingredient to the cutting board.
    /// </summary>
    /// <param name="ingredient">The ingredient to transfer to the cutting board.</param>
    /// <returns>Returns true if the passed ingredient was able to be transferred onto the cutting board.</returns>
    private bool PutIngredientOnBoard(Ingredient ingredient)
    {
        if (sliceableObject.gameObject.activeSelf == false)
        {
            sliceableObject.gameObject.SetActive(true);
            sliceableObject.Prepare(ingredient);
            preparedIngredients.Add(ingredient);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Executes every frame.
    /// </summary>
    private void Update()
    {
        if (sliceableObject.gameObject.activeSelf == true)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)    //if a the beginning of a touch has been detected
            {
                sliceStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                sliceTimer = 0;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && sliceTimer >= 0)    //if the end of a touch is detected
            {
                sliceTimer = -1;
                sliceEnd = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Log($"Slice distance: {Vector2.Distance(sliceStart, sliceEnd)}");
                if (sliceEnd != sliceStart && Vector2.Distance(sliceStart, sliceEnd) >= minSliceDistance)     //check the touch was not just a tap (start/end are the same)
                {
                    RaycastHit2D hit = Physics2D.Raycast(sliceStart, sliceEnd - sliceStart);
                    if (hit.collider != null && hit.collider.gameObject == sliceableObject.gameObject)
                    {
                        int chooseRandomSlicingSound = Random.Range(0, slicingFoodSFX.Length);
                        audioSource.PlayOneShot(slicingFoodSFX[chooseRandomSlicingSound]);    //written by Cameron Moore

                        if (sliceableObject.Slice() == true)
                        {
                            Log("Finished preparing ingredient.");
                            sliceableObject.gameObject.SetActive(false);
                            if (preparedIngredients.Count == GameState.CurrentIngredients.Count)
                            {
                                IngredientSlot.OnIngredientSelected -= OnIngredientSelected;
                                UI_PersistentCanvas.Instance.FinishSlicingIngredients();
                            }
                        }
                    }
                }
            }
        }

        if(sliceTimer >= 0) //slice timer functionality
        {
            sliceTimer += Time.deltaTime;
            if(sliceTimer > sliceTime)
            {
                sliceTimer = -1;
            }
        }
    }

    /// <summary>
    /// Event subscriber for executing ingredient transfer.
    /// </summary>
    /// <param name="ingredientSlot">The ingredient slot that was tapped from the inventory.</param>
    /// <returns>Returns true if the ingredient associated with the passed slot was successfully transferred to the board.</returns>
    private bool OnIngredientSelected(IngredientSlot ingredientSlot)
    {
        return PutIngredientOnBoard(ingredientSlot.CurrentIngredient);
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
                    Debug.Log($"[CUTTING BOARD] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[CUTTING BOARD] {message}");
                    break;
                case 2:
                    Debug.LogError($"[CUTTING BOARD] {message}");
                    break;
            }
        }
    }
}
