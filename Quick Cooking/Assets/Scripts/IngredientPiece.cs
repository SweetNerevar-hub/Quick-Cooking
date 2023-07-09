using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientPiece : MonoBehaviour
{
    public delegate void IngredientPieceCookedDelegate();

    [Tooltip("The time the ingredient takes to brown.")]
    [SerializeField] private float cookingTime;
    [Tooltip("The colour of the ingredient slice when completely brown.")]
    [SerializeField] private Color brownColor;

    private SpriteRenderer spriteRenderer;
    private float targetTime = 0;
    private float cookingTimer = -1;
    private Color startColor;
    private Color targetColor;

    public bool Cooked { get; set; } = false;

    public static event IngredientPieceCookedDelegate OnPieceCooked;

    private void Awake()
    {
        TryGetComponent(out spriteRenderer);
        ChangeColour(brownColor);
    }

    private void Start()
    {
        StartCooking(0);
    }

    void Update()
    {
        if (Cooked == false)
        {
            if (cookingTimer >= 0)
            {
                cookingTimer += Time.deltaTime;
                spriteRenderer.color = new Color()
                {
                    r = Mathf.Lerp(startColor.r, targetColor.r, cookingTimer / cookingTime),
                    g = Mathf.Lerp(startColor.g, targetColor.g, cookingTimer / cookingTime),
                    b = Mathf.Lerp(startColor.b, targetColor.b, cookingTimer / cookingTime),
                    a = Mathf.Lerp(startColor.a, targetColor.a, cookingTimer / cookingTime)
                };
                if (cookingTimer >= targetTime)
                {
                    cookingTimer = -1;
                    OnPieceCooked?.Invoke();
                }
            }
        }
    }

    private void StartCooking(float timeOffset, bool overrideCookingTime = false)
    {
        targetTime = overrideCookingTime == false ? cookingTime + timeOffset : timeOffset;
        cookingTimer = 0;
    }

    public void ChangeColour(Color colorTarget)
    {
        if(colorTarget == targetColor)
        {
            return;
        }
        startColor = spriteRenderer.color;
        targetColor = colorTarget;
    }

    public void Eat()
    {
        if (Cooked == true)
        {
            GameState.IngredientPieces.Remove(this);
            Destroy(gameObject);
        }
    }
}
