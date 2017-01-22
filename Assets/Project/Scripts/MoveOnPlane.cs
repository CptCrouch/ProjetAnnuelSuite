using UnityEngine;
using System.Collections;

public class MoveOnPlane : MonoBehaviour {

    public float speedRotate = 5f;
    private float speed;
    public float speedRun = 10;
    public KeyCode activeGravity = KeyCode.LeftShift;
    private CharacterController characController;

    public float minPosZ;
    public float maxPosZ;

	// Use this for initialization
	void Start () {
        characController = GetComponent<CharacterController>();
        speed = speedRotate;
	}
	
	// Update is called once per frame
	void Update () {
        float axisX = Input.GetAxis("Horizontal");
        float axisZ = Input.GetAxis("Vertical");
        float axisY = Input.GetAxis("UpDown");

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(transform.position.y, 0.5f, 1000f);
        //pos.z = Mathf.Clamp(transform.position.z, minPosZ, maxPosZ);
        transform.position = pos;
        
            transform.Translate(new Vector3(0, 0, axisZ) * speed * Time.deltaTime);


        transform.RotateAround(Vector3.zero, Vector3.up, axisX * Time.deltaTime*speed);
        
        if(Input.GetKeyDown(activeGravity))
        {
            if(characController.enabled == false)
                characController.enabled = true;
            else
                characController.enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            
            speed = speedRun;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {

            speed = speedRotate;
        }



    }
}
