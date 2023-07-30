using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles the data and functionality associated with a UI ingredient slot.
/// </summary>
public class IngredientSlot : MonoBehaviour
{
    /// <summary>
    /// Generic ingredient slot delegate that.
    /// </summary>
    public delegate bool IngredientSlotDelegate(IngredientSlot ingredient);

    [Tooltip("Set to true if this ingredient slot is associated with the inventory bar.")]
    [SerializeField] private bool isInventoryIcon = false;

    private Image image;    //reference to the image component for this object

    /// <summary>
    /// Returns true if this ingredient slot is associated with the inventory UI.
    /// </summary>
    public bool IsInventoryIcon { get { return isInventoryIcon; } }
    /// <summary>
    /// The ingredient currently associated with this ingredient slot.
    /// </summary>
    public Ingredient CurrentIngredient { get; private set; }

    /// <summary>
    /// Invoked when an ingredient slot is selected/interacted with.
    /// </summary>
    public static event IngredientSlotDelegate OnIngredientSelected;

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        TryGetComponent(out image);
    }

    /// <summary>
    /// Updates the ingredient associated with the ingredient slot.
    /// Disables the ingredient slot when passed a null value.
    /// </summary>
    /// <param name="ingredient">The ingredient to update the slot to match.</param>
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

    /// <summary>
    /// Called when the slot is interacted with.
    /// </summary>
    public void Select()
    {
        if (OnIngredientSelected?.Invoke(this) == true)
        {
            UpdateIcon(null);
        }
    }
}
