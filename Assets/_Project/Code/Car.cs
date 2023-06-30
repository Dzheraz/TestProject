using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    private float _steeringSpeed = 0.1f;
    [SerializeField]
    private float _motorForwardSpeed = 10;
    [SerializeField]
    private float _motorBackwardSpeed = 1;

    [SerializeField]
    private float _maxSteeringAngle = 60f;

    [SerializeField]
    private WheelCollider _frontLeftWheel;
    [SerializeField]
    private WheelCollider _frontRightWheel;
    [SerializeField]
    private WheelCollider _rearLeftWheel;
    [SerializeField]
    private WheelCollider _rearRightWheel;

    private MoveState _moveState = MoveState.Stop;
    private bool _isSteering = false;
    private float _currentSteeringAngle = 0;

    private void Update()
    {
        float motor = 0f;

        switch (_moveState)
        {
            case MoveState.Forward:
                motor = _motorForwardSpeed;
                break;
            case MoveState.Backward:
                motor = -_motorBackwardSpeed;
                break;
            default:
                break;
        }

        if (!_isSteering)
            ApplySteer(0f);
        _isSteering = false;

        ApplyMotor(_rearLeftWheel, motor);
        ApplyMotor(_rearRightWheel, motor);

        ApplySteer(_frontLeftWheel, _currentSteeringAngle);
        ApplySteer(_frontRightWheel, _currentSteeringAngle);

        ApplyView(_rearLeftWheel);
        ApplyView(_rearRightWheel);
        ApplyView(_frontLeftWheel);
        ApplyView(_frontRightWheel);
    }

    #region Constrolls

    private void ApplySteer(float angle)
    {
        _currentSteeringAngle = Mathf.MoveTowards(_currentSteeringAngle, angle, _steeringSpeed * Time.deltaTime);
    }

    public void Steer(float axis)
    {
        _isSteering = true;
        ApplySteer(axis * _maxSteeringAngle);
    }
    public void StartMoveForward()
    {
        _moveState = MoveState.Forward;
    }
    public void StartMoveBackward()
    {
        if (_moveState != MoveState.Stop)
            return;
        _moveState = MoveState.Backward;
    }
    public void StopMove()
    {
        _moveState = MoveState.Stop;
    }
    #endregion

    public void ApplyMotor(WheelCollider wheel, float motor)
    {
        wheel.motorTorque = motor;
    }
    public void ApplySteer(WheelCollider wheel, float steering)
    {
        wheel.steerAngle = steering;
    }
    private void ApplyView(WheelCollider wheel)
    {
        if (wheel.transform.childCount == 0)
            return;
        Transform view = wheel.transform.GetChild(0);
        wheel.GetWorldPose(out Vector3 position, out Quaternion rotation);
        view.SetPositionAndRotation(position, rotation);
        //view.transform.position = position;
        //view.transform.rotation = rotation;
    }

    private enum MoveState
    {
        Stop,
        Forward,
        Backward,
    }
}
