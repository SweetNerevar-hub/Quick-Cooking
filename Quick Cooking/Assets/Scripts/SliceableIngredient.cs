using UnityEngine;

public class SliceableIngredient : MonoBehaviour, ILoggable
{
    [Tooltip("Enable to print debug messages to the console.")]
    [SerializeField] private bool debug = false;

    private SpriteRenderer[] sliceRenderers;

    private void Awake()
    {
        sliceRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

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
