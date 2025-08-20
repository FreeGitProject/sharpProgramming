using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Test.Scenario.Data;
using Test.Scenario.Entities.Resumes;
using Test.Scenario.Services;

namespace Test.Scenario.Controllers
{
    [ApiController]
    [Route("api/resumes")]
    public class ResumesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IResumeTextExtractor _extractor;

        public ResumesController(AppDbContext db, IResumeTextExtractor extractor)
        {
            _db = db;
            _extractor = extractor;
        }

        [HttpPost]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<IActionResult> Upload([FromForm] IFormFile file,
                                                [FromForm] string? candidateName,
                                                [FromForm] string? email)
        {
            if (file is null || file.Length == 0) return BadRequest("No file.");

            await using var stream = file.OpenReadStream();
            var text = await _extractor.ExtractAsync(file.ContentType, stream, file.FileName);

            // read bytes (reset stream first)
            stream.Position = 0;
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var resume = new Resume
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileBytes = bytes,
                ExtractedText = text,
                CandidateName = candidateName,
                Email = email
            };

            _db.Resumes.Add(resume);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMeta), new { id = resume.Id }, new { resume.Id });
        }

        // Minimal listing item (no bytes!)
        public record ResumeListItem(Guid Id, string FileName, DateTime UploadedAt, string? CandidateName, string? Email);

        [HttpGet("{id}")]
        public async Task<ActionResult<ResumeListItem>> GetMeta(Guid id)
        {
            var r = await _db.Resumes
                .Where(x => x.Id == id)
                .Select(x => new ResumeListItem(x.Id, x.FileName, x.UploadedAt, x.CandidateName, x.Email))
                .FirstOrDefaultAsync();

            return r is null ? NotFound() : Ok(r);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResumeListItem>>> List([FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);

            var items = await _db.Resumes
                .OrderByDescending(r => r.UploadedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(r => new ResumeListItem(r.Id, r.FileName, r.UploadedAt, r.CandidateName, r.Email))
                .ToListAsync();

            return Ok(items);
        }
        //Full-Text Search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ResumeSearchResult>>> Search(
    [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query required.");
            page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);

            // Use CONTAINSTABLE for ranking. Parameterized to avoid injection.
            var skip = (page - 1) * size;
            var sql = $@"
SELECT r.Id, r.FileName, r.UploadedAt, r.CandidateName, r.Email, ft.[RANK] AS Rank
FROM dbo.Resumes r
JOIN CONTAINSTABLE(dbo.Resumes, ExtractedText, @p0) ft ON ft.[KEY] = r.Id
ORDER BY ft.[RANK] DESC
OFFSET {skip} ROWS FETCH NEXT {size} ROWS ONLY;";

            var results = await _db.ResumeSearchResults
                .FromSqlRaw(sql, q)        // @p0 is bound to 'q'
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }
        //Here are several alternative approaches since you can't use Full-Text Search:
        //Alternative 1: LIKE-Based Search with Pagination
        //
        [HttpGet("searchv1")]
        public async Task<ActionResult<IEnumerable<ResumeSearchResult>>> SearchV1(
    [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query required.");
            page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);

            var skip = (page - 1) * size;

            // Basic LIKE search with ranking based on occurrence count
            var sql = @"
SELECT 
    r.Id, 
    r.FileName, 
    r.UploadedAt, 
    r.CandidateName, 
    r.Email,
    CAST((LEN(r.ExtractedText) - LEN(REPLACE(r.ExtractedText, @p0, ''))) / LEN(@p0) AS INT) AS Rank
FROM dbo.Resumes r
WHERE r.ExtractedText LIKE '%' + @p0 + '%'
ORDER BY Rank DESC
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY;";

            var results = await _db.ResumeSearchResults
                .FromSqlRaw(sql,
                    new SqlParameter("@p0", q),
                    new SqlParameter("@skip", skip),
                    new SqlParameter("@size", size))
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        //Alternative 2: Advanced LIKE Search with Multiple Terms
        [HttpGet("searchv2")]
        public async Task<ActionResult<IEnumerable<ResumeSearchResult>>> SearchV2(
    [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query required.");
            page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);

            var skip = (page - 1) * size;
            var searchTerms = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var sqlBuilder = new StringBuilder(@"
SELECT 
    r.Id, 
    r.FileName, 
    r.UploadedAt, 
    r.CandidateName, 
    r.Email,
    (");

            // Build ranking formula based on term occurrences
            for (int i = 0; i < searchTerms.Length; i++)
            {
                if (i > 0) sqlBuilder.Append(" + ");
                sqlBuilder.Append($"(LEN(r.ExtractedText) - LEN(REPLACE(r.ExtractedText, @term{i}, ''))) / LEN(@term{i})");
            }

            sqlBuilder.Append(@") AS Rank
FROM dbo.Resumes r
WHERE (");

            // Build WHERE clause for all terms
            for (int i = 0; i < searchTerms.Length; i++)
            {
                if (i > 0) sqlBuilder.Append(" OR ");
                sqlBuilder.Append($"r.ExtractedText LIKE '%' + @term{i} + '%'");
            }

            sqlBuilder.Append(@")
ORDER BY Rank DESC
OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY;");

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@skip", skip),
        new SqlParameter("@size", size)
    };

            for (int i = 0; i < searchTerms.Length; i++)
            {
                parameters.Add(new SqlParameter($"@term{i}", searchTerms[i]));
            }

            var results = await _db.ResumeSearchResults
                .FromSqlRaw(sqlBuilder.ToString(), parameters.ToArray())
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        //Alternative 3: Application-Level Search (Better for Large Datasets)
        [HttpGet("searchv3")]
        public async Task<ActionResult<IEnumerable<ResumeSearchResult>>> SearchV3(
    [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query required.");
            page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);

            var skip = (page - 1) * size;

            // Get all resumes (or use pagination at database level)
            var allResumes = await _db.Resumes
                .AsNoTracking()
                .Where(r => !string.IsNullOrEmpty(r.ExtractedText))
                .ToListAsync();

            // Application-level search with ranking
            var results = allResumes
                .Select(r => new
                {
                    Resume = r,
                    Rank = CalculateRank(r.ExtractedText, q)
                })
                .Where(x => x.Rank > 0)
                .OrderByDescending(x => x.Rank)
                .Skip(skip)
                .Take(size)
                .Select(x => new ResumeSearchResult
                {
                    Id = x.Resume.Id,
                    FileName = x.Resume.FileName,
                    UploadedAt = x.Resume.UploadedAt,
                    CandidateName = x.Resume.CandidateName,
                    Email = x.Resume.Email,
                    Rank = x.Rank
                })
                .ToList();

            return Ok(results);
        }

        //Alternative 4: Hybrid Approach (Database Filter + Application Ranking)


        [HttpGet("searchv4")]
        public async Task<ActionResult<IEnumerable<ResumeSearchResult>>> SearchV4(
    [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int size = 20)
        {
            if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query required.");
            page = Math.Max(1, page); size = Math.Clamp(size, 1, 100);

            var skip = (page - 1) * size;
            var searchTerms = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // First, get IDs that match any term
            var query = _db.Resumes.AsQueryable();

            foreach (var term in searchTerms)
            {
                var currentTerm = term;
                query = query.Where(r => r.ExtractedText.Contains(currentTerm));
            }

            var matchingResumes = await query
                .Select(r => new { r.Id, r.ExtractedText })
                .ToListAsync();

            // Then rank in application layer
            var rankedResults = matchingResumes
                .Select(r => new
                {
                    Id = r.Id,
                    Rank = CalculateRank(r.ExtractedText, q)
                })
                .OrderByDescending(x => x.Rank)
                .Skip(skip)
                .Take(size)
                .ToList();

            // Finally, get full details for top results
            var resultIds = rankedResults.Select(x => x.Id).ToList();
            var finalResults = await _db.Resumes
                .Where(r => resultIds.Contains(r.Id))
                .Select(r => new ResumeSearchResult
                {
                    Id = r.Id,
                    FileName = r.FileName,
                    UploadedAt = r.UploadedAt,
                    CandidateName = r.CandidateName,
                    Email = r.Email,
                    Rank = rankedResults.First(x => x.Id == r.Id).Rank
                })
                .ToListAsync();

            return Ok(finalResults.OrderByDescending(r => r.Rank));
        }
        /*
         Recommendation
        Start with Alternative 1 (simple LIKE search) if your dataset is small. 
        If performance becomes an issue, consider Alternative 4 (hybrid approach) or implement a 
        proper search engine like Elasticsearch or Azure Cognitive Search for production use.
         */

        [HttpGet("{id}/file")]
        public async Task<IActionResult> Download(Guid id)
        {
            var r = await _db.Resumes.FindAsync(id);
            if (r is null) return NotFound();
            return File(r.FileBytes, r.ContentType, r.FileName);
        }
        private int CalculateRank(string text, string searchTerm)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int rank = 0;

            foreach (var term in terms)
            {
                int count = (text.Length - text.Replace(term, "").Length) / term.Length;
                rank += count;
            }

            return rank;
        }

      

    }

}
