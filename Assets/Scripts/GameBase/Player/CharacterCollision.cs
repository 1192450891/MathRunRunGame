using BrokenVector.LowPolyFencePack;
using GameBase.Player;
using Manager;
using Struct;
using UnityEngine;


    public class CharacterCollision
    {
        private readonly Player player;
        public CharacterCollision(Player p)
        {
            player = p;
        }
        public void OnTriggerEnter(Collider other)
        {
            LayerMask layer = other.gameObject.layer;
            if (layer==11)
            {
                OnTriggerEnterFence(other);
            }
            OnTriggerEnterFinishLine(layer);
        }
        private void OnTriggerEnterFence(Collider other)
        {
            int mode = -1;
            if (other.name[0]-'0' == QuestionController.Instance.CurLevelData.Way)
            {
                mode = 1;//加速
                ChooseCorrectWay();
            }
            else
            {
                mode = 0;//減速
                ChooseWrongWay();
            }
            PlayFenceAni(other);
            player.ChangeSpeed(mode);
            QuestionController.Instance.NextQuestion();//最后去切换问题
            
            void ChooseCorrectWay()
            {
                GameStaticData.HasCorrectNum++;
                ScoreManager.Instance.AddScore();
                GameStaticData.CorrectQuestionIdList.Add(QuestionController.Instance.CurLevelData.ID);
            }

            void ChooseWrongWay()
            {
                GameStaticData.WrongQuestionIdList.Add(QuestionController.Instance.CurLevelData.ID);
            }
        }
        
        private void PlayFenceAni(Collider other)
        {
            other.transform.GetComponent<FenceAni>().ToggleFence();
        }
        private void OnTriggerEnterFinishLine(LayerMask layer)
        {
            if (layer.value - 10 == 2 && !GameStaticData.GameHasEnd)
            {
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
                player.CreateNewRunwayBackgroundEnvironment();
                if (!RunwayManager.Instance.IsAllQuestionHasCreated())
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
