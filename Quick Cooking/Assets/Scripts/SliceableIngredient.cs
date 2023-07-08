using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceableIngredient : MonoBehaviour
{
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
                Debug.Log("INGREDIENT SLICED");
                sliceRenderers[i].gameObject.SetActive(false);
                return i == sliceRenderers.Length - 1;
            }
        }
        return false;
    }
}
