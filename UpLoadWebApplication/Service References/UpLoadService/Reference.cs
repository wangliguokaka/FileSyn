﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpLoadWebApplication.UpLoadService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="UpLoadService.IUpLoadService")]
    public interface IUpLoadService {
        
        // CODEGEN: 消息 FileUploadMessage 的包装名称(FileUploadMessage)以后生成的消息协定与默认值(UploadFile)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="UploadFile", ReplyAction="http://tempuri.org/IUpLoadService/UploadFileResponse")]
        UpLoadWebApplication.UpLoadService.ReMessage UploadFile(UpLoadWebApplication.UpLoadService.FileUploadMessage request);
        
        [System.ServiceModel.OperationContractAttribute(Action="GetAllFiles", ReplyAction="http://tempuri.org/IUpLoadService/GetNewFilesResponse")]
        string[] GetNewFiles();
        
        [System.ServiceModel.OperationContractAttribute(Action="DownLoadFile", ReplyAction="http://tempuri.org/IUpLoadService/DownLoadFileResponse")]
        System.IO.Stream DownLoadFile();
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="FileUploadMessage", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class FileUploadMessage {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string FileName;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string SavePath;
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public string[] listFile;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public System.IO.Stream FileData;
        
        public FileUploadMessage() {
        }
        
        public FileUploadMessage(string FileName, string SavePath, string[] listFile, System.IO.Stream FileData) {
            this.FileName = FileName;
            this.SavePath = SavePath;
            this.listFile = listFile;
            this.FileData = FileData;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ReMessage", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ReMessage {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/")]
        public bool IsPass;
        
        public ReMessage() {
        }
        
        public ReMessage(bool IsPass) {
            this.IsPass = IsPass;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUpLoadServiceChannel : UpLoadWebApplication.UpLoadService.IUpLoadService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UpLoadServiceClient : System.ServiceModel.ClientBase<UpLoadWebApplication.UpLoadService.IUpLoadService>, UpLoadWebApplication.UpLoadService.IUpLoadService {
        
        public UpLoadServiceClient() {
        }
        
        public UpLoadServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UpLoadServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UpLoadServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UpLoadServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        UpLoadWebApplication.UpLoadService.ReMessage UpLoadWebApplication.UpLoadService.IUpLoadService.UploadFile(UpLoadWebApplication.UpLoadService.FileUploadMessage request) {
            return base.Channel.UploadFile(request);
        }
        
        public bool UploadFile(string FileName, string SavePath, string[] listFile, System.IO.Stream FileData) {
            UpLoadWebApplication.UpLoadService.FileUploadMessage inValue = new UpLoadWebApplication.UpLoadService.FileUploadMessage();
            inValue.FileName = FileName;
            inValue.SavePath = SavePath;
            inValue.listFile = listFile;
            inValue.FileData = FileData;
            UpLoadWebApplication.UpLoadService.ReMessage retVal = ((UpLoadWebApplication.UpLoadService.IUpLoadService)(this)).UploadFile(inValue);
            return retVal.IsPass;
        }
        
        public string[] GetNewFiles() {
            return base.Channel.GetNewFiles();
        }
        
        public System.IO.Stream DownLoadFile() {
            return base.Channel.DownLoadFile();
        }
    }
}
