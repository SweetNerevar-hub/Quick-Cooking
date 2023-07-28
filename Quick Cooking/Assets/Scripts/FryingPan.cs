using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the data and functionality associated with the frying pan mechanics.
/// </summary>
public class FryingPan : MonoBehaviour
{
    [Tooltip("The maximum number of ingredient pieces that can be spawned into the pan.")]
    [SerializeField] private int maximumPieces = 20;
    [Tooltip("The radius surrounding the input position for detecting ingredient pieces.")]
    [SerializeField] private float touchRadius = 1;
    [Tooltip("The force added to ingredient pieces by player input.")]
    [SerializeField] private float stirForce = 2;

    private List<IngredientPiece> pieces = new List<IngredientPiece>(); //the spawned pieces
    private int cookedPieces = 0;   //the number of pieces that have completely cooked

    /// <summary>
    /// Executes on the first frame of the game.
    /// </summary>
    private void Start()
    {
        IngredientPiece.OnPieceCooked += OnPieceCooked;
        foreach (Ingredient ingredient in GameState.CurrentIngredients)
        {
            SpawnPieces(ingredient);
        }
    }

    /// <summary>
    /// Executes every frame.
    /// </summary>
    private void Update()
    {
        if (Input.touchCount > 0)   //if a touch is detected
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Collider2D hit = Physics2D.OverlapCircle(touchPos, touchRadius);
            if(hit != null && hit.TryGetComponent(out Rigidbody2D rb) == true)
            {
                rb.AddForce((Vector2)hit.transform.position - touchPos * stirForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Event subscription response to an ingredient piece finishing cooking.
    /// </summary>
    private void OnPieceCooked()
    {
        cookedPieces++;
        if(cookedPieces == pieces.Count)
        {
            IngredientPiece.OnPieceCooked -= OnPieceCooked;
            UI_PersistentCanvas.Instance.FinishCookingPieces(pieces);
        }
    }

    /// <summary>
    /// Spawns in the ingredient pieces associated with the passed ingredient.
    /// The number of pieces spawned is determined by the ratio of the maximum 
    /// count divided by the total number of ingredients.
    /// </summary>
    /// <param name="ingredient">The ingredient to spawn in ingredient pieces for.</param>
    /// <returns>Returns true.</returns>
    private bool SpawnPieces(Ingredient ingredient)
    {
        int amount = maximumPieces / GameState.CurrentIngredients.Count;
        for (int i = 0; i < amount; i++)
        {
            if (pieces.Count >= maximumPieces)
            {
                break;
            }
            pieces.Add(Instantiate(ingredient.SlicedPrefab, transform));
        }
        return true;
    }
}
