using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    public delegate bool IngredientSlotDelegate(IngredientSlot ingredient);

    [Tooltip("Set to true if this ingredient slot is associated with the inventory bar.")]
    [SerializeField] private bool isInventoryIcon = false;

    private Image image;

    public bool IsInventoryIcon { get { return isInventoryIcon; } }
    public Ingredient CurrentIngredient { get; private set; }

    public static event IngredientSlotDelegate OnIngredientSelected;

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
        image.sprite = ingredient.FridgeSprite;
        if(gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }
    }

    public void Select()
    {
        if (OnIngredientSelected?.Invoke(this) == true)
        {
            UpdateIcon(null);
        }
    }
}
