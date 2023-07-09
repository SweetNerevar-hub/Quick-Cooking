using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;
    [Tooltip("Reference to the sliceable object in the scene.")]
    [SerializeField] private SliceableIngredient sliceableObject;
    [Tooltip("The amount of time the player has to lift their finger when slicing.")]
    [SerializeField] private float sliceTime = 0.5f;

    private Vector2 sliceStart = Vector2.zero;
    private Vector2 sliceEnd = Vector2.zero;
    private float sliceTimer = -1;
    private List<Ingredient> preparedIngredients = new List<Ingredient>();

    public static CuttingBoard Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            sliceableObject.gameObject.SetActive(false);
            IngredientSlot.OnIngredientSelected += OnIngredientSelected;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool PutIngredientOnBoard(Ingredient ingredient)
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

    private void Update()
    {
        if (sliceableObject.gameObject.activeSelf == true)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                sliceStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                sliceTimer = 0;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && sliceTimer >= 0)
            {
                sliceTimer = -1;
                sliceEnd = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                if (sliceEnd != sliceStart)
                {
                    RaycastHit2D hit = Physics2D.Raycast(sliceStart, sliceEnd - sliceStart);
                    if (hit.collider != null && hit.collider.gameObject == sliceableObject.gameObject)
                    {
                        if (sliceableObject.Slice() == true)
                        {
                            Log("FINISHED SLICING INGREDIENT");
                            sliceableObject.gameObject.SetActive(false);
                            if (preparedIngredients.Count == GameState.Ingredients.Count)
                            {
                                IngredientSlot.OnIngredientSelected -= OnIngredientSelected;
                                UI_PersistentCanvas.Instance.FinishSlicingIngredients();
                            }
                        }
                    }
                }
            }
        }

        if(sliceTimer >= 0)
        {
            sliceTimer += Time.deltaTime;
            if(sliceTimer > sliceTime)
            {
                sliceTimer = -1;
            }
        }
    }

    public bool OnIngredientSelected(IngredientSlot ingredientSlot)
    {
        return PutIngredientOnBoard(ingredientSlot.CurrentIngredient);
    }

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
