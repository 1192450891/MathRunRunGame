using System;
using BrokenVector.LowPolyFencePack;
using Manager;
using Struct;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoSingleton<Player>
{
    [Tooltip("已通过关卡计数")] public int hasPassedNum;
    private QuestionController questionController=>QuestionController.Instance;
    public CharacterLocomotion characterLocomotion;
    private RunwayBackgroundEnvironmentManager runwayBackgroundEnvironmentManager;
    

    private void Start()
    {
        InitCharacterLocomotion();
        InitRunwayBackgroundEnvironmentManager();
        InitRunwayBackgroundEnvironment();
    }

    private void InitCharacterLocomotion()
    {
        characterLocomotion = new CharacterLocomotion();
        characterLocomotion.Init(transform.GetComponent<CharacterController>(),transform);
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
        LayerMask layer = other.gameObject.layer;
        OnTriggerEnterFence(layer, other);
        OnTriggerEnterFinishLine(layer);
    }

    private void OnTriggerEnterFence(LayerMask layer, Collider other)
    {
        int layerValue = layer.value - 10;
        if (layerValue == 0 || layerValue == 1)
        {
            if (layerValue == questionController.levelData[hasPassedNum].way)
            {
                characterLocomotion.ChangeSpeed(1);//加速
                PlayFenceAni(other);
                ChangeFenceColor(other, 1);
                ScoreManager.Instance.AddScore(hasPassedNum);
                questionController.NextQuestion();
            }
            else
            {
                characterLocomotion.ChangeSpeed(0); //減速
                PlayFenceAni(other);
                ChangeFenceColor(other, 0);
                questionController.NextQuestion();
            }
        }
    }

    private void OnTriggerEnterFinishLine(LayerMask layer)
    {
        if (layer.value - 10 == 2 && !GameStaticData.GameHasEnd)
        {
            questionController.ClearQuestion();
            UIManager.Instance.HideAllPanel();
            UIManager.Instance.ShowPanel<GameOverPanel>();
            GameStaticData.GameHasEnd = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.tag;
        LayerMask layer = other.gameObject.layer;
        if (tag == "loadRunway")
        {
            runwayBackgroundEnvironmentManager.CreateNewRunwayBackgroundEnvironment();
            if (!questionController.IsAllQuestionDone())
            {
                CreatNewRunWay();
            }
        }

        if (layer.value - 10 == 0 || layer.value - 10 == 1)
        {
            ++hasPassedNum;
        }
    }


    private void PlayFenceAni(Collider other)
    {
        other.transform.GetComponentInParent<DoorController>().OpenDoor();
    }
    private void ChangeFenceColor(Collider other, int num)
    {
        Renderer renderer = other.gameObject.GetComponent<Renderer>();
        switch (num)
        {
            case 0:
                renderer.material.color = Color.red;
                break;
            case 1:
                renderer.material.color = Color.green;
                break;
        }
    }

    public void SetPos(Vector3 vector3)
    {
        GameStaticData.CanOperate = false;
        transform.position = vector3;
        TimeTool.Instance.Delay(0.3f, () =>
        {
            GameStaticData.CanOperate = true;
        }
        );
    }
    private void InitRunwayBackgroundEnvironmentManager()
    {
        runwayBackgroundEnvironmentManager =
            new RunwayBackgroundEnvironmentManager(GameObject.Find("FirstLeftRunwayBackgroundEnvironmentPos"), GameObject.Find("FirstRightRunwayBackgroundEnvironmentPos"));
    }
    private void InitRunwayBackgroundEnvironment()
    {
        runwayBackgroundEnvironmentManager.CreateNewRunwayBackgroundEnvironment();
    }
    private void CreatNewRunWay()
    {
        RunwayManager.Instance.CreateNewRunway();
    }
    public void ReStart()
    {
        hasPassedNum = 0;
        characterLocomotion.InitSpeed();
        SetPos(GameObject.Find("PlayerPos").transform.position);

    }
}