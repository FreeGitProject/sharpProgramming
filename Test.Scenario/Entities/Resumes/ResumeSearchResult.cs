namespace Test.Scenario.Entities.Resumes
{
    public class ResumeSearchResult
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = default!;
        public DateTime UploadedAt { get; set; }
        public string? CandidateName { get; set; }
        public string? Email { get; set; }
        public int Rank { get; set; }
    }
}
