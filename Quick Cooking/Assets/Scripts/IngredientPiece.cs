using UnityEngine;

/// <summary>
/// This class handles the functionality and mechanics associated with the individual pieces of each ingredient (post-preparation).
/// </summary>
public class IngredientPiece : MonoBehaviour
{
    /// <summary>
    /// Generic delegate for when ingredient pieces are finished cooking.
    /// </summary>
    public delegate void IngredientPieceCookedDelegate();

    [Tooltip("The time the ingredient takes to brown.")]
    [SerializeField] private float cookingTime;
    [Tooltip("The colour of the ingredient slice when completely brown.")]
    [SerializeField] private Color brownColor;

    private SpriteRenderer spriteRenderer;  //a reference to the object's sprite renderer
    private float targetTime = 0;   //the target time the ingredient will cook in
    private float cookingTimer = -1;    //tracks cooking time
    private Color startColor;   //the initial colour of the ingredient
    private Color targetColor;  //the target colour of the ingredient

    /// <summary>
    /// Returns true if the ingredient piece has finished cooking.
    /// </summary>
    public bool Cooked { get; set; } = false;

    /// <summary>
    /// Invoked when an ingredient piece has finished cooking.
    /// </summary>
    public static event IngredientPieceCookedDelegate OnPieceCooked;

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        SetColourTarget(brownColor);
    }

    /// <summary>
    /// Executes on the first frame of the game.
    /// </summary>
    private void Start()
    {
        StartCooking(0);
    }

    /// <summary>
    /// Executes every frame.
    /// </summary>
    private void Update()
    {
        if (Cooked == false)
        {
            if (cookingTimer >= 0)  //is currently cooking
            {
                cookingTimer += Time.deltaTime;
                spriteRenderer.color = new Color()
                {
                    r = Mathf.Lerp(startColor.r, targetColor.r, cookingTimer / cookingTime),
                    g = Mathf.Lerp(startColor.g, targetColor.g, cookingTimer / cookingTime),
                    b = Mathf.Lerp(startColor.b, targetColor.b, cookingTimer / cookingTime),
                    a = Mathf.Lerp(startColor.a, targetColor.a, cookingTimer / cookingTime)
                };
                if (cookingTimer >= targetTime) //finished cooking
                {
                    cookingTimer = -1;
                    OnPieceCooked?.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Starts the cooking timer for the ingredient piece.
    /// </summary>
    /// <param name="timeOffset">An offset to be applied to the cooking timer.</param>
    /// <param name="overrideCookingTime">If true, the passed time offset will override the cooking time.</param>
    private void StartCooking(float timeOffset, bool overrideCookingTime = false)
    {
        targetTime = overrideCookingTime == false ? cookingTime + timeOffset : timeOffset;
        cookingTimer = 0;
    }

    /// <summary>
    /// Sets the target colour to the passed colour.
    /// </summary>
    /// <param name="colorTarget">The new colour target.</param>
    private void SetColourTarget(Color colorTarget)
    {
        if(colorTarget == targetColor)
        {
            return;
        }
        startColor = spriteRenderer.color;
        targetColor = colorTarget;
    }

    /// <summary>
    /// Destroys and stops tracking this ingredient piece.
    /// </summary>
    public void Eat()
    {
        if (Cooked == true)
        {
            PersistentAudio.Instance.EatFoodAudio();    //written by Cameron Moore.

            GameState.IngredientPieces.Remove(this);
            Destroy(gameObject);
        }
    }
}
