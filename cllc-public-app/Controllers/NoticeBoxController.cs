using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    public class NoticeBoxModel
    {
        public bool noticeBoxEnabled { get; set; }
        public string noticeBoxTitle { get; set; }
        public string noticeBoxText { get; set; }
    }

    [Route("api/noticebox")]
    [ApiController]
    [AllowAnonymous]

    public class NoticeBoxController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public NoticeBoxController(IConfiguration configuration, ILogger<NoticeBoxController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetNoticeBox()
        {
            try
            {
                NoticeBoxModel noticeBox = new NoticeBoxModel
                {
                    noticeBoxEnabled = _configuration.GetValue<bool>("NOTICEBOX_ENABLED", false),
                    noticeBoxTitle = _configuration.GetValue<string>("NOTICEBOX_TITLE", ""),
                    noticeBoxText = _configuration.GetValue<string>("NOTICEBOX_TEXT", "")
                };
                return Ok(noticeBox);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving notice box configuration");
                return StatusCode(500, "Unable to retrieve notice box configuration");
            }
        }
    }
}
