using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInvoices() => new NotFoundResult();

        [HttpGet("{id}")]
        public IActionResult GetInvoice(string id) => new NotFoundResult();

        [HttpPost]
        public IActionResult CreateInvoice() => new NotFoundResult();

        [HttpPut("{id}")]
        public IActionResult UpdateInvoice(string id) => new NotFoundResult();

        [HttpPost("{id}/delete")]
        public IActionResult DeleteInvoice(string id) => new NotFoundResult();
    }
}
