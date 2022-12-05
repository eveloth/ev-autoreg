namespace Autoregistrar.Services;

public interface IIssueProcessor
{
    Task ProcessEvent(string issueNo);
}