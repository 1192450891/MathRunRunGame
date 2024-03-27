using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLocomotion : MonoSingleton<CharacterLocomotion>
{
    [Tooltip("Assign animator if you would like. We are using 2d blendtree")]
    [SerializeField] Animator animator;
    //this script will make you character move correctly regardless of how camera is setup
    [Tooltip("Character controller is a built in component in unity. Feel free to use rigidbody or changing transform directly")]
    [SerializeField] CharacterController characterController;
    [Tooltip("how fast the player walks")]
    
    private float walkSpeed;
    public float minwalkSpeed;
    public float maxwalkSpeed;
    public bool canOperate;//是否能进行操作
    public float WalkSpeed
    {
        get { return walkSpeed; }
        set { 
        walkSpeed = Mathf.Clamp(value,minwalkSpeed,maxwalkSpeed);
        EventManager.Instance.TriggerEvent<float>(ClientEvent.RunningPanel_SpeedChange,walkSpeed);
        }
    }
    [Tooltip("if you would like separate visual from player assign something else here")]
    [SerializeField] Transform characterVisual;//if you would like separate visual from player assign something else here
    [Tooltip("Turn this off if you want to separate movement and aiming")]
    [SerializeField] bool lookToMovementDirection = true;//turn this off if you want to separate movement and aiming
    [Tooltip("Feel free to assign other joysticks here")]
    public FixedJoystick moveJoystick;//assign joystick here
    [Tooltip("Self explanatory. After this magnitude player will move ")]
    [SerializeField] float movementThreshold = 0.1f;// self explanatory. After this magnitude player will move 
    [Header("Animation variables")]
    [Tooltip("This will turn rotation towards the joystick direction")]
    [SerializeField] bool canStrafe = false;
    [Tooltip("Animation variables for blendtrees")]
    [SerializeField] string forwardAnimationVar = "Forward";
    [Tooltip("Animation variables for blendtrees")]
    [SerializeField] string strafeAnimationVar = "Strafe";
    float mag; // maginutde
    Transform camTransform;
    Vector3 fwd,right; //camera fwd,right
    Vector3 input,move;//input for animations
    Vector3 cameraForward;
    public bool hasStart;
    public bool hasEnd;
    float forward,strafe;//we will use them in animation variables
    void Awake()
    {
        if(characterController == null){
            characterController = GetComponent<CharacterController>();
            //getting the characterController component
        }
        if(characterVisual == null){
            characterVisual = transform;
        }
        camTransform = Camera.main.transform;
    }
    void Start(){
        characterController.detectCollisions = false; //we don't want character controller to detect collisions
        RecalculateCameraGaming();
        // RecalculateCamera(Camera.main);//we should know where camera is looking at. Call this method each time camera angle changes
        //also consider caching the camera
    }
    void Update(){
        if(!canOperate) return;

        mag = Mathf.Clamp01(new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical).sqrMagnitude);
        if(canStrafe){
            lookToMovementDirection = false;
            //I turn it off because player needs to strafe to it's forward.
            //use strafe when you look at certain object(target) for instance
        }
        //getting the magnitude
        if (mag >= movementThreshold || PlayerIsPlaying()) 
        {
            MovementAndRotation();
        }
        else{
            characterController.Move(new Vector3(0,-1f,0));//gravity when idle
        }
        if(animator != null){
            if(canStrafe){
                RelativeAnimations();
            }
            else{
                if(PlayerIsPlaying()) mag=1;
                animator.SetFloat(forwardAnimationVar,mag);
            }
        }
    }
   
    void RelativeAnimations(){
        if (camTransform != null)
        {
            cameraForward = Vector3.Scale(camTransform.up, new Vector3(1, 0, 1)).normalized; //camera forwad
            move = moveJoystick.Vertical * cameraForward + moveJoystick.Horizontal * camTransform.right;//relative 
            //vector to camera forward and right
        }
        else
        {
            move = moveJoystick.Vertical * Vector3.forward + moveJoystick.Horizontal * Vector3.right;
            //if there is no camera transform(for any reason then we use joystick forward and right)
        }
        if (move.magnitude > 1)
        {
            move.Normalize();//normalizing here
        }
        MoveAnims(move);
    }
    void MoveAnims(Vector3 move){
        this.input = move;
        Vector3 localMove = transform.InverseTransformDirection(input);//inversing local move from the input
        strafe = localMove.x;//x is right input relative to camera 
        forward = localMove.z;//z is forward joystick input relative to camera
        animator.SetFloat(forwardAnimationVar, forward*2f, 0.01f, Time.deltaTime);//setting animator floats
        animator.SetFloat(strafeAnimationVar, strafe*2f, 0.01f, Time.deltaTime);
    }
    // void RecalculateCamera(Camera _cam)//不在闯关中
    // {
    //         Camera cam = _cam;
    //         camTransform = cam.transform;
    //         fwd = cam.transform.forward; //camera forward
    //         fwd.y = 0;
    //         fwd = Vector3.Normalize(fwd);
    //         right = Quaternion.Euler(new Vector3(0, 90, 0)) * fwd; //camera right
    // }
    void RecalculateCameraGaming()//游戏进行中
    {
        fwd = Vector3.forward; //世界坐标向前自动移动 不需要相机
        fwd = Vector3.Normalize(fwd);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * fwd; //camera right
    }
    void MovementAndRotation(){
        Vector3 direction = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);//joystick direction
        Vector3 rightMovement = right * walkSpeed * Time.deltaTime * moveJoystick.Horizontal;//getting right movement out of joystick(relative to camera)
        Vector3 upMovement;
        if(PlayerIsPlaying())
        {
            upMovement = fwd * walkSpeed * Time.deltaTime; //getting up movement out of joystick(relative to camera)
        }
        else
        {
            upMovement = fwd * walkSpeed * Time.deltaTime * moveJoystick.Vertical;
        }
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement); //final movement vector
        heading.y = -9.8f;//gravity while moving
        characterController.Move(heading * walkSpeed*Time.deltaTime);//move
        if(lookToMovementDirection){
            characterVisual.forward = new Vector3(heading.x,characterVisual.forward.y,heading.z);
            //look to movement direction
        }
    }
    public bool PlayerIsPlaying()//检测玩家是否正在跑道上闯关
    {
        return hasStart&&!hasEnd;
    }
}
