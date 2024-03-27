// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using WeChatWASM;

// public class WxLogin : MonoBehaviour
// {
// #if !UNITY_EDITOR
//     double screenWidth;
//     double screenHeight;
//     void Start()
//     {
//         var sysInfo = WX.GetSystemInfoSync();
//         screenWidth = sysInfo.screenWidth;
//         screenHeight = sysInfo.screenHeight;
//     }

//     public void Login()
//     {
//         // 获取微信界面大小
//         //   var self = this
//         WX.Login(new LoginOption()
//         {
//             success = (res) =>
//             {
//                 if (res.code != null)
//                 {
//                     var code = res.code;
//                     Debug.Log("登陆成功,获取到code:" + code);
//                     //  WX.request({

//                     //   url:GLOBAL_REQUEST_URL,

//                     //   data:{

//                     //     "code":code

//                     //   },

//                     //   success (res) {

//                     //     //res.data 就是返回的json 字符串解解析后的数据 res.data.account.sdkId

//                     //     Debug.Log("login result:"+res.data)

//                     //     //var loginResult = JSON.parse(res);

//                     //     // loginResult.get

//                     //    }

//                     //  });

//                 }



//             }

//         });

//     }
//     public void CreateLoginButton()
//     {
//         // var button = WX.CreateUserInfoButton(0, 0, (int)screenWidth, (int)screenHeight, "zh_CN", false);

//         // button.OnTap((res) =>

//         // {

//         //     if (res.errMsg == "getUserInfo:ok")
//         //     {
//         //         Debug.Log("授权用户信息");
//         //         //获取到用户信息

//         //         var userInfo = res.userInfo;

//         //         // self.wxLogin(userInfo);

//         //         WX.GetUserInfo(new GetUserInfoOption()
//         //         {

//         //             lang = "zh_CN",

//         //             success = (res) =>
//         //             {

//         //                 var userInfo = res.userInfo;

//         //                 var avatarUrl = userInfo.avatarUrl;

//         //                 //  assetManager.loadRemote(avatarUrl,{ ext: '.png' },(err, spriteFrame)=>{

//         //                 //   if(err){

//         //                 //     return;

//         //                 //   }

//         //                 //   DownloadResource.avartarSpriteFrame = SpriteFrame.createWithImage(spriteFrame as ImageAsset);
//         //                 Debug.Log("userInfo:" + userInfo);
//         //             },

//         //             fail = (res) =>
//         //                   {
//         //             Debug.Log("获取用户信息失败");
//         //         }
//         //         });
//         //         //清除微信授权按钮

//         //         button.Destroy();
//         //     }
//         //     else
//         //     {
//         //         Debug.Log("用户信息授权失败");
//         //     }

//         // });
//     }
// #endif
// }
