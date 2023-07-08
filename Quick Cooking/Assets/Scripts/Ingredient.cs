using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private FoodGroupType foodGroup;
    [SerializeField] private Sprite sprite;

    public string ID { get { return id; } }
    public FoodGroupType FoodGroup { get { return foodGroup; } }
    public Sprite Sprite { get { return sprite; } }
}

public enum FoodGroupType { dairy = 0, fruit = 1, grain = 2, protein = 3, vegetable = 4 }