using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    private Car _car;

    // Use this for initialization
	void Start ()
	{
	    _car = GetComponent<Car>();
	}
	
    public void Execute(Interactor interactor)
    {
        var player = interactor.GetComponent<Player>();
        _car.SetPlayer(player.PlayerId);

        // point camera at car
        var cameraFollowPath = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowPosition>();
        cameraFollowPath.Target = _car.transform;

        // destroy player guy
        Destroy(player.gameObject);
    }
}
