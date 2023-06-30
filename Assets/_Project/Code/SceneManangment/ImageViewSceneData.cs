using UnityEngine;

internal class ImageViewSceneData
{
    private Sprite _image;
    public Sprite Image => _image;
    public void SetImage(Sprite image)
    {
        _image = image;
    }
}
