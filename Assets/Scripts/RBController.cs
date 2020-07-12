using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBController : MonoBehaviour
{
    public float mouseSensitivity, thrust, jumpForce, gAcceleration, staticDrag;
    [Header("Dash")]
    public float dashForce;
    public float dashDuration;
    public float fovSpeed;
    public float dashFov;
    public float dashCooldown;
    [Header("Wall Climb")]
    public float climbSpeed;
    public float edgeUpForce;
    private bool isGrounded, isClimbing, isDashing;
    private float inputX, inputY, gravity,wallRaycastDistance, baseCameraFov, dashCooldownTimer = 0;
    private Vector3 dir;
    private Rigidbody rb;
    private int jumpCount;
    private PlayerCamera playerCamScript;
    private Camera playerCam;
    void Start()
    {
        playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCamScript = GameObject.Find("Main Camera").GetComponent<PlayerCamera>();

        baseCameraFov = playerCam.fieldOfView;
        playerCamScript.mouseSensitivity = this.mouseSensitivity;
        wallRaycastDistance = 1;
        gravity = 0;
        rb = GetComponent<Rigidbody>();
        isClimbing = false;
        isDashing = false;
    }
    void Update()
    {
        RegisterGrounded();
        RegisterInput();
        RegisterClimbing();
        RegisterDrag();
    }
    void FixedUpdate(){
        Move();
        RegisterFOVTransition();
    }
    void Move(){
        //Move
        dir = (transform.right * inputX) + (transform.forward * inputY); 
        rb.AddForce(dir*thrust/2, ForceMode.VelocityChange);
        //Fall
        if(!isGrounded)
            rb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }
    IEnumerator Climb(Collider climbableCollider){
        while(Input.GetKey("space")){
            isClimbing = true;
            rb.useGravity = false;
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, wallRaycastDistance)){
                if(hit.collider == climbableCollider){
                    ResetVelocityY();
                    rb.velocity += new Vector3(0,climbSpeed,0);
                    yield return null;
                }
                else
                    break;
            }
            else
                break;
        }
        ResetVelocityY();
        rb.useGravity = true;
        isClimbing = false;
        jumpCount = 1;
        rb.velocity = new Vector3(0,edgeUpForce,0);
    }
    IEnumerator Dash(){
        rb.AddForce(Camera.main.transform.forward * dashForce, ForceMode.VelocityChange);
        
        yield return new WaitForSeconds(dashDuration);

        //Reset velocity after dash (removes innertia)
        rb.velocity = Vector3.zero;

        isDashing = false;
    }
    void Jump(){
        ResetVelocityY();
        if(jumpCount == 0){
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            if(isGrounded)
                jumpCount = 1;
            else
                jumpCount = 2;
        }

        else if(jumpCount == 1){
            rb.AddForce(Vector3.up * jumpForce * 2, ForceMode.VelocityChange);
            jumpCount = 2;
        }
    }
    void RegisterInput(){
        //Input
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        
        //Jump
        if(Input.GetKeyDown("space") && jumpCount < 2 && !isClimbing)
            Jump();
        //Manages Dash Cooldown
        if(dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
        //Dash
        else if(Input.GetKeyDown(KeyCode.V)){
            isDashing = true;
            //Starts Dash Cooldown
            dashCooldownTimer = dashCooldown;
            StartCoroutine(Dash());
        }
            
    }
    void RegisterGrounded(){
        //if a collider was hit, we are grounded
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f)) {
            isGrounded = true;
            //Set jump count on grounded
            jumpCount = 0;
            //Sets gravity back to 0
            gravity = 0;
        }
        else{
            //Increase downwards vertical velocity on every itteration
            gravity += gAcceleration;
            isGrounded = false;
        }
    }
    void RegisterClimbing(){
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, wallRaycastDistance) && Input.GetKey("space")){
            if(hit.collider.tag == "Wall"){
                ResetVelocityY();
                rb.angularVelocity = Vector3.zero;
                StartCoroutine(Climb(hit.collider));
            }
        }
    }
    void ResetVelocityY(){
        //Resets vertical velocity
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        gravity = 0;
    }
    void RegisterDrag(){
        if(Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0 && !isClimbing && isGrounded)
            rb.drag = staticDrag;
        else
            rb.drag = 3;
    }
    void RegisterFOVTransition(){
        if(isDashing && playerCam.fieldOfView < dashFov)
            playerCam.fieldOfView += fovSpeed;
        else if(!isDashing && playerCam.fieldOfView > baseCameraFov)
            playerCam.fieldOfView -= fovSpeed*2;
    }
}
