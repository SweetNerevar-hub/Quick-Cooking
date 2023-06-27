using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingMethod : MonoBehaviour, ILoggable
{
    public delegate void CookingCompleteDelegate(float outcome);
    public delegate void SelectCookingMethodDelegate(CookingMethod cookingMethod);

    [SerializeField] private bool debug = false;
    [SerializeField] private CookingMethodType method;
    [SerializeField] private Image timeImage;

    private float cookingTimer = -1;
    private float cookingOutcome = 0;
    private Meal currentMeal;

    public static event CookingCompleteDelegate CookingCompleteEvent;
    public static event SelectCookingMethodDelegate SelectCookingMethodEvent;

    public void Cook(Meal meal)
    {
        if(meal.CookingMethod != method) { return; }
        currentMeal = meal;
        cookingTimer = 0;
        timeImage.gameObject.SetActive(true);
        Log($"Cooking {currentMeal.ID}");
    }

    private void Start()
    {
        timeImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (cookingTimer >= 0)
        {
            cookingTimer += Time.deltaTime;
            timeImage.fillAmount = cookingTimer / currentMeal.CookingTime;
            if (cookingTimer > currentMeal.CookingTime)
            {
                cookingTimer = -1;
                cookingOutcome = currentMeal.NutritionalValue / 100; //build an algorithm that generates a signed outcome
                CookingCompleteEvent?.Invoke(cookingOutcome);
                Log($"Finished cooking {currentMeal.ID} with an outcome of {cookingOutcome}");
                currentMeal = null;
                timeImage.gameObject.SetActive(false);
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
                    Debug.Log($"[{method.ToString().ToUpper()}] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[{method.ToString().ToUpper()}] {message}");
                    break;
                case 2:
                    Debug.LogError($"[{method.ToString().ToUpper()}] {message}");
                    break;
            }
        }
    }

    private void OnMouseDown()
    {
        if (currentMeal == null)
        {
            SelectCookingMethodEvent?.Invoke(this);
            Log("Invoking method selection");
        }
    }
}

public enum PreparationType { wash = 0, peel = 1, dice = 2, slice = 3, spread = 4 }
public enum CookingMethodType { microwave, oven, stove, grill, kettle }
