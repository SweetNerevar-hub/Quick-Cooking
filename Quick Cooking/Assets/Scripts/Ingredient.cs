using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private FoodGroupType foodGroup;
    [SerializeField] private Sprite fridgeSprite;
    [SerializeField] private Sprite firstSliceSprite;
    [SerializeField] private Sprite lastSliceSprite;
    [SerializeField] private Sprite middleSlicesSprite;

    public string ID { get { return id; } }
    public FoodGroupType FoodGroup { get { return foodGroup; } }
    public Sprite FridgeSprite { get { return fridgeSprite; } }
    public Sprite FirstSliceSprite { get { return firstSliceSprite; } }
    public Sprite LastSliceSprite { get { return lastSliceSprite; } }
    public Sprite MiddleSlicesSprite { get { return middleSlicesSprite; } }
}

public enum FoodGroupType { dairy = 0, fruit = 1, grain = 2, protein = 3, vegetable = 4 }