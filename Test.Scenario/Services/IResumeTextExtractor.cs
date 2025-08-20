namespace Test.Scenario.Services
{
    public interface IResumeTextExtractor
    {
        Task<string?> ExtractAsync(string contentType, Stream fileStream, string fileName);
    }
}
