using System.Threading.Tasks;
using UnityEngine;

public class WebGalleryScreen : MonoBehaviour
{
    [SerializeField]
    private WebGalleryGrid _grid;

    private async void OnEnable()
    {
        await Task.Delay(1);
        RectTransform rectTransform = GetComponent<RectTransform>();
        _grid.SetWidth(rectTransform.rect.width);
    }
}
