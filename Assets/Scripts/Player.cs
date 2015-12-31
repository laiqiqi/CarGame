using System;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(Interactor))]
public class Player : MonoBehaviour
{
    private const float InputTolerance = 0.01f;
    private Rewired.Player _player;
    private Animator _animator;
    private Interactor _interactor;
    private Rigidbody2D _rigidBody;
    private Vector2 _turnVector;
    private Vector2 _moveVector;
    private Vector2 _movement;

    public int PlayerId = 0;
    public float MovementSpeed = 65;
    public float RunningFactor = 2f;
    public float AimingFactor = 0.4f;

    public void Awake()
    {
        _player = ReInput.players.GetPlayer(PlayerId);
    }

    // Use this for initialization
    public void Start ()
    {
        _animator = GetComponent<Animator>();
        _interactor = GetComponent<Interactor>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void Update()
    {
        GetController();
        GetInput();
        ProcessInput();
    }

    public void FixedUpdate()
    {
        _rigidBody.velocity = _movement;
    }

    private void GetController()
    {
        foreach(var controller in ReInput.controllers.Controllers)
        {
            if (controller.GetAnyButtonDown())
            {
                Debug.Log(controller.hardwareName);
                _player.controllers.ClearAllControllers();
                _player.controllers.AddController(controller, true);
                return;
            }
        }
    }

    private void GetInput()
    {
        _turnVector.x = _player.GetAxis("Turn Horizontal");
        _turnVector.y = _player.GetAxis("Turn Vertical");

        _moveVector.x = _player.GetAxis("Move Horizontal");
        _moveVector.y = _player.GetAxis("Move Vertical");
    }

    private void ProcessInput()
    {
        _movement = Vector2.zero;
        
        //process aiming
        var isAiming = _player.GetButton("Aim");
        _animator.SetBool("IsAiming", isAiming);

        // Process move
        if (Math.Abs(_moveVector.x) > InputTolerance || Math.Abs(_moveVector.y) > InputTolerance)
        {
            var isRunning = !isAiming && _player.GetButton("Run");
            _animator.SetBool("IsRunning", isRunning);

            var movementFactor = 1.0f;
            if (isRunning) movementFactor = RunningFactor;
            if (isAiming) movementFactor = AimingFactor;
            _movement = _moveVector * Time.fixedDeltaTime * MovementSpeed * movementFactor;
        }

        // Process turn
        if (isAiming && (Math.Abs(_turnVector.x) > InputTolerance || Math.Abs(_turnVector.y) > InputTolerance))
        {
            var angle = Mathf.Atan2(_turnVector.x, _turnVector.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.Rotate(0, 0, -90);
        }
        else
        {
            var angle = Mathf.Atan2(_moveVector.y, _moveVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        _animator.SetFloat("MovementScalar", _moveVector.magnitude);

        //process Enter Vehicle
        var isEnterVehicle = _player.GetButtonDown("Enter Vehicle");
        if (isEnterVehicle)
        {
            _interactor.TryEnterVehicle();
        }
    }
}
