using System.IO;
using GameBase.Player;
using Manager;
using Struct;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameStart : MonoSingleton<GameStart>
{
    [SerializeField]public bool DevelopToggle;
    private QuestionController questionController;
    private Player player;
    private UIManager uiManager;
    private new void Awake()
    {
        GameStartFun();
        test();
    }
    private void GameStartFun()
    {
        InitQuestionController();
        InitSpawnAndCharacter();
        //InitCamera();
        InitUIManager();
    }




    private void InitQuestionController()
    {
        questionController = new QuestionController();
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

    private void test()
    {

    }
}