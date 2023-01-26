namespace EvAutoreg.Autoregistrar.Services.Interfaces;

public interface IIssueProcessor
{
    Task ProcessEvent(string issueNo);
}