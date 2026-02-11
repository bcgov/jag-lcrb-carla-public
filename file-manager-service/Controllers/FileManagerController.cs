using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            Console.WriteLine($"FileManagerController - GetToken - Request: {JsonSerializer.Serialize(request)}");
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
            Console.WriteLine($"FileManagerController - CreateFolder - Request: {JsonSerializer.Serialize(request)}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.CreateFolder(request, null);

            return Ok(new { resultStatus = result.ResultStatus.ToString(), errorDetail = result.ErrorDetail });
        }

        [HttpPost("folder-files")]
        public async Task<IActionResult> FolderFiles([FromBody] FolderFilesRequest request)
        {
            Console.WriteLine($"FileManagerController - FolderFiles - Request: {JsonSerializer.Serialize(request)}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.FolderFiles(request, null);

            return Ok(result);
        }

        [HttpPost("delete-file")]
        public async Task<IActionResult> DeleteFile([FromBody] DeleteFileRequest request)
        {
            Console.WriteLine($"FileManagerController - DeleteFile - Request: {JsonSerializer.Serialize(request)}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.DeleteFile(request, null);

            return Ok(new { resultStatus = result.ResultStatus.ToString(), errorDetail = result.ErrorDetail });
        }

        [HttpPost("download-file")]
        public async Task<IActionResult> DownloadFile([FromBody] DownloadFileRequest request)
        {
            Console.WriteLine($"FileManagerController - DownloadFile - Request: {JsonSerializer.Serialize(request)}");
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
            Console.WriteLine($"FileManagerController - FileExists - Request: {JsonSerializer.Serialize(request)}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.FileExists(request, null);

            return Ok(new { resultStatus = result.ResultStatus.ToString(), errorDetail = result.ErrorDetail });
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile([FromBody] UploadFileDto dto)
        {
            Console.WriteLine($"FileManagerController - UploadFile - Request: EntityName={dto.EntityName}, FolderName={dto.FolderName}, FileName={dto.FileName}, ContentType={dto.ContentType}, DataLength={dto.Data?.Length ?? 0}");
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
            Console.WriteLine($"FileManagerController - GetTruncatedFilename - Request: {JsonSerializer.Serialize(request)}");
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

        [HttpPost("ensure-folder-path")]
        public async Task<IActionResult> EnsureFolderPath([FromBody] EnsureFolderPathDto dto)
        {
            Console.WriteLine($"FileManagerController - EnsureFolderPath - Request: {JsonSerializer.Serialize(dto)}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);

            var request = new EnsureFolderPathRequest { EntityName = dto.EntityName };

            _logger.LogInformation(
                $"EnsureFolderPath Controller: Received request for entity {dto.EntityName} with {dto.FolderPath?.Count ?? 0} segments"
            );

            // Add folder segments
            if (dto.FolderPath != null)
            {
                foreach (var segment in dto.FolderPath)
                {
                    _logger.LogInformation(
                        $"EnsureFolderPath Controller: Segment - FolderNameSegment: '{segment.FolderNameSegment}', FolderGuidSegment: '{segment.FolderGuidSegment}', FolderName: '{segment.FolderName}'"
                    );

                    request.FolderPath.Add(
                        new FolderSegment
                        {
                            FolderNameSegment = segment.FolderNameSegment ?? "",
                            FolderGuidSegment = segment.FolderGuidSegment ?? "",
                            FolderName = segment.FolderName ?? ""
                        }
                    );
                }
            }

            var result = await service.EnsureFolderPath(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    serverRelativeUrl = result.ServerRelativeUrl,
                    errorDetail = result.ErrorDetail
                }
            );
        }

        // DTO for JSON deserialization
        public class EnsureFolderPathDto
        {
            public string EntityName { get; set; }
            public List<FolderSegmentDto> FolderPath { get; set; }
        }

        public class FolderSegmentDto
        {
            public string FolderNameSegment { get; set; }
            public string FolderGuidSegment { get; set; }
            public string FolderName { get; set; }
        }

        [HttpPost("find-folder")]
        public async Task<IActionResult> FindFolder([FromBody] FindFolderRequest request)
        {
            Console.WriteLine($"FileManagerController - FindFolder - Request: {JsonSerializer.Serialize(request)}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);
            var result = await service.FindFolder(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    folders = result.Folders.Select(f => new
                    {
                        name = f.Name,
                        serverRelativeUrl = f.ServerRelativeUrl
                    }),
                    errorDetail = result.ErrorDetail
                }
            );
        }

        [HttpPost("upload-file-with-folder-path")]
        public async Task<IActionResult> UploadFileWithFolderPath([FromBody] UploadFileWithFolderPathDto dto)
        {
            Console.WriteLine($"FileManagerController - UploadFileWithFolderPath - Request: EntityName={dto.EntityName}, FileName={dto.FileName}, ContentType={dto.ContentType}, FolderPathSegments={dto.FolderPath?.Count ?? 0}, DataLength={dto.Data?.Length ?? 0}");
            var service = new FileManagerService(_logger as ILogger<FileManagerService>, _configuration);

            var request = new UploadFileWithFolderPathRequest
            {
                EntityName = dto.EntityName,
                FileName = dto.FileName,
                ContentType = dto.ContentType,
                Data = Google.Protobuf.ByteString.CopyFrom(Convert.FromBase64String(dto.Data))
            };

            _logger.LogInformation(
                $"UploadFileWithFolderPath Controller: Uploading file {dto.FileName} to entity {dto.EntityName} with {dto.FolderPath?.Count ?? 0} folder segments"
            );

            // Add folder segments
            if (dto.FolderPath != null)
            {
                foreach (var segment in dto.FolderPath)
                {
                    _logger.LogInformation(
                        $"UploadFileWithFolderPath Controller: Segment - FolderNameSegment: '{segment.FolderNameSegment}', FolderGuidSegment: '{segment.FolderGuidSegment}', FolderName: '{segment.FolderName}'"
                    );

                    request.FolderPath.Add(
                        new FolderSegment
                        {
                            FolderNameSegment = segment.FolderNameSegment ?? "",
                            FolderGuidSegment = segment.FolderGuidSegment ?? "",
                            FolderName = segment.FolderName ?? ""
                        }
                    );
                }
            }

            var result = await service.UploadFileWithFolderPath(request, null);

            return Ok(
                new
                {
                    resultStatus = result.ResultStatus.ToString(),
                    fileName = result.FileName,
                    serverRelativeUrl = result.ServerRelativeUrl,
                    errorDetail = result.ErrorDetail
                }
            );
        }

        // DTO for JSON deserialization
        public class UploadFileWithFolderPathDto
        {
            public string EntityName { get; set; }
            public List<FolderSegmentDto> FolderPath { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public string Data { get; set; } // base64 string
        }
    }
}
