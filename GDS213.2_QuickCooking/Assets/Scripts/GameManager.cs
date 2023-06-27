using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, ILoggable
{
    [SerializeField] private bool debug = false;
    [SerializeField] private float initialFunds = 100;
    [SerializeField] private float weeklyFunds = 100;
    [SerializeField] private Text wellbeingText;
    [SerializeField] private Text fundsText;
    [SerializeField] private Text purchasedListText;
    [SerializeField] private Fridge fridge;
    [SerializeField] private Text[] mealsText;
    [SerializeField] private Meal[] mealDatabase;

    private int dayIndex = 0;
    private float healthRating = 0;
    private float currentFunds = 0f;
    public readonly Dictionary<FoodGroupType, bool> UnlockedFoodGroups = new Dictionary<FoodGroupType, bool>
    {
        { FoodGroupType.fruit, true},
        { FoodGroupType.vegetable, false},
        { FoodGroupType.grain, false},
        { FoodGroupType.protein, false},
        { FoodGroupType.dairy, false}
    };
    private List<Ingredient> purchasedIngredients = new List<Ingredient>();
    private Meal recipe;

    public float CurrentFunds 
    { 
        get 
        { 
            return currentFunds; 
        }
        set
        {
            currentFunds = value;
            UpdateFundsText();
        }
    }

    private void Awake()
    {
        fridge.OnAttemptPurchaseEvent += PurchaseIngredient;
        mealsText[0].transform.parent.parent.gameObject.SetActive(false);
        CookingMethod.SelectCookingMethodEvent += CookMeal;
        CookingMethod.CookingCompleteEvent += CompleteCooking;
    }

    private void CompleteCooking(float outcome)
    {
        healthRating = Mathf.Clamp(healthRating + outcome, -1, 1);
        wellbeingText.text = $"Wellbeing: {healthRating}";
        purchasedListText.transform.parent.gameObject.SetActive(true);
        fridge.CheckFridge();
        UpdatePurchasedText();
        NextDay();
    }

    private void NextDay()
    {
        dayIndex++;
        if (dayIndex >= 6)
        {
            CurrentFunds += weeklyFunds;
            dayIndex = 0;
        }
    }

    private void CookMeal(CookingMethod cookingMethod)
    {
        if (purchasedIngredients.Count > 0 && recipe != null)
        {
            foreach (Ingredient i in recipe.Ingredients)
            {
                purchasedIngredients.Remove(i);
            }
            cookingMethod.Cook(recipe);
            recipe = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentFunds = initialFunds;
        fridge.CheckFridge();
        UpdatePurchasedText();
        wellbeingText.text = $"Wellbeing: {healthRating}";
    }

    private void UpdateFundsText()
    {
        fundsText.text = $"Funds: {currentFunds.ToString("C2")}";
    }

    private void UpdatePurchasedText()
    {
        string text = string.Empty;
        if (purchasedIngredients.Count > 0)
        {
            for (int i = 0; i < purchasedIngredients.Count; i++)
            {
                text += $"{i + 1}. {purchasedIngredients[i].ID} ({char.ToUpper(purchasedIngredients[i].FoodGroup.ToString()[0])}{purchasedIngredients[i].FoodGroup.ToString().Substring(1)})";
                if (i != purchasedIngredients.Count)
                {
                    text += '\n';
                }
            }
        }
        purchasedListText.text = text;
    }

    private bool PurchaseIngredient(Ingredient ingredient)
    {
        if (CurrentFunds - ingredient.Cost < 0)
        {
            return false;
        }
        CurrentFunds -= ingredient.Cost;
        purchasedIngredients.Add(ingredient);
        UpdatePurchasedText();
        return true;
    }

    public void SkipDay()
    {
        healthRating = Mathf.Clamp(healthRating - 0.05f, -1, 1);
        wellbeingText.text = $"Wellbeing: {healthRating}";
        mealsText[0].transform.parent.parent.gameObject.SetActive(false);
        NextDay();
        purchasedListText.transform.parent.gameObject.SetActive(true);
        fridge.CheckFridge();
        UpdatePurchasedText();
    }

    public void ConfirmIngredients()
    {
        if (purchasedIngredients.Count > 0)
        {
            purchasedListText.transform.parent.gameObject.SetActive(false);
            mealsText[0].transform.parent.parent.gameObject.SetActive(true);
            for (int i = 0; i < mealsText.Length; i++)
            {
                if (i < mealDatabase.Length)
                {
                    mealsText[i].text = $"{i + 1}. {mealDatabase[i].ID} [{mealDatabase[i].NutritionalValue}]";
                    for(int x = 0; x < mealDatabase[i].Ingredients.Length; x++)
                    {
                        if (purchasedIngredients.Contains(mealDatabase[i].Ingredients[x]) == false)
                        {
                            mealsText[i].color = Color.red;
                            break;
                        }
                        else if(x == mealDatabase[i].Ingredients.Length - 1)
                        {
                            mealsText[i].color = Color.green;
                        }
                    }
                    mealsText[i].transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    mealsText[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ConfirmMeal(int index)
    {
        for (int i = 0; i < mealDatabase[index].Ingredients.Length; i++)
        {
            if (purchasedIngredients.Contains(mealDatabase[index].Ingredients[i]) == false)
            {
                return;
            }
        }
        recipe = mealDatabase[index];
        Log($"Selected {recipe.ID}");
        mealsText[0].transform.parent.parent.gameObject.SetActive(false);
    }

    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[GAME MANAGER] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[GAME MANAGER] {message}");
                    break;
                case 2:
                    Debug.LogError($"[GAME MANAGER] {message}");
                    break;
            }
        }
    }
}

public interface ILoggable
{
    void Log(string message, int level = 0);
}