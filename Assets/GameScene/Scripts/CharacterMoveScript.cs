using UnityEngine;
using System.Collections;

public class CharacterMoveScript : MonoBehaviour 
{
	public float Speed = 1f;
	public GameObject legs;

	private PhotonView photonView;
	private Animator animator;

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	
	void Awake() 
    {
		photonView = GetComponent<PhotonView>();
		animator = GetComponent<Animator> ();
    }

  	

    void FixedUpdate()
    {
        if( photonView.isMine == false )
        {
            return;
        }

		UpdateRotation ();
		InputMovement ();
		UpdateCamera (rigidbody2D.transform, Camera.main);
		UpdateIsMoving ();
		UpdatePunching ();
    }

	void UpdateRotation(){
		var mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Quaternion rot = Quaternion.LookRotation (transform.position - mousePosition, Vector3.forward);
		transform.rotation = rot;
		transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.z);
		rigidbody2D.angularVelocity = 0;

	}

	void UpdatePunching(){
		if (Input.GetMouseButtonDown(0)) {
			SetPunching();
			photonView.RPC( "SetPunching", PhotonTargets.Others);
		}
	}

	void SetPunching(){
		animator.SetTrigger("isPunching");	
	}

	void InputMovement()
	{
		if (Input.GetKey (KeyCode.W)) {
			rigidbody2D.transform.position += Vector3.up * Speed * Time.deltaTime;
			legs.transform.eulerAngles = new Vector3(0, 0, 0);
		}
			
		if (Input.GetKey (KeyCode.S)) {
			rigidbody2D.transform.position -= Vector3.up * Speed * Time.deltaTime;		
			legs.transform.eulerAngles = new Vector3(0, 0, 180);
		}

		if (Input.GetKey (KeyCode.D)) {
			rigidbody2D.transform.position += Vector3.right * Speed * Time.deltaTime;
			legs.transform.eulerAngles = new Vector3(0, 0, 270);
		}
		
		if (Input.GetKey (KeyCode.A)) {
			rigidbody2D.transform.position -= Vector3.right * Speed * Time.deltaTime;
			legs.transform.eulerAngles = new Vector3(0, 0, 90);
		}
	}

	void UpdateCamera(Transform target, Camera camera){
		camera.transform.position = Vector3.SmoothDamp(camera.transform.position, target.position + new Vector3(0f, 0f, -10f), ref velocity, dampTime);
	}
	
	void UpdateIsMoving(){
		bool isMoving = Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D); 
		SetMoving(isMoving);
		photonView.RPC( "SetMoving", PhotonTargets.Others, new object[]{isMoving});
	}
		
	[RPC]
	private void SetMoving(bool isMoving){
		animator.SetBool ("isMoving", isMoving);
		legs.GetComponent<Animator> ().SetBool ("isMoving", isMoving);
	}
}
