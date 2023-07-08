using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PersistentCanvas : MonoBehaviour
{
    [SerializeField] private IngredientSlot[] inventorySlots;
    [SerializeField] private RectTransform scrollViewContent;

    public static UI_PersistentCanvas Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (IngredientSlot slot in inventorySlots)
        {
            slot.UpdateIcon(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddIngredientToInventory(Ingredient ingredient)
    {
        foreach(IngredientSlot slot in inventorySlots)
        {
            if(slot.gameObject.activeSelf == false)
            {
                slot.UpdateIcon(ingredient);
                return true;
            }
            else if(slot.CurrentIngredient == ingredient)
            {
                return false;
            }
        }
        return false;
    }
}
