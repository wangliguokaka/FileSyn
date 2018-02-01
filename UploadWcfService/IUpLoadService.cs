using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace UploadWcfService
{
    // 注意: 如果更改此处的接口名称 "IUpLoadService"，也必须更新 Web.config 中对 "IUpLoadService" 的引用。
    [ServiceContract]
    public interface IUpLoadService
    {
        [OperationContract(Action = "UploadFile", IsOneWay = false)]
        ReMessage UploadFile(FileUploadMessage request);

        [OperationContract(Action = "GetAllFiles", IsOneWay = false)]
        List<String> GetNewFiles();
        [OperationContract(Action = "DownLoadFile", IsOneWay = false)]
        Stream DownLoadFile();
    }


    [MessageContract]
    public class FileUploadMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string SavePath;

        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageBodyMember(Order = 1)]
        public Stream FileData;


        [MessageHeader(MustUnderstand = true)]
        public List<String> listFile;

        

    }

    [MessageContract]
    public class DownloadMessage
    {

        [MessageHeader(MustUnderstand = true)]
        public List<String> listFile;



    }

    [MessageContract]
    public class ReMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public bool IsPass;
    }

    [MessageContract]
    public class StreamMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public Stream FileStream;

        [MessageHeader(MustUnderstand = true)]
        public List<String> listFile;
    }

}
