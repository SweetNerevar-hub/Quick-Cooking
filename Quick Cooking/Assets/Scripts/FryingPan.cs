using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan : MonoBehaviour
{
    [SerializeField] private int maximumPieces = 20;
    [SerializeField] private float touchRadius = 1;
    [SerializeField] private float stirForce = 2;

    private List<IngredientPiece> pieces = new List<IngredientPiece>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Ingredient ingredient in GameState.Ingredients)
        {
            SpawnPieces(ingredient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Collider2D hit = Physics2D.OverlapCircle(touchPos, touchRadius);
            if(hit != null && hit.TryGetComponent(out Rigidbody2D rb) == true)
            {
                rb.AddForce((Vector2)hit.transform.position - touchPos * stirForce, ForceMode2D.Impulse);
            }
        }
    }

    public bool SpawnPieces(Ingredient ingredient)
    {
        int amount = maximumPieces / GameState.Ingredients.Count;
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
