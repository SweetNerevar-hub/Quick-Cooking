using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuttingBoard : MonoBehaviour
{
    [SerializeField] private SliceableIngredient sliceableObject;
    [SerializeField] private float sliceTime = 0.5f;

    private Vector2 sliceStart = Vector2.zero;
    private Vector2 sliceEnd = Vector2.zero;
    private float sliceTimer = -1;

    public static CuttingBoard Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            sliceableObject.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool PutIngredientOnBoard(Ingredient ingredient)
    {
        if (sliceableObject.gameObject.activeSelf == false)
        {
            sliceableObject.gameObject.SetActive(true);
            sliceableObject.Prepare(ingredient);
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (sliceableObject.gameObject.activeSelf == true)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                sliceStart = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                sliceTimer = 0;
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && sliceTimer >= 0)
            {
                sliceTimer = -1;
                sliceEnd = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Debug.DrawLine(sliceStart, sliceEnd, Color.red, 3);
                //raycast here
                RaycastHit2D hit = Physics2D.Raycast(sliceStart, sliceEnd - sliceStart);
                if (hit.collider != null && hit.collider.gameObject == sliceableObject.gameObject)
                {
                    if(sliceableObject.Slice() == true)
                    {
                        //ingredient fully sliced
                        Debug.Log("FINISHED SLICING INGREDIENT");
                        sliceableObject.gameObject.SetActive(false);
                    }
                }
            }
        }

        if(sliceTimer >= 0)
        {
            sliceTimer += Time.deltaTime;
            if(sliceTimer > sliceTime)
            {
                sliceTimer = -1;
            }
        }
    }
}
