using System;
using Rewired;
using UnityEngine;

[RequireComponent(typeof(CarInteraction))]
public class Car : MonoBehaviour
{
    private Rewired.Player _player;
    private Rigidbody2D _rigidBody;
    private float _acceleratorPedal;
    private float _brakePedal;
    private float _steeringWheel;

    public float EngineForce = 8391;
    public float Drag = 0.4f;
    public float BrakeForce = 1500;
    public float CarWheelBase = 3.4f;
    public float MinTurnInRadians = 0f;
    public float MaxTurnInRadians = 0.7f;
    [Range(0.0f, 1.0f)]
    public float WheelFrictionFactor = 0.7f;

    public bool IsHandbrakeOn;
    public Light LeftHeadLight;
    public Light RightHeadLight;
    public Light LeftBrakeLight;
    public Light RightBrakeLight;
    public GameObject FrontLeftTyre;
    public GameObject FrontRightTyre;

    // Use this for initialization
    public void Start ()
	{
	    _rigidBody = GetComponent<Rigidbody2D>();
        IsHandbrakeOn = true;
        LeftHeadLight.enabled = false;
        RightHeadLight.enabled = false;
        LeftBrakeLight.enabled = false;
        RightBrakeLight.enabled = false;
    }
	
	// Update is called once per frame
	public void Update ()
	{
	    GetInput();

        var backwards = Vector3.Dot(transform.up, _rigidBody.velocity) < 0;
        
        LeftHeadLight.enabled = _player != null;
        RightHeadLight.enabled = _player != null;

        LeftBrakeLight.enabled = _brakePedal > 0.0f;
	    LeftBrakeLight.color = backwards ? Color.white : Color.red;
        RightBrakeLight.enabled = _brakePedal > 0.0f;
        RightBrakeLight.color = backwards ? Color.white : Color.red;
    }

    public void FixedUpdate()
    {
        ProcessInput();
    }

    private void GetInput()
    {
        if (_player == null) return;

        _acceleratorPedal = _player.GetAxis("Accelerator");
        _brakePedal = _player.GetAxis("Brake");
        _steeringWheel = _player.GetAxis("Steer") * Math.Abs(_player.GetAxis("Steer"));
    }

    private void ProcessInput()
    {
        var backwards = Vector3.Dot(transform.up, _rigidBody.velocity) < 0;

        var newWheelFriction = _brakePedal > 0 ? 0.95f : WheelFrictionFactor;

        // wheel direction
        var turnAngle = Mathf.Lerp(MaxTurnInRadians, MinTurnInRadians, _rigidBody.velocity.magnitude / 100) * _steeringWheel;
        var wheelDirection = new Vector2(transform.up.x, transform.up.y).Rotate(turnAngle * Mathf.Rad2Deg);

        FrontLeftTyre.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -turnAngle * Mathf.Rad2Deg));
        FrontRightTyre.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -turnAngle * Mathf.Rad2Deg));

        // steering
        if (backwards) _steeringWheel = _steeringWheel * -1;
        var turningRadius = CarWheelBase / (float)Math.Sin(turnAngle);
        var rotation = _rigidBody.velocity.magnitude / turningRadius;
        _rigidBody.angularVelocity -= rotation / Time.fixedDeltaTime;

        // acceleration
        var traction = wheelDirection * EngineForce * _acceleratorPedal;
        _rigidBody.AddForce(traction, ForceMode2D.Force);

        // air resistance and rolling drag
        var drag = Drag * _rigidBody.velocity.magnitude * _rigidBody.velocity.magnitude;
        var rollingResistance = Drag* 30 * _rigidBody.velocity.magnitude;
        _rigidBody.drag = drag + rollingResistance;

        // braking
        var brakeForce = BrakeForce * _brakePedal;
        //_rigidBody.velocity -= _rigidBody.velocity*brakeForce;
        _rigidBody.AddForce(-_rigidBody.velocity * brakeForce, ForceMode2D.Force);

        // traction control
        Vector2 forwardVelocity = transform.up * Vector2.Dot(_rigidBody.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(_rigidBody.velocity, transform.right);
        _rigidBody.velocity = forwardVelocity + rightVelocity * newWheelFriction * newWheelFriction;

        // handbrake
        if (IsHandbrakeOn)
        {
            _rigidBody.drag = 10000;
        }
    }

    public void SetPlayer(int playerId)
    {
        _player = ReInput.players.GetPlayer(playerId);
        IsHandbrakeOn = false;
        SendMessage("ListenSetPlayer", playerId);
    }
}
