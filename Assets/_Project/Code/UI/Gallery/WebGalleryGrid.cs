using DCFApixels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

internal class WebGalleryGrid : MonoBehaviour
{

    private void setRefs()
    {
        _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>(true);
        _images = GetComponentsInChildren<Image>(true);
    }

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;
    [SerializeField]
    private Image[] _images;

    private int columns = 2;

    private bool _isVerticalOrientation;

    public Image GetImage(int index)
    {
        return _images[index];
    }

    private void OnEnable()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        ApplyGrid();
    }

    private void ApplyGrid()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float cellSize = rectTransform.rect.width / columns;
        _gridLayoutGroup.cellSize = Vector2.one * cellSize;
    }
}
