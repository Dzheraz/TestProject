using UnityEngine;
using Zenject;

public class CarHUD : MonoBehaviour
{
    [Inject]
    private CarSelectionService _carSelectionService;
    [Inject]
    private CarsList _carsList;

    private int _currenSelectedCarNumber = 0;

    [SerializeField]
    private ControllButton _upButton;
    [SerializeField]
    private ControllButton _downButton;
    [SerializeField]
    private ControllButton _leftButton;
    [SerializeField]
    private ControllButton _rightButton;


    [SerializeField]
    private ControllButton _switchCarButton;

    private Car _targetCar;

    private void OnEnable()
    {
        _targetCar = _carSelectionService.Car;
        _carSelectionService.selected += OnCarSelected;
        _upButton.up += OnUpButtonDoUp;
        _upButton.down += OnUpButtonDoDown;

        _downButton.up += OnDownButtonDoUp;
        _downButton.down += OnDownButtonDoDown;

        _switchCarButton.up += OnSwitchCarButtonDoDown;
    }

    private void OnSwitchCarButtonDoDown()
    {
        var cars = _carsList.Cars;
        if (++_currenSelectedCarNumber >= cars.Count)
            _currenSelectedCarNumber = 0;

        _carSelectionService.SelectCar(cars[_currenSelectedCarNumber]);
    }

    private void OnDisable()
    {
        _carSelectionService.selected -= OnCarSelected;
        _upButton.up -= OnUpButtonDoUp;
        _upButton.down -= OnUpButtonDoDown;

        _downButton.up -= OnDownButtonDoUp;
        _downButton.down -= OnDownButtonDoDown;

        _switchCarButton.up -= OnSwitchCarButtonDoDown;
    }
    private void Update()
    {
        float streetAxis = 0;
        if (_leftButton.IsPressed)
            streetAxis += -1f;
        if (_rightButton.IsPressed)
            streetAxis += 1f;
        if (streetAxis != 0f)
            _targetCar.Steer(streetAxis);
    }

    private void OnCarSelected(Car car) => _targetCar = car;


    private void OnDownButtonDoUp()
    {
        _targetCar.StopMove();
    }
    private void OnDownButtonDoDown()
    {
        _targetCar.StartMoveBackward();
    }

    private void OnUpButtonDoUp()
    {
        _targetCar.StopMove();
    }
    private void OnUpButtonDoDown()
    {
        _targetCar.StartMoveForward();
    }
}
