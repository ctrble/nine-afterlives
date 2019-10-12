using UnityEngine;
using System.Collections;

public class PlayerControls : MonoBehaviour {
	
	//variables
	public float currentSpeed = 6F;
	public float crouchSpeed = 3F;
	public float runSpeed = 6F;
	public float jumpSpeed = 3F;
	public float wallBounce = 2F;
	public float rotateSpeed = 3F;
	public float gravity = 20F;

	private bool doubleJumped = false;
	private bool canWallJump = false;

	private Vector3 moveDirection = Vector3.zero;
	private Vector3 hitWall;
	
	void Start() {

	}	
	
	void Update() {
		
		CharacterController controller = GetComponent<CharacterController> ();

		//gravity
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
		
		//current direction
		transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed, 0);
		KeepUpright();

		//moving around
		if(controller.isGrounded) {
			Move();
			SetPace();
		}
		
		//allow for single jump
		if(controller.isGrounded==true && doubleJumped==false && Input.GetButtonDown("Jump")) {
			Jump();
		}
		
		//allow for double jump
		if(controller.isGrounded==false && doubleJumped==false && Input.GetButtonDown("Jump")) {
			DoubleJump();
		}

		//wall slide
		if (controller.collisionFlags == CollisionFlags.Sides) { //only making contact with the sides of the player, not the top or bottom
			WallSlide ();
		}

		//set up playerGrounded so that other functions can know if the player is on the ground or not
		if(controller.isGrounded==false) {
			SetPace(); //to ensure steady pace after a jump
		}
		
		//remembers if already double jumped or wall jumped and resets once player is back on the ground
		if(controller.isGrounded==true) {
			doubleJumped = false;
			canWallJump = false;
			Debug.Log(currentSpeed);
		}
	}
	
	void Move() {
		//WASD
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= currentSpeed;
	}
	
	void SetPace() {
		//crouch while c is held
		if(Input.GetKeyDown(KeyCode.C)) {
			//need to add a crouch animation/function
			currentSpeed = currentSpeed - crouchSpeed;
		}
		if(Input.GetKeyUp(KeyCode.C)) {
			//need to add a crouch animation/function
			currentSpeed = currentSpeed + crouchSpeed;
		}
		
		//run while shift is held
		if(Input.GetKeyDown(KeyCode.LeftShift)) {
			//need to add a run animation/function
			currentSpeed = currentSpeed + runSpeed;
		}
		if(Input.GetKeyUp(KeyCode.LeftShift)) {
			//need to add a run animation/function
			currentSpeed = currentSpeed - runSpeed;
		}
	}

	void KeepUpright() {
		//straightens player if tilted
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		eulerAngles = new Vector3(0, eulerAngles.y, 0);
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(eulerAngles), Time.deltaTime * currentSpeed); //adjusted by player speed
	}
	
	void Jump() {
		//jumps vertically according to player speed
		moveDirection.y = currentSpeed + jumpSpeed;
	}
	
	void DoubleJump() {
		//double jumps vertically according to player speed
		moveDirection.y = currentSpeed + jumpSpeed;
		doubleJumped = true;
	}

	void WallSlide() {
		//rotates away from the wall
		transform.rotation = Quaternion.FromToRotation (Vector3.forward, hitWall); 
		canWallJump = true;
		
		//wall jump
		if (canWallJump == true && Input.GetButtonDown ("Jump")) {
			WallJump ();
		}
	}
		
	void WallJump() {
		//bigger jump off walls only
		moveDirection = Vector3.Reflect(moveDirection, hitWall);
		moveDirection.y = currentSpeed + jumpSpeed;

		//resets by touching another wall
		canWallJump = false;
		doubleJumped = false;
	}

	//remember where the player hit the wall for wall jumps
	void OnControllerColliderHit(ControllerColliderHit hit) {
		hitWall = hit.normal;
		Debug.Log ("hit wall");
	}
		
//end
}