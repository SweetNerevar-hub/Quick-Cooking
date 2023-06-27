using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fridge : MonoBehaviour, ILoggable
{
    public delegate bool PurchaseIngredientDelegate(Ingredient ingredient);

    [SerializeField] private bool debug = false;
    [SerializeField] private int ingredientsMin, ingredientsMax;
    [SerializeField] private Ingredient[] ingredientDatabase;
    [SerializeField] private Text[] ingredientsText;

    private List<Ingredient> currentIngredients;

    public event PurchaseIngredientDelegate OnAttemptPurchaseEvent;

    // Start is called before the first frame update
    void Start()
    {
        ingredientsMin = ingredientsMin < 0 ? ingredientsMax > 0 ? ingredientsMax : 0 : ingredientsMin;
        ingredientsMax = ingredientsMax < ingredientsMin ? ingredientsMin : ingredientsMax;
    }

    private int GetRandomIngredientIndex()
    {
        return Random.Range(0, ingredientDatabase.Length);
    }

    private void UpdateIngredientsText()
    {
        for (int i = 0; i < ingredientsText.Length; i++)
        {
            if (i < currentIngredients.Count)
            {
                string text = $"{i + 1}. {currentIngredients[i].ID} ({currentIngredients[i].FoodGroup}) [{currentIngredients[i].Cost.ToString("C2")}]";
                ingredientsText[i].text = text;
                ingredientsText[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                ingredientsText[i].text = string.Empty;
                ingredientsText[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    private void AssignRandomIngredients(int minIngredients, int maxIngredients)
    {
        if (ingredientDatabase.Length > 0)
        {
            currentIngredients = new List<Ingredient>();
            int ingredientCount = Random.Range(minIngredients, maxIngredients + 1);
            for (int i = 0; i < ingredientCount; i++)
            {
                if (i >= ingredientDatabase.Length)
                {
                    break;
                }
                int r = GetRandomIngredientIndex();
                while (currentIngredients.Contains(ingredientDatabase[r]) == true)
                {
                    r = GetRandomIngredientIndex();
                }
                currentIngredients.Add(ingredientDatabase[r]);
                Log(currentIngredients[i].ID);
            }
        }
    }

    public void CheckFridge()
    {
        AssignRandomIngredients(ingredientsMin, ingredientsMax);
        UpdateIngredientsText();
    }

    public void PurchaseIngredient(int index)
    {
        if (index < currentIngredients.Count)
        {
            if(OnAttemptPurchaseEvent?.Invoke(currentIngredients[index]) == true)
            {
                currentIngredients.RemoveAt(index);
                UpdateIngredientsText();
            }
        }
    }

    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[FRIDGE] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[FRIDGE] {message}");
                    break;
                case 2:
                    Debug.LogError($"[FRIDGE] {message}");
                    break;
            }
        }
    }
}
