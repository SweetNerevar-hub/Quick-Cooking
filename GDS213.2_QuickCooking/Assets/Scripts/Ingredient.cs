using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient")]
public class Ingredient : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private FoodGroupType foodGroup;
    [SerializeField] private float cost;

    public string ID { get { return id; } }
    public FoodGroupType FoodGroup { get { return foodGroup; } }
    public float Cost { get { return cost; } }
}

public enum FoodGroupType { fruit, vegetable, grain, protein, dairy }