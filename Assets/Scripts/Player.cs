using Manager;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoSingleton<Player>
{
    [Tooltip("开局速度")] public float startrationRate;
    [Tooltip("加速率")] public float accelerationRate = 2f;
    [Tooltip("減速率")] public float dccelerationRate = 5f;
    [Tooltip("自然減速率")] public float Natural_deceleration_rate = 1000;
    [Tooltip("已通过关卡计数")] public int hasPassedNum;
    private QuestionController questionController=>QuestionController.Instance;
    private CharacterLocomotion characterLocomotion;
    private RunwayBackgroundEnvironmentManager runwayBackgroundEnvironmentManager;
    

    private void Start()
    {
        characterLocomotion = GetComponent<CharacterLocomotion>();
        InitSpeed();
        InitRunwayBackgroundEnvironmentManager();
        InitRunwayBackgroundEnvironment();
    }

    private void FixedUpdate()
    {
        if (characterLocomotion.PlayerIsPlaying())
        {
            characterLocomotion.WalkSpeed -= dccelerationRate * Mathf.Log10(characterLocomotion.WalkSpeed) /
                                             Natural_deceleration_rate;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LayerMask layer = other.gameObject.layer;
        OnTriggerEnterFence(layer, other);
        OnTriggerEnterFinishLine(layer);
    }

    private void OnTriggerEnterFence(LayerMask layer, Collider other)
    {
        if (layer.value - 10 == 0 || layer.value - 10 == 1)
        {
            if (layer.value - 10 == questionController.levelData[hasPassedNum].way)
            {
                ChangeSpeed(1); //加速
                ChangeFenceColor(other, 1);
                ScoreManager.Instance.AddScore(hasPassedNum);
                questionController.NextQuestion();
            }
            else
            {
                ChangeSpeed(0); //減速
                ChangeFenceColor(other, 0);
                questionController.NextQuestion();
            }
        }
    }

    private void OnTriggerEnterFinishLine(LayerMask layer)
    {
        if (layer.value - 10 == 2 && !characterLocomotion.hasEnd)
        {
            questionController.ClearQuestion();
            UIManager.Instance.HideAllPanel();
            UIManager.Instance.ShowPanel<GameOverPanel>();
            SetHasEnd(true);
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


    private void ChangeSpeed(int num)
    {
        float currentSpeed = characterLocomotion.WalkSpeed;
        switch (num)
        {
            case 0:
                characterLocomotion.WalkSpeed -= dccelerationRate * Mathf.Log10(currentSpeed);
                break;
            case 1:
                characterLocomotion.WalkSpeed += accelerationRate * Mathf.Log(currentSpeed);
                break;
        }

        SetHasStart(true);
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
        characterLocomotion.enabled = false;
        transform.position = vector3;
        TimeTool.Instance.Delay(0.3f, () =>
        {
            characterLocomotion.enabled = true;
        }
        );
    }
    public void SetHasStart(bool isStart)
    {
        characterLocomotion.hasStart = isStart;
    }
    public void SetHasEnd(bool isEnd)
    {
        characterLocomotion.hasEnd = isEnd;
    }
    private void InitSpeed()
    {
        characterLocomotion.WalkSpeed = startrationRate;
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
        InitSpeed();
        SetPos(GameObject.Find("PlayerPos").transform.position);

    }
}