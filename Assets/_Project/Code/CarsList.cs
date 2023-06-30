using System.Collections.Generic;
using UnityEngine;

public class CarsList : MonoBehaviour
{
    [SerializeField]
    private List<Car> _cars;
    public IReadOnlyList<Car> Cars => _cars;
}