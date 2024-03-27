using System.IO;
using Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameStart : MonoSingleton<GameStart>
{
    private void Awake()
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



    public void BackToMainPanel()
    {
        UIManager.Instance.ShowPanel<MainPanel>();
        
        QuestionController.Instance.ReStart();//重置参数
        Player.Instance.ReStart();
        Player.Instance.SetHasStart(false);
        Player.Instance.SetHasEnd(false);
        ScoreManager.Instance.ReStart();
        
        RunwayManager.Instance.InitRunways();
        RunwayManager.Instance.DestoryFinishLine();
    }
    public void GameReStart()//ReStart里面重置参数
    {
        UIManager.Instance.ShowPanel<RunningPanel>();
        
        QuestionController.Instance.ReStart();//重置参数
        Player.Instance.ReStart();
        ScoreManager.Instance.ReStart();
        
        QuestionController.Instance.ManualStart();
        Player.Instance.SetHasStart(true);
        Player.Instance.SetHasEnd(false);
        
        RunwayManager.Instance.InitRunways();
        RunwayManager.Instance.DestoryFinishLine();
    }
    private void InitQuestionController()
    {
        LoadManager.Instance.LoadAndShowPrefabAsync("QuestionController", "Assets/Prebs/UI/QuestionController.prefab",null,
            (o =>
            {
                RunwayManager.Instance.InitRunways();
                GameStaticDataInit();
            }));
    }

    private void GameStaticDataInit()
    {
        
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