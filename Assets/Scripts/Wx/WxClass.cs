using GameBase;
using LitJson;
using Manager;
using Struct;
using UnityEngine;
using WeChatWASM;

namespace Wx
{
    public static class WxClass
    {
        public static void InitSDK()
        {
            WXBase.InitSDK((int code) =>
            {
                System.Console.WriteLine("WXInitSDKComplete");
                InitCloud();
            });
        }

        private static void InitCloud()
        {
            WXBase.cloud.Init(new CallFunctionInitParam()
            {
                env = "mathrunruncloud-9gio3fjz9fc0f7c1"
            });
        }

        public static void GameOverUpload(PlayerGameInfo playerGameInfo)
        {
            var reporter = new PlayerDataReporter();
            UploadWxRankData(reporter,playerGameInfo);
            UploadGameInfo(reporter,playerGameInfo);
        }

        private static void UploadWxRankData(PlayerDataReporter reporter,PlayerGameInfo playerGameInfo)//排行榜数据写入
        {
            reporter.UpPlayerInfoDataToRank(playerGameInfo);
        }
        private static void UploadGameInfo(PlayerDataReporter reporter,PlayerGameInfo playerGameInfo)//提交玩家数据 更新数据库
        {
            reporter.UpPlayerInfoDataToUserData(playerGameInfo);//添加本局游戏记录
            // reporter.UpPlayerInfoDataToQuestionData(playerGameInfo);//更新题目总表数据
        }
    }
}