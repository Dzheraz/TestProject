using UnityEngine;

public class CarSelectionService : MonoBehaviour
{
    [SerializeField]
    private Car _car;
    public Car Car => _car;

    public void SelectCar(Car car)
    {
        _car = car;
        selected(car);
    }

    public delegate void CarSelectionCallback(Car car);
    public event CarSelectionCallback selected = delegate { };
}
