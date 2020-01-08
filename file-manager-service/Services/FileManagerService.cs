using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Gov.Lclb.Cllb.Interfaces;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    // Default to require authorization
    [Authorize]
    public class FileManagerService : FileManager.FileManagerBase
    {
        private readonly ILogger<FileManagerService> _logger;
        private readonly IConfiguration _configuration;

        public FileManagerService(ILogger<FileManagerService> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override Task<FileExistsReply> FileExists(FileExistsRequest request, ServerCallContext context)
        {
            var result = new FileExistsReply();

            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);

            List<Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = null;
            try
            {
                fileDetailsList = _sharePointFileManager.GetFileDetailsListInFolder(GetDocumentTemplateUrlPart(request.EntityName), request.FolderName, request.DocumentType).GetAwaiter().GetResult();
                if (fileDetailsList != null)

                {
                    var hasFile = fileDetailsList.Any(f => f.ServerRelativeUrl == request.ServerRelativeUrl);

                    if (hasFile)
                    {
                        result.ResultStatus = FileExistStatus.Exist;
                    }
                    else
                    {
                        result.ResultStatus = FileExistStatus.NotExist;
                    }
                    
                }
            }
            catch (SharePointRestException spre)
            {
                _logger.LogError(spre, "Error determining if file exists");
                result.ResultStatus = result.ResultStatus = FileExistStatus.Error;
                result.ErrorDetail = "Error determining if file exists";
            }

            catch (Exception e)
            {
                result.ResultStatus =  FileExistStatus.Error;
                result.ErrorDetail = $"Error determining if file exists";
                _logger.LogError(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        private string GetDocumentListTitle(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = SharePointFileManager.DefaultDocumentListTitle;
                    break;
                case "application":
                    listTitle = SharePointFileManager.ApplicationDocumentListTitle;
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = SharePointFileManager.WorkertDocumentListTitle;
                    break;
                default:
                    break;
            }
            return listTitle;
        }

        private string GetDocumentTemplateUrlPart(string entityName)
        {
            var listTitle = "";
            switch (entityName.ToLower())
            {
                case "account":
                    listTitle = SharePointFileManager.DefaultDocumentListTitle;
                    break;
                case "application":
                    listTitle = "adoxio_application";
                    break;
                case "contact":
                    listTitle = SharePointFileManager.ContactDocumentListTitle;
                    break;
                case "worker":
                    listTitle = "adoxio_worker";
                    break;
                default:
                    break;
            }
            return listTitle;
        }

        private void CreateDocumentLibraryIfMissing(string listTitle, string documentTemplateUrl = null)
        {
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
            var exists = _sharePointFileManager.DocumentLibraryExists(listTitle).GetAwaiter().GetResult();
            if (!exists)
            {
                _sharePointFileManager.CreateDocumentLibrary(listTitle, documentTemplateUrl).GetAwaiter().GetResult();
            }
        }

        public override Task<DeleteFileReply> DeleteFile(DeleteFileRequest request, ServerCallContext context)
        {
            var result = new DeleteFileReply();

            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);

            try
            {
                bool success = _sharePointFileManager.DeleteFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

                if (success)
                {
                    result.ResultStatus = ResultStatus.Success;
                }
                else
                {
                    result.ResultStatus = ResultStatus.Fail;

                }

            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in deleting file {request.ServerRelativeUrl}";
                _logger.LogError(ex, result.ErrorDetail);

            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in deleting file {request.ServerRelativeUrl}";
                _logger.LogError(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<DownloadFileReply> DownloadFile(DownloadFileRequest request, ServerCallContext context)
        {
            var result = new DownloadFileReply();

            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);

            try
            {
                byte[] data = _sharePointFileManager.DownloadFile(request.ServerRelativeUrl).GetAwaiter().GetResult();

                if (data != null)
                {
                    result.ResultStatus = ResultStatus.Success;
                    result.Data = ByteString.CopyFrom(data);
                }
                else
                {
                    result.ResultStatus = ResultStatus.Fail;

                }

            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in downloading file {request.ServerRelativeUrl}";
                _logger.LogError(ex, result.ErrorDetail);

            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in downloading file {request.ServerRelativeUrl}";
                _logger.LogError(e, result.ErrorDetail);
            }


            return Task.FromResult(result);
        }

        public override Task<UploadFileReply> UploadFile(UploadFileRequest request, ServerCallContext context)
        {
            var result = new UploadFileReply();
            

            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);

            CreateDocumentLibraryIfMissing(GetDocumentListTitle(request.EntityName), GetDocumentTemplateUrlPart(request.EntityName));

            try
            {
                string fileName = _sharePointFileManager.AddFile(GetDocumentTemplateUrlPart(request.EntityName), 
                    request.FolderName, 
                    request.FileName, 
                    request.Data.ToByteArray(), request.ContentType).GetAwaiter().GetResult();
                
                result.FileName = fileName;
                result.ResultStatus = ResultStatus.Success;
            }
            catch (SharePointRestException ex)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in uploading file {request.FileName} to folder {request.FolderName}";
                _logger.LogError(ex, result.ErrorDetail);
                
            }
            catch (Exception e)
            {
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = $"ERROR in uploading file {request.FileName} to folder {request.FolderName}";
                _logger.LogError(e, result.ErrorDetail);
            }

            return Task.FromResult(result);
        }

        public override Task<FolderFilesReply> FolderFiles(FolderFilesRequest request, ServerCallContext context)
        {
            var result = new FolderFilesReply();

            
            // Get the file details list in folder
            List<Interfaces.SharePointFileManager.FileDetailsList> fileDetailsList = null;
            SharePointFileManager _sharePointFileManager = new SharePointFileManager(_configuration);
            try
            {
                fileDetailsList = _sharePointFileManager.GetFileDetailsListInFolder(GetDocumentTemplateUrlPart(request.EntityName), request.FolderName, request.DocumentType).GetAwaiter().GetResult();
                if (fileDetailsList != null)

                {
                    // gRPC ensures that the collection has space to accept new data; no need to call a constructor
                    foreach (var item in fileDetailsList)
                    {
                        FileSystemItem newItem = new FileSystemItem()
                        {
                             DocumentType = item.DocumentType,
                             Name = item.Name,
                             ServerRelativeUrl = item.ServerRelativeUrl,
                             Size = int.Parse (item.Length)
                        };

                        result.Files.Add(newItem);
                                              
                    }
                    result.ResultStatus = ResultStatus.Success;
                }
                

            }
            catch (SharePointRestException spre)
            {
                _logger.LogError(spre, "Error getting SharePoint File List");
                result.ResultStatus = ResultStatus.Fail;
                result.ErrorDetail = "Error getting SharePoint File List";
            }

            return Task.FromResult(result);
        }

        [AllowAnonymous]
        public override Task<TokenReply> GetToken(TokenRequest request, ServerCallContext context)
        {
            TokenReply result = new TokenReply();
            result.ResultStatus = ResultStatus.Fail;

            string configuredSecret = _configuration["JWT_TOKEN_KEY"];
            if (configuredSecret.Equals(request.Secret))
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuredSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["JWT_VALID_ISSUER"],
                    _configuration["JWT_VALID_AUDIENCE"],
                    expires: DateTime.UtcNow.AddYears(5),
                    signingCredentials: creds
                    );                
                result.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                result.ResultStatus = ResultStatus.Success;
            }
            else
            {
                result.ErrorDetail = "Bad Request";
            }
            return Task.FromResult(result);
        }
    }
}
