using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class UrlGalleryLoadImageService
{
    [SerializeField]
    private int _maxLoaded = 30;
    [SerializeField]
    private string urlFormat = "http://data.ikppbb.com/test-task-unity-data/pics/{0}.jpg";
   
    
    
    public void RequesImage(int index, Action<Sprite> callback)
    {
       // GetRemoteTextureAsync(string.Format(urlFormat, index));
    }

    
}

public interface IGalleryLoadImageRequest
{
    bool IsImageAvailable { get; }
    float LoadProgress { get; }
    GalleryLoadImageStatus Status { get; }
    Sprite Image { get; }
    void Repeat();
}
public class UrlGalleryLoadImageRequest : IGalleryLoadImageRequest
{
    private UnityWebRequest _webRequest;
    private Sprite _image;
    private string _url;
    private GalleryLoadImageStatus _status;

    public UrlGalleryLoadImageRequest(string url)
    {
        GetRemoteTextureAsync(url);
    }

    public float LoadProgress => _webRequest.downloadProgress;
    public GalleryLoadImageStatus Status => throw new NotImplementedException();
    public Sprite Image => throw new NotImplementedException();
    public bool IsImageAvailable => _image != null;

    public void Repeat()
    {
        GetRemoteTextureAsync(_url);
    }

    public async void GetRemoteTextureAsync(string url)
    {
        if (_webRequest != null)
            _webRequest.Dispose();
        using (_webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = _webRequest.SendWebRequest();

            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);

            if (_webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(_webRequest.error);
            }
            else
            {
                Texture2D texutre2d = DownloadHandlerTexture.GetContent(_webRequest);
                _image = Sprite.Create(texutre2d, new Rect(0.0f, 0.0f, texutre2d.width, texutre2d.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
        _webRequest = null;
    }
}
public enum GalleryLoadImageStatus : byte
{
    Sleep,
    InProgress,
    Error,
    Success,
}