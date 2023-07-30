using UnityEngine;
using UnityEngine.UI;

public class UIScrollingImage : MonoBehaviour
{
    [Tooltip("The speed of the image scrolling animation.")]
    [SerializeField] [Range(0, 1)] private float scrollSpeed = 1;
    [Tooltip("Enable to scroll the image on the X axis.")]
    [SerializeField] private bool scrollX;
    [Tooltip("Enable to scroll the image on the Y axis.")]
    [SerializeField] private bool scrollY;

    private Image image;    //reference to the image component to scroll

    /// <summary>
    /// Executed when the object first loads.
    /// </summary>
    private void Awake()
    {
        TryGetComponent(out image);
        if(image.material != null)
        {
            image.material.mainTextureOffset = Vector2.zero;
        }
    }

    /// <summary>
    /// Executes every frame.
    /// </summary>
    private void Update()
    {
        if (image.material != null)
        {
            Vector2 scrollVector = new Vector2(scrollX == true ? Time.deltaTime * scrollSpeed : 0, scrollY == true ? Time.deltaTime * scrollSpeed : 0);
            image.material.mainTextureOffset += scrollVector;
        }
    }
}
