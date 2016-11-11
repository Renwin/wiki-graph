
using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Move ZeroG")]
public class CameraControlZeroG : MonoBehaviour {

	public float speed = 5f;
	//public GUIText movementSpeed;

	private Vector3 move = new Vector3();
	private Vector3 pos1 = new Vector3(0, 0, -10);


	void Start(){
		//set to first cluster position
		
	}
	
	void Update () {
		move.x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		move.z = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		move.y = 0;
		if (Input.GetKey ("space")) {
			move.y = speed * Time.deltaTime;
		}

		if (Input.GetKey ("left ctrl")) {
			move.y = -speed * Time.deltaTime;
		}

		//adjust speed with mouse wheel
		speed += Input.GetAxis("Mouse ScrollWheel");
		if (speed < 5)
			speed = 5;

		//movementSpeed.text = "Move Speed: " + speed;

		move = transform.TransformDirection(move);
		transform.position += move;

		//set warp to cluster controls
		if(Input.GetKey("1")){
			transform.position = pos1;
		}
	}
}
