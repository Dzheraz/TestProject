﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

internal class WebGalleryGrid : MonoBehaviour
{
    [ContextMenu("Set Refs")]
    private void setRefs()
    {
        _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>(true);
        _images = GetComponentsInChildren<ImagePreview>(true);
    }

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;
    [SerializeField]
    private ImagePreview[] _images;
    [SerializeField]
    private int columns = 2;


    private float _cellSize;

    public ImagePreview GetImage(int index)
    {
        return _images[index];
    }

    private async void OnEnable()
    {
        await Task.Delay(1);
        UpdateCellSize();
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].SetIndex(i);
        }
    }

    private void UpdateCellSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        _cellSize = rectTransform.rect.width / columns;
        _gridLayoutGroup.cellSize = Vector2.one * _cellSize;
    }
}
