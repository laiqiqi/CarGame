using Rewired;
using UnityEngine;

[RequireComponent(typeof(Car))]
public class PoliceCar : MonoBehaviour
{
    public Light LeftSirenLight;
    public Light RightSirenLight;

    public Color Red;
    public Color Blue;

    public AudioSource Siren;

    public float LightFlashInterval = 1;

    private Rewired.Player _player;
    private float _timerCountdown;
    private bool _isSirenOn;

    // Use this for initialization
	public void Start ()
	{
        LeftSirenLight.enabled = false;
        RightSirenLight.enabled = false;

        _timerCountdown = LightFlashInterval;
	}

    // Update is called once per frame
    public void Update()
    {
        GetInput();
    }

    public void FixedUpdate()
    {
        UpdateSiren();
    }

    public void ListenSetPlayer(int playerId)
    {
        _player = ReInput.players.GetPlayer(playerId);
    }

    private void GetInput()
    {
        if (_player == null) return;

        if (_player.GetButtonDown("Toggle Car Special")) ToggleSiren();
    }

    private void ToggleSiren()
    {
        if (_isSirenOn)
        {
            Siren.Stop();
            LeftSirenLight.enabled = false;
            RightSirenLight.enabled = false;
            _isSirenOn = false;
        }
        else
        {
            Siren.Play();

            LeftSirenLight.enabled = true;
            RightSirenLight.enabled = true;

            LeftSirenLight.color = Red;
            RightSirenLight.color = Blue;

            _timerCountdown = LightFlashInterval;

            _isSirenOn = true;
        }
    }

    private void UpdateSiren()
    {
        if (_isSirenOn)
        {
            _timerCountdown -= Time.deltaTime;

            if (_timerCountdown <= 0)
            {
                var tempColor = LeftSirenLight.color;

                LeftSirenLight.color = RightSirenLight.color;
                RightSirenLight.color = tempColor;

                _timerCountdown = LightFlashInterval;
            }
        }
    }
}
