using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform Target;
    public float ZOffset;
    
    // Update is called once per frame
    public void Update ()
	{
	    transform.position = Target.position + new Vector3(0, 0, ZOffset);
	}
}
