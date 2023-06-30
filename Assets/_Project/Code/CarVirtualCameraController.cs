using Cinemachine;
using UnityEngine;
using Zenject;

public class CarVirtualCameraController : MonoBehaviour
{
    [Inject]
    private CarSelectionService _carSelectionService;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    private void OnEnable()
    {
        _carSelectionService.selected += OnCarSelected;
        Apply(_carSelectionService.Car.transform);
    }

    private void OnDisable()
    {
        _carSelectionService.selected -= OnCarSelected;
    }

    private void Apply(Transform target)
    {
        _virtualCamera.Follow = target;
        _virtualCamera.LookAt = target;
    }

    private void OnCarSelected(Car car)
    {
        Apply(car.transform);
    }
}
