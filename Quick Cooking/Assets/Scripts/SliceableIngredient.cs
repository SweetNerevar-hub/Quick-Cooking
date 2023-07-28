using UnityEngine;

/// <summary>
/// This class handles the functionality associated with the sliceable objects for each ingredient.
/// </summary>
public class SliceableIngredient : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;

    private SpriteRenderer[] sliceRenderers;    //reference to the different sprite renderer objects (children of this object)

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        sliceRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Sets the sprites for each slice renderer to the slice sprites associated with the passed ingredient.
    /// </summary>
    /// <param name="ingredient">The ingredient to be prepared.</param>
    /// <returns>Returns true.</returns>
    public bool Prepare(Ingredient ingredient)
    {
        for (int i = 0; i < sliceRenderers.Length; i++)
        {
            sliceRenderers[i].gameObject.SetActive(true);
            if (i == 0)
            {
                sliceRenderers[i].sprite = ingredient.FirstSliceSprite;
            }
            else if (i == sliceRenderers.Length - 1)
            {
                sliceRenderers[i].sprite = ingredient.LastSliceSprite;
            }
            else
            {
                sliceRenderers[i].sprite = ingredient.MiddleSlicesSprite;
            }
        }
        return true;
    }

    /// <summary>
    /// Disables the next active slice sprite renderer.
    /// </summary>
    /// <returns>Returns true if the last slice sprite renderer was disabled.</returns>
    public bool Slice()
    {
        for(int i = 0; i < sliceRenderers.Length; i++)
        {
            if (sliceRenderers[i].gameObject.activeSelf == true)
            {
                Log("INGREDIENT SLICED");
                sliceRenderers[i].gameObject.SetActive(false);
                return i == sliceRenderers.Length - 1;
            }
        }
        return false;
    }

    /// <summary>
    /// Logs the passed message to the console.
    /// </summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">Defines the alert level for the message. 0 = normal; 1 = warning; 2 = error.</param>
    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[SLICEABLE INGREDIENT] {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[SLICEABLE INGREDIENT] {message}");
                    break;
                case 2:
                    Debug.LogError($"[SLICEABLE INGREDIENT] {message}");
                    break;
            }
        }
    }
}
