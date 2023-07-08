using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    [SerializeField] private bool isInventoryIcon = false;

    private Image image;

    public Ingredient CurrentIngredient { get; private set; }

    private void Awake()
    {
        TryGetComponent(out image);
    }

    public void UpdateIcon(Ingredient ingredient)
    {
        CurrentIngredient = ingredient;
        if (CurrentIngredient == null)
        {
            image.sprite = null;
            gameObject.SetActive(false);
            return;
        }
        image.sprite = ingredient.Sprite;
        if(gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
    }

    public void Select()
    {
        if (isInventoryIcon == false)
        {
            if (UI_PersistentCanvas.Instance.AddIngredientToInventory(CurrentIngredient) == true)
            {
                UpdateIcon(null);
            }
        }
        else if(CurrentIngredient != null)
        {
            if (Fridge.Instance.PutIngredientBack(CurrentIngredient) == true)
            {
                UpdateIcon(null);
            }
        }
    }
}
