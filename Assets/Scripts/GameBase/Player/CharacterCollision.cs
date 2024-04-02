using BrokenVector.LowPolyFencePack;
using Manager;
using Struct;
using UnityEngine;


    public class CharacterCollision
    {
        private Player player;
        public CharacterCollision(Player p)
        {
            player = p;
        }
        public void OnTriggerEnter(Collider other)
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
                int mode = -1;
                if (layerValue == QuestionController.Instance.levelData[GameStaticData.HasPassedNum].way)
                {
                    
                    mode = 1;//加速
                    ChooseCorrectWay();
                }
                else
                {
                    mode = 0;//減速
                    ChooseWrongWay();
                }
                player.characterLocomotion.ChangeSpeed(mode);
                QuestionController.Instance.NextQuestion();
            }
            void ChooseCorrectWay()
            {
                PlayFenceAni(other);
                // ChangeFenceColor(other, 1);
                GameStaticData.HasCorrectNum++;
                ScoreManager.Instance.AddScore(GameStaticData.HasPassedNum);
                GameStaticData.CorrectQuestionIdList.Add(QuestionController.Instance.levelData[GameStaticData.HasPassedNum].id);
            }

            void ChooseWrongWay()
            {
                PlayFenceAni(other);
                GameStaticData.WrongQuestionIdList.Add(QuestionController.Instance.levelData[GameStaticData.HasPassedNum].id);
                // ChangeFenceColor(other, 0);
            }
        }
        
        private void PlayFenceAni(Collider other)
        {
            other.transform.GetComponentInParent<DoorController>().OpenDoor();
        }
        private void OnTriggerEnterFinishLine(LayerMask layer)
        {
            if (layer.value - 10 == 2 && !GameStaticData.GameHasEnd)
            {
                QuestionController.Instance.ClearQuestion();
                UIManager.Instance.HideAllPanel();
                UIManager.Instance.ShowPanel<GameOverPanel>();
                GameStaticData.GameHasEnd = true;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            string tag = other.tag;
            LayerMask layer = other.gameObject.layer;
            if (tag == "loadRunway")
            {
                player.runwayBackgroundEnvironmentManager.CreateNewRunwayBackgroundEnvironment();
                if (!QuestionController.Instance.IsAllQuestionDone())
                {
                    RunwayManager.Instance.CreateNewRunway();
                }
            }

            if (layer.value - 10 == 0 || layer.value - 10 == 1)
            {
                ++GameStaticData.HasPassedNum;
            }
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
    }