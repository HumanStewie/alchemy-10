using UnityEngine;
using UnityEngine.UI;

public class UVScroller : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.1f, 0.05f);
    private RawImage _rawImage;

    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        Rect currentRect = _rawImage.uvRect;
        currentRect.position += scrollSpeed * Time.deltaTime;
        _rawImage.uvRect = currentRect;
    }
}
