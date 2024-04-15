using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class PlayerDataReporter
{
    public void UpPlayerInfoDataToRank(string scoreValue)
    {
        KVData kvData=new KVData
        {
            key = "score",
            value = "1234"
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
}
