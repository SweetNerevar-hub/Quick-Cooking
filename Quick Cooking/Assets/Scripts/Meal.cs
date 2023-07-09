using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meal : MonoBehaviour
{
    [SerializeField] private Text rewardMessageText;
    [SerializeField] [TextArea] private string[] rewardMessages;

    private void Awake()
    {
        foreach (IngredientPiece piece in GameState.IngredientPieces)
        {
            if (piece.TryGetComponent(out Rigidbody2D rb) == true)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rewardMessageText.transform.root.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.IngredientPieces.Count > 0 && Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
                if (hit != null)
                {
                    if (hit.TryGetComponent(out IngredientPiece piece) == true)
                    {
                        piece.Eat();
                        if (GameState.IngredientPieces.Count == 0)
                        {
                            //GAME OVER
                            rewardMessageText.text = rewardMessages[Random.Range(0, rewardMessages.Length)];
                            rewardMessageText.transform.root.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void ContinueGame()
    {
        rewardMessageText.transform.root.gameObject.SetActive(false);
        UI_PersistentCanvas.Instance.FinishedMeal();
    }
}
