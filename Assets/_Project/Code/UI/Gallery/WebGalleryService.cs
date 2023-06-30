using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
internal class WebGalleryService
{
    [SerializeField]
    private string _usrFormat = "http://data.ikppbb.com/test-task-unity-data/pics/{0}.jpg";

    private List<Sprite> _sprites = new List<Sprite>();

    public void RequestImage(int index, Action<Sprite> onReturn)
    {
        index++;
        if (_sprites.Count <= index)
            _sprites.AddRange(Enumerable.Repeat<Sprite>(null, index + 1));
        if (_sprites[index] != null)
        {
            onReturn(_sprites[index]);
            return;
        }
        RequestImageInternalAsync(index, onReturn);
    }
    private async void RequestImageInternalAsync(int index, Action<Sprite> onReturn)
    {
        string url = string.Format(_usrFormat, index);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = request.SendWebRequest();

            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(url + " " + request.error);
            }
            else
            {
                Texture2D texutre2d = DownloadHandlerTexture.GetContent(request);
                Sprite sprote = Sprite.Create(texutre2d, new Rect(0.0f, 0.0f, texutre2d.width, texutre2d.height), new Vector2(0.5f, 0.5f), 100.0f);
                _sprites[index] = sprote;
                onReturn(sprote);
            }
        }
    }
}
