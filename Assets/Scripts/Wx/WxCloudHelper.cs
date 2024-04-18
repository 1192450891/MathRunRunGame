using WeChatWASM;

namespace Wx
{
    public class WxCloudHelper
    {
        public void w()
        {
            var downloadFileOption = new DownloadFileOption
            {
                url = null,
                complete = null,
                fail = null,
                filePath = null,
                header = null,
                success = null,
                timeout = null
            };
            WX.DownloadFile(downloadFileOption);
        }

        // 下载云存储文件到本地临时文件
        // wx.cloud.downloadFile({fileID:'your-file-id',
        //     success:res =>{
        //         //  获取本地临时文件的文件信息
        //     wx.getFileInfo({
        //         filePath: res.tempFilePath,
        //         success:fileInfo =>{
        //             console.log('文件大小:'，fileInfo.size)console.log('创建时间:”,fileInfo.createTime)/! 使用获取到的文件信息进行下一步操作
        //         }，
        //         fail:err =>{
        //             console.error('获取文件信息失败:'’err
        //             子)
        //             fail:err =>{
        //                 console.error('下载文件失败:'，err)
        // }
    }
}