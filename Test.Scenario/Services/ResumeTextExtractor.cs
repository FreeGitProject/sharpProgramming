using System.Text;
using Test.Scenario.Services;

public class ResumeTextExtractor : IResumeTextExtractor
{
    public async Task<string?> ExtractAsync(string contentType, Stream fileStream, string fileName)
    {
        // reset position in case stream is reused
        if (fileStream.CanSeek) fileStream.Position = 0;

        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (ext == ".pdf")
        {
            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            ms.Position = 0;
            using var doc = UglyToad.PdfPig.PdfDocument.Open(ms);
            var sb = new StringBuilder();
            foreach (var page in doc.GetPages()) sb.AppendLine(page.Text);
            return sb.ToString();
        }

        if (ext == ".docx")
        {
            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            ms.Position = 0;
            using var wordDoc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(ms, false);
            return wordDoc.MainDocumentPart?.Document?.InnerText;
        }

        if (ext == ".txt")
        {
            using var sr = new StreamReader(fileStream, leaveOpen: true);
            return await sr.ReadToEndAsync();
        }

        // Unknown types: store bytes, but skip text
        return null;
    }
}