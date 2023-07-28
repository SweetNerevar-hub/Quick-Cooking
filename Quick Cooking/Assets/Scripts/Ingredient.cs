using UnityEngine;

/// <summary>
/// This class defines a scriptable object used to create different ingredients.
/// </summary>
[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    [Tooltip("The identity (e.g. name) for the ingredient.")]
    [SerializeField] private string id;
    [Tooltip("The food group this ingredient falls within.")]
    [SerializeField] private FoodGroupType foodGroup;
    [Tooltip("The ingredient sprite displayed in the fridge.")]
    [SerializeField] private Sprite fridgeSprite;
    [Tooltip("The ingredient slice sprite for one of the end slices.")]
    [SerializeField] private Sprite firstSliceSprite;
    [Tooltip("The ingredient slice sprite for the opposite end slice.")]
    [SerializeField] private Sprite lastSliceSprite;
    [Tooltip("The ingredient slice sprite for the middle slices.")]
    [SerializeField] private Sprite middleSlicesSprite;
    [Tooltip("The prefab for the chopped up ingredient object.")]
    [SerializeField] private IngredientPiece slicedPrefab;

    /// <summary>
    /// The ID for the ingredient.
    /// </summary>
    public string ID { get { return id; } }
    /// <summary>
    /// The food group the ingredient is included within.
    /// </summary>
    public FoodGroupType FoodGroup { get { return foodGroup; } }
    /// <summary>
    /// The sprite displayed in the fridge.
    /// </summary>
    public Sprite FridgeSprite { get { return fridgeSprite; } }
    /// <summary>
    /// The sliced sprite for the first portion of the ingredient.
    /// </summary>
    public Sprite FirstSliceSprite { get { return firstSliceSprite; } }
    /// <summary>
    /// The slice sprite for the last portion of the ingredient.
    /// </summary>
    public Sprite LastSliceSprite { get { return lastSliceSprite; } }
    /// <summary>
    /// The slice sprite for the middle portion of the ingredient.
    /// </summary>
    public Sprite MiddleSlicesSprite { get { return middleSlicesSprite; } }
    /// <summary>
    /// A reference to the sliceable object prefab for this ingredient.
    /// </summary>
    public IngredientPiece SlicedPrefab { get { return slicedPrefab; } }
}

/// <summary>
/// The different food groups for ingredients.
/// </summary>
public enum FoodGroupType { dairy = 0, fruit = 1, grain = 2, protein = 3, vegetable = 4 }