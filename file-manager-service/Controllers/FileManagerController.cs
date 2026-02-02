using System;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Services.FileManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Services.FileManager.Controllers
{
    /// <summary>
    /// This controller is a HTTP compatible wrapper around the FileManagerService gRPC service.
    /// This controller is not the source of truth, and should only ever reflect the methods
    /// available on the actual gRPC service (See FileManagerService.cs and file-manager.proto).
    /// </summary>
    [ApiController]
    [Route("api")]
    public class FileManagerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileManagerController> _logger;

        public FileManagerController(IConfiguration configuration, ILogger<FileManagerController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.GetToken(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    token = result.Token,
                    errorDetail = result.ErrorDetail
                }
            );
        }

        [HttpPost("create-folder")]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.CreateFolder(request, null);

            return Ok(new { resultStatus = result.ResultStatus.ToString(), errorDetail = result.ErrorDetail });
        }

        [HttpPost("folder-files")]
        public async Task<IActionResult> FolderFiles([FromBody] FolderFilesRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.FolderFiles(request, null);

            return Ok(result);
        }

        [HttpPost("delete-file")]
        public async Task<IActionResult> DeleteFile([FromBody] DeleteFileRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.DeleteFile(request, null);

            return Ok(new { resultStatus = result.ResultStatus.ToString(), errorDetail = result.ErrorDetail });
        }

        [HttpPost("download-file")]
        public async Task<IActionResult> DownloadFile([FromBody] DownloadFileRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.DownloadFile(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    data = result.Data.ToBase64(),
                    errorDetail = result.ErrorDetail
                }
            );
        }

        [HttpPost("file-exists")]
        public async Task<IActionResult> FileExists([FromBody] FileExistsRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.FileExists(request, null);

            return Ok(new { resultStatus = result.ResultStatus.ToString(), errorDetail = result.ErrorDetail });
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromBody] UploadFileDto dto)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);

            var request = new UploadFileRequest
            {
                EntityName = dto.EntityName,
                FolderName = dto.FolderName,
                FileName = dto.FileName,
                ContentType = dto.ContentType,
                Data = Google.Protobuf.ByteString.CopyFrom(Convert.FromBase64String(dto.Data))
            };

            var result = await service.UploadFile(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    fileName = result.FileName,
                    errorDetail = result.ErrorDetail
                }
            );
        }

        // DTO for JSON deserialization
        public class UploadFileDto
        {
            public string EntityName { get; set; }
            public string FolderName { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public string Data { get; set; } // base64 string
        }

        [HttpPost("truncated-filename")]
        public async Task<IActionResult> GetTruncatedFilename([FromBody] TruncatedFilenameRequest request)
        {
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.GetTruncatedFilename(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    fileName = result.FileName,
                    errorDetail = result.ErrorDetail
                }
            );
        }
    }
}
