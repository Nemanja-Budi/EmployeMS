using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System.Net.Mail;
using System.Net;

namespace ADMitroSremEmploye.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly string _pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedPdfs");

        public PdfController()
        {
            if (!Directory.Exists(_pdfDirectory))
            {
                Directory.CreateDirectory(_pdfDirectory);
            }
        }

        [HttpPost("generate-for-banks")]
        public IActionResult GenerateBankPdf([FromBody] HtmlContentRequest request)
        {
            var htmlContent = request.HtmlContent;

            var pdf = new HtmlToPdf();
            var doc = pdf.ConvertHtmlString(htmlContent);

            using (var memoryStream = new MemoryStream())
            {
                doc.Save(memoryStream);
                return File(memoryStream.ToArray(), "application/pdf", request.FileName);
            }
        }

        [HttpPost("generate")]
        public IActionResult GeneratePdf([FromBody] HtmlContentRequest request)
        {
            var htmlContent = request.HtmlContent;

            var pdf = new HtmlToPdf();
            var doc = pdf.ConvertHtmlString(htmlContent);

            using (var memoryStream = new MemoryStream())
            {
                doc.Save(memoryStream);
                var fileBytes = memoryStream.ToArray();
                var filePath = Path.Combine(_pdfDirectory, request.FileName);

                System.IO.File.WriteAllBytes(filePath, fileBytes);

                // Vratite putanju i ime datoteke
                return Ok(new { filePath = filePath, fileName = request.FileName });
            }
        }

        [HttpPost("send")]
        public IActionResult SendPdf([FromBody] SendPdfRequest request)
        {
            try
            {
                //var filePath = request.FilePath;
                var fileName = request.FileName;
                var filePath = Path.Combine(_pdfDirectory, fileName);
                var email = request.Email;

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { error = "File not found." });
                }

                // Ukloni .pdf ekstenziju iz imena datoteke
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                // Podeli ime datoteke na delove prema razmacima
                string[] parts = fileNameWithoutExtension.Split(new[] { ' ' }, 3);

                if (parts.Length >= 3)
                {
                    // Deo od početka do drugog razmaka
                    string firstPart = $"{parts[0]} {parts[1]}";
                    // Deo od drugog razmaka do kraja
                    string secondPart = parts[2];

                    var mailMessage = new MailMessage(SD.SalaryMail, email)
                    {
                        Subject = fileNameWithoutExtension,
                        Body = $"Poštovani,\n\n\"{firstPart}\" u prilogu možete pronaći platu za \"{secondPart}\".\n\nS poštovanjem,\nVaša firma",
                        IsBodyHtml = false // Postavi na true ako želiš HTML format
                    };

                    // Koristi using blok da osiguraš da Attachment bude zatvoren pre nego što obrišeš datoteku
                    using (var attachment = new Attachment(filePath))
                    {
                        mailMessage.Attachments.Add(attachment);

                        using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                        {
                            smtpClient.Port = 587;
                            smtpClient.Credentials = new NetworkCredential(SD.SalaryMail, SD.SalaryPassword); // Koristi App Password za Gmail
                            smtpClient.EnableSsl = true;
                            smtpClient.Send(mailMessage);
                        }
                    }

                    // Obriši datoteku nakon slanja
                    System.IO.File.Delete(filePath);

                    return Ok(new { message = "Email sent successfully." });
                }
                else
                {
                    return BadRequest(new { error = "File name does not have enough parts." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to send email.", details = ex.Message });
            }
        }

        [HttpGet("list")]
        public IActionResult ListPdfs()
        {
            var folderPath = Path.Combine(_pdfDirectory);
            var pdfFiles = Directory.GetFiles(_pdfDirectory, "*.pdf")
                                    .Select(filePath => new
                                    {
                                        Name = Path.GetFileName(filePath),
                                        Url = Url.Content($"{folderPath}/{Path.GetFileName(filePath)}") // Ažuriraj putanju prema potrebi
                                    })
                                    .ToList();

            return Ok(pdfFiles);
        }

        [HttpGet("item/{fileName}")]
        public IActionResult GetSinglePdf(string fileName)
        {
            var filePath = Path.Combine(_pdfDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "File not found." });
            }

            var pdfFile = new
            {
                Name = Path.GetFileName(filePath),
                Url = Url.Content($"{Request.Scheme}://{Request.Host}/api/pdf/pdf/{Path.GetFileName(filePath)}")
            };

            return Ok(pdfFile);
        }

        [HttpGet("get-pdf/{fileName}")]
        public IActionResult GetPdf(string fileName)
        {
            var filePath = Path.Combine(_pdfDirectory, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/pdf";
            return File(fileBytes, contentType, fileName);
        }

        [HttpDelete("delete-pdf/{fileName}")]
        public IActionResult DeletePdf(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_pdfDirectory, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File not found." });
                }

                System.IO.File.Delete(filePath);

                return Ok(new { message = $"File '{fileName}' deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the file.", details = ex.Message });
            }
        }

        public class HtmlContentRequest
        {
            public string HtmlContent { get; set; }
            public string FileName { get; set; }
        }

        public class SendPdfRequest
        {
            //public string FilePath { get; set; }
            public string FileName { get; set; }
            public string Email { get; set; }
        }
    }
}