using Manager;
using UnityEngine;

namespace GameBase.Player
{
    public class Player : MonoSingleton<Player>
    {
        private CharacterLocomotion characterLocomotion;
        private CharacterCollision characterCollision;
        private RunwayBackgroundEnvironmentManager runwayBackgroundEnvironmentManager;
        private PlayerSkyBox playerSkyBox;

        private const string leftPos = "FirstLeftRunwayBackgroundEnvironmentPos";
        private const string rightPos = "FirstRightRunwayBackgroundEnvironmentPos";

        private void Start()
        {
            InitChildrenModule();
        }

        private void InitChildrenModule()
        {
            characterLocomotion = new CharacterLocomotion();
            characterLocomotion.Init(transform);//角色移动
        
            characterCollision = new CharacterCollision(this);//角色碰撞
        
            runwayBackgroundEnvironmentManager =
                new RunwayBackgroundEnvironmentManager(GameObject.Find(leftPos), GameObject.Find(rightPos));//跑道背景管理器

            playerSkyBox = new PlayerSkyBox(transform);
        }

        private void Update()
        {
        }

        private void FixedUpdate()
        {
            characterLocomotion.FixUpdate();
            playerSkyBox.FixUpdate();
        }

        private void OnTriggerEnter(Collider other)
        {
            characterCollision.OnTriggerEnter(other);
        }


        private void OnTriggerExit(Collider other)
        {
            characterCollision.OnTriggerExit(other);
        }

        public void ReStart()
        {
            characterLocomotion.InitSpeed();
            characterLocomotion.SetPos(GameObject.Find("PlayerPos").transform.position);
        }

        public void SetJoyStick(Joystick joystickComponent)
        {
            characterLocomotion.MoveJoystick = joystickComponent;
        }
    
        public void ChangeSpeed(int mode)
        {
            characterLocomotion.ChangeSpeed(mode);
        }

        public void CreateNewRunwayBackgroundEnvironment()
        {
            runwayBackgroundEnvironmentManager.CreateNewRunwayBackgroundEnvironment();
        }
    }
}