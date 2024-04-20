using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Struct
{
    public class CsvStaticData
    {
        
        public static readonly DataTable ConfigTable=new DataTable();
        
        public static readonly DataTable TureOrFalseQuestionTable=new DataTable();
        
        public static readonly DataTable TwoAnswerQuestionTable=new DataTable();
        
        public static readonly DataTable ThreeAnswerQuestionTable=new DataTable();

        public static readonly Dictionary<string, Texture2D> Texture2DDic = new Dictionary<string, Texture2D>();
        
        public static void SetCsvDataTable(Action callback = null)//进入游戏时读一次存起来
        {
            LoadManager.Instance.LoadCsvAssetAsync("QuestionConfig",ConfigTable,(() =>
            {
                LoadManager.Instance.LoadCsvAssetAsync("TureOrFalseQuestion",TureOrFalseQuestionTable,(() =>
                {
                    LoadManager.Instance.LoadCsvAssetAsync("TwoAnswerQuestion",TwoAnswerQuestionTable,(() =>
                    {
                        LoadManager.Instance.LoadCsvAssetAsync("ThreeAnswerQuestion",ThreeAnswerQuestionTable,(() =>
                        {
                            callback?.Invoke();  
                        }));
                    }));
                }));
            }));
            
            
            
        }

    }
}