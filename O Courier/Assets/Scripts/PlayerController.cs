using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	[Header("Movement")]
	public float walkSpeed = 2;
	public float runSpeed = 6;
    public float ClimbSpeed = 2;
	public float gravity = -12;
	public float jumpHeight = 1;
	[Range(0,1)]
	public float airControlPercent;

	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;

	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
	float velocityY;
	[HideInInspector]public bool isJumping;

	Animator animator;
	Transform cameraT;
	CharacterController controller;
	public float NormalChHeight;
	public float SlideChHeight;

	[Header("Parkour")]
	public float DistIndicator;
	public Transform hips;
	bool vaultFwd;
	Vector3 vaultPos;
	Vector3 vaultNor;
	public Vector2 vaultMinMax;
	public Vector2 vaultFix;
	public Transform vaultMarker;
	public LayerMask vaultLayer;
	[Space]
	bool ClimbFwd;
	public Vector2 ClimbMinMax;
	public Vector2 ClimbFix;
	Vector3 climbPos;
	Vector3 climbNor;

    public bool Climbing = false;

    public bool Zipline = false;
    
    public Transform EndZipline;
    public Transform StartZipline;


	void Start () {
		animator = GetComponent<Animator> ();
		cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController> ();
	}

	void Update () {

        // input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir, running);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FrontTwistLand") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Vault") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Climb") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            animator.applyRootMotion = false;
        }
        else
        {
            animator.applyRootMotion = true;
        }
        controller.enabled = (!animator.applyRootMotion || animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        ParkourVoid();
        // animator
        if (!freeze)
        {
            float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
            animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        }
        
        if (Climbing == true || Zipline == true)
        {
            gravity = 0;
        }

        else if (Climbing == false || Zipline == false)
        {
            gravity = -12;
        }

        if (Zipline == true)
        {
            UseZipline();
        }

    }

	bool freeze;

	void Move(Vector2 inputDir, bool running) {

        if (Climbing == false && Zipline == false)
        {
            float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

            if (Input.GetKeyDown(KeyCode.E) && !animator.applyRootMotion && running && animator.GetFloat("speedPercent") > 0.5f)
            {
                animator.CrossFade("Slide", 0.1f);
            }

            velocityY += Time.deltaTime * gravity;
            Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FrontTwistLand") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Vault") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Climb") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Stand") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                freeze = false;
            }
            else
            {
                freeze = true;
            }
            if (!freeze)
            {
                controller.Move(velocity * Time.deltaTime);
                if (inputDir != Vector2.zero)
                {
                    float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
                }
            }
            else
            {
                animator.SetFloat("speedPercent", 0f);
            }
            currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
            {
                controller.height = SlideChHeight;
                controller.center = new Vector3(0f, SlideChHeight / 2, 0f);
            }
            else
            {
                controller.height = NormalChHeight;
                controller.center = new Vector3(0f, NormalChHeight / 2, 0f);
            }
            if (controller.isGrounded)
            {
                velocityY = 0;
            }

            if (isJumping && controller.isGrounded)
            {
                isJumping = false;
                animator.CrossFade("FrontTwistLand", 0.05f);
            }
        }

		if (Climbing == true)
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * ClimbSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * ClimbSpeed);
            }
        }
        
	}

	void Jump() {
		if (controller.isGrounded && !vaultable && !climbable && !animator.applyRootMotion) {
			float jumpVelocity = Mathf.Sqrt (-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
			animator.CrossFade ("FrontTwist", 0.02f);
			isJumping = true;
		}
	}

	Vector3 vaultTarget;
	bool vaultable;
	bool climbable;
	private void ParkourVoid(){
		RaycastHit raycastHit;
		Vector3 vector = base.transform.forward * 2f;
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Vault")) {
			transform.position += transform.forward * 1f * Time.deltaTime;
		}
		Vector3 vector2 = (hips.position + hips.transform.forward * -0.35f) - base.transform.up + transform.up * 0.3f;
		if (Physics.Raycast (vector2, vector, out raycastHit, 2f, vaultLayer)) {
		}
		Vector3 vector3 = raycastHit.point + base.transform.up * 15f + base.transform.forward * 0.02f;
		Vector3 vector4 = Vector3.down * 6.5f;
		RaycastHit raycastHit2;
		if (Physics.Raycast (vector3, vector4, out raycastHit2, 14.5f, vaultLayer)) {
			vaultMarker.gameObject.SetActive (true);
			vaultMarker.position = raycastHit2.point;
			vaultMarker.rotation = Quaternion.LookRotation (raycastHit2.normal);
		} else {
			vaultMarker.gameObject.SetActive (false);
		}
		float nums = Vector3.Distance (vector3, raycastHit2.point);
		DistIndicator = nums;
		if (nums > vaultMinMax.x && nums <= vaultMinMax.y) {
			vaultable = true;
		} else {
			vaultable = false;
		}
		if (nums > ClimbMinMax.x && nums <= ClimbMinMax.y) {
			climbable = true;
		} else {
			climbable = false;
		}
		if (vaultable && !animator.GetCurrentAnimatorStateInfo(0).IsName("Vault") && controller.enabled && (Input.GetButtonDown ("Jump"))) {
			vaultPos = raycastHit2.point;
			vaultNor = -raycastHit.normal;
			vaultNor.y = 0;
			animator.CrossFade ("Vault", 0.02f, 0);
			StartCoroutine (vaultRotin ());
			vaultTarget = vaultPos + Vector3.up * vaultFix.x + base.transform.forward * vaultFix.y;
		}
		if (vaultFwd) {
			base.transform.position = Vector3.MoveTowards(base.transform.position, vaultTarget, 5f * Time.deltaTime);
			if (vaultNor != Vector3.zero)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(vaultNor), 15f * Time.deltaTime);
			}
		}
		if (climbable && !animator.GetCurrentAnimatorStateInfo(0).IsName("Climb") && controller.enabled && (Input.GetButtonDown ("Jump"))) {
			climbPos = raycastHit2.point;
			climbNor = -raycastHit.normal;
			climbNor.y = 0;
			animator.CrossFade ("Climb", 0.02f, 0);
			StartCoroutine (climbRotin ());
			climbPos = climbPos + Vector3.up * ClimbFix.x + base.transform.forward * ClimbFix.y;
		}
		if (ClimbFwd) {
			base.transform.position = Vector3.MoveTowards(base.transform.position, climbPos, 5f * Time.deltaTime);
			if (vaultNor != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation (climbNor);
			}
		}
	}



	IEnumerator vaultRotin(){
		isJumping = false;
		vaultFwd = true;
		yield return new WaitForSeconds (0.25f);
		vaultFwd = false;
	}

	IEnumerator climbRotin(){
		isJumping = false;
		ClimbFwd = true;
		yield return new WaitForSeconds (0.3f);
		ClimbFwd = false;
		yield return new WaitForSeconds (2f);
		velocityY = 2f;
	}

	float GetModifiedSmoothTime(float smoothTime) {
		if (controller.isGrounded) {
			return smoothTime;
		}

		if (airControlPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            Climbing = true;
        }

        else if (other.gameObject.tag == "Zipline")
        {
            Zipline = true;
            EndZipline = other.gameObject.transform.Find("End").GetComponent<Transform>();
            StartZipline = other.GetComponent<Transform>();
        }

        else if (other.gameObject.tag == "EndZipline")
        {
            Zipline = false;
            EndZipline = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            Climbing = false;
        }
    }

    void UseZipline()
    {
        float time = 5;
        float elapsedtime = 0;
        Vector3 StartPosition = transform.position;

        while (elapsedtime < time)
        {
            transform.position = Vector3.Lerp(StartPosition, EndZipline.position, 2 * Time.deltaTime);
            elapsedtime += Time.deltaTime;
            
        }
        

    }

}
