using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private List<Collider2D> _colliders;

    // Use this for initialization
	public void Start ()
	{
	    _colliders = new List<Collider2D>();
	}

    // Update is called once per frame
    public void Update () {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        _colliders.Add(other);
        Debug.Log(other);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        _colliders.Remove(other);
    }

    public void TryEnterVehicle()
    {
        var firstVehicle = _colliders.FirstOrDefault(x => x.gameObject.tag == "Vehicle");
        if (firstVehicle == null) return;

        var carInteraction = firstVehicle.GetComponent<CarInteraction>();
        carInteraction.Execute(this);
    }
}
