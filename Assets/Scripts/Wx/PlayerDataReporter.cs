using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using Struct;
using UnityEngine;
using WeChatWASM;
using Wx;

public class PlayerDataReporter
{
    public void UpPlayerInfoDataToRank(PlayerGameInfo playerGameInfo)
    {
        KVData kvData=new KVData
        {
            key = "score",
            value = playerGameInfo.Score.ToString()
        };
        
        var setUserCloudStorageOption = new SetUserCloudStorageOption
        {
            
            KVDataList = new[]
            {
                kvData
            },
            complete = res => {

                System.Console.WriteLine("SetUserCloudStorage   complete:"+res);
            },
            fail = res => {

                System.Console.WriteLine("SetUserCloudStorage   Fail:"+res);
            },
            success = res => {

                System.Console.WriteLine("SetUserCloudStorage   success:"+res);
            },
        };

        WX.SetUserCloudStorage(setUserCloudStorageOption);
    }

    public void UpPlayerInfoDataToUserData(PlayerGameInfo playerGameInfo)
    {
        WxCloudHelper wxCloudHelper = new WxCloudHelper();
        wxCloudHelper.SetUserData(playerGameInfo);
    }
    
    public void UpPlayerInfoDataToQuestionData(PlayerGameInfo playerGameInfo)
    {
        WxCloudHelper wxCloudHelper = new WxCloudHelper();
        var questionIdData = new QuestionIdData
        {
            CorrectQuestionIdList = playerGameInfo.CorrectQuestionIdList,
            WrongQuestionIdList = playerGameInfo.WrongQuestionIdList
        };
        wxCloudHelper.SetQuestionData(questionIdData);
    }
}
