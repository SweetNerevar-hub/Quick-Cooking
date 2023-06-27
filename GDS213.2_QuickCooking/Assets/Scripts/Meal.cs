using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Meal", menuName = "Meal")]
[System.Serializable]
public class Meal : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Ingredient[] ingredients;
    [SerializeField] private PreparationType[] preparationMethod = new PreparationType[] { PreparationType.wash };
    [SerializeField] private CookingMethodType cookingMethod;
    [SerializeField] private float cookingTime;
    [SerializeField] [Range(100, 300)] private float cookingTemperature;
    [SerializeField] [Range(0, 100)] private float nutritionalValue;

    public string ID { get { return id; } }
    public Ingredient[] Ingredients { get { return ingredients; } }
    public PreparationType[] PreparationMethod { get { return preparationMethod; } }
    public CookingMethodType CookingMethod { get { return cookingMethod; } }
    public float CookingTime { get { return cookingTime; } }
    public float CookingTemperature { get { return cookingTemperature; } }
    public float NutritionalValue { get { return nutritionalValue; } }
}