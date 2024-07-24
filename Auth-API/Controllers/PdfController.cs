using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GeneratePdf([FromBody] HtmlContentRequest request)
        {
            var htmlContent = request.HtmlContent;

            var pdf = new HtmlToPdf();
            var doc = pdf.ConvertHtmlString(htmlContent);

            using (var memoryStream = new MemoryStream())
            {
                doc.Save(memoryStream);
                return File(memoryStream.ToArray(), "application/pdf", "employee-salary.pdf");
            }
        }
    }

    public class HtmlContentRequest
    {
        public string HtmlContent { get; set; }
    }
}

