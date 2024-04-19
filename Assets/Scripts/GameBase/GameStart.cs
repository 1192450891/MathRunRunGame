using System;
using System.Data;
using System.IO;
using GameBase.Player;
using Manager;
using Struct;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Wx;

public class GameStart : MonoSingleton<GameStart>
{
    [SerializeField]public bool DevelopToggle;
    private QuestionController questionController;
    private Player player;
    private UIManager uiManager;
    private new void Awake()
    {
        CsvStaticData.SetCsvStaticData(GameStartFun);
        WxInit();
        awakeTest();
    }



    private void GameStartFun()
    {
        InitQuestionController();
        InitSpawnAndCharacter();
        InitUIManager();
    }
    
    private void WxInit()
    {
#if !UNITY_EDITOR
        WxClass.InitSDK();
#endif
    }
    
    private void Update()
    {
        updateTest();
    }

    private void InitQuestionController()
    {
        QuestionController.Instance.GetData();
        RunwayManager.Instance.InitRunways();
    }

    private void InitSpawnAndCharacter()
    {
        LoadManager.Instance.LoadAndShowPrefabAsync("Spawn", "Assets/Prebs/Environment/Spawn.prefab",null,
            (o =>
            {
                InitCharacter();
            }));
    }
    private void InitCharacter()
    {
        var playerPos=GameObject.Find("PlayerPos");
        if (playerPos == null)
        {
            Debug.Log("InitCharacter Error");
            return;
        }
        LoadManager.Instance.LoadAndShowPrefabAsync("Player", "Assets/Prebs/Character/Player.prefab",null,
            (o)=>
            {
                o.transform.position = playerPos.transform.position;
            });
    }
    private void InitCamera()
    {
        LoadManager.Instance.LoadAndShowPrefabAsync("MainCamera", "Assets/Prebs/Camera/Main Camera.prefab");
    }
    private void InitUIManager()
    {
        LoadManager.Instance.LoadAndShowPrefabAsync("UIManager", "Assets/Prebs/Manager/UIManager.prefab");
        
    }

    private void awakeTest()
    {
        
    }
    private void updateTest()
    {
        if (Input.GetKey(KeyCode.K))
        {
            UIManager.Instance.ShowPanel<FreeMovePanel>();
        }
    }
}