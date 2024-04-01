using System;
using BrokenVector.LowPolyFencePack;
using Manager;
using Struct;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoSingleton<Player>
{
    public CharacterLocomotion characterLocomotion;
    private CharacterCollision characterCollision;
    public RunwayBackgroundEnvironmentManager runwayBackgroundEnvironmentManager;

    private readonly string LeftPos = "FirstLeftRunwayBackgroundEnvironmentPos";
    private readonly string RightPos = "FirstRightRunwayBackgroundEnvironmentPos";

    private void Start()
    {
        InitChildrenModule();
    }

    private void InitChildrenModule()
    {
        characterLocomotion = new CharacterLocomotion();
        characterLocomotion.Init(transform.GetComponent<CharacterController>(),transform);//角色移动
        
        characterCollision = new CharacterCollision(this);//角色碰撞
        
        runwayBackgroundEnvironmentManager =
            new RunwayBackgroundEnvironmentManager(GameObject.Find(LeftPos), GameObject.Find(RightPos));//跑道背景管理器
    }

    private void Update()
    {
        characterLocomotion.Update();
    }

    private void FixedUpdate()
    {
        characterLocomotion.FixUpdate();
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
}