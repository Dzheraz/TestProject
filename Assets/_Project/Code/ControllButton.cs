using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isPressed = false;
    public bool IsPressed => _isPressed;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        down();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        up();
    }

    public event Action up = delegate { };
    public event Action down = delegate { };
}
