namespace Test.Scenario.Entities.Resumes
{
    public class Resume
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public byte[] FileBytes { get; set; } = default!;
        public string? ExtractedText { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? CandidateName { get; set; }
        public string? Email { get; set; }
    }
}
