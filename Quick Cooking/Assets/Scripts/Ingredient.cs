using UnityEngine;

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

    public string ID { get { return id; } }
    public FoodGroupType FoodGroup { get { return foodGroup; } }
    public Sprite FridgeSprite { get { return fridgeSprite; } }
    public Sprite FirstSliceSprite { get { return firstSliceSprite; } }
    public Sprite LastSliceSprite { get { return lastSliceSprite; } }
    public Sprite MiddleSlicesSprite { get { return middleSlicesSprite; } }
}

public enum FoodGroupType { dairy = 0, fruit = 1, grain = 2, protein = 3, vegetable = 4 }