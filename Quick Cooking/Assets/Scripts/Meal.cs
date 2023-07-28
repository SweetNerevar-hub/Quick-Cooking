using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class manages the data and functionality associated with the final scene of the game loop (eating the meal).
/// </summary>
public class Meal : MonoBehaviour
{
    [Tooltip("A reference to the text object that displays reward messages.")]
    [SerializeField] private Text rewardMessageText;
    [Tooltip("The range of reward messages that will be displayed to the player.")]
    [SerializeField] [TextArea] private string[] rewardMessages;

    private AudioSource aSrc;   //reference to this object's audio source

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        foreach (IngredientPiece piece in GameState.IngredientPieces)
        {
            if (piece.TryGetComponent(out Rigidbody2D rb) == true)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
        TryGetComponent(out aSrc);
    }

    /// <summary>
    /// Executes on the first frame of the game.
    /// </summary>
    private void Start()
    {
        rewardMessageText.transform.root.gameObject.SetActive(false);
    }

    /// <summary>
    /// Executes every frame.
    /// </summary>
    private void Update()
    {
        if (GameState.IngredientPieces.Count > 0 && Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)    //touch detected
            {
                Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
                if (hit != null)
                {
                    if (hit.TryGetComponent(out IngredientPiece piece) == true)
                    {
                        piece.Eat();
                        if (GameState.IngredientPieces.Count == 0)  //all ingredient pieces eaten
                        {
                            aSrc.Play();   //originally written by Cameron Moore, updated by Josh Ferguson
                            rewardMessageText.text = rewardMessages[Random.Range(0, rewardMessages.Length)];    //display random reward message
                            rewardMessageText.transform.root.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Call this method to finish the meal and move to next game round.
    /// </summary>
    public void ContinueGame()
    {
        rewardMessageText.transform.root.gameObject.SetActive(false);
        UI_PersistentCanvas.Instance.FinishedMeal();
    }
}
