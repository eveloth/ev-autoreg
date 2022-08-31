using Microsoft.Extensions.Configuration;

namespace EVAutoreg.App
{
    public class Rules
    {
        public string[] SubjectRules { get; init; }
        public string[] BodyRules { get; init; }
        public string[] SubjectNegativeRules { get; init; }
        public string[] BodyNegativeRules { get; init; }
        public string[] ExternalITSubjectRules { get; init; }
        public string[] ExternalITBodyRules { get; init; }
        public string[] SpamSubjectRules { get; init; }
        public string[] SpamBodyRules { get; init; }
        public string RegexNewIssue { get; init; }
        public string RegexIssueNo { get; init; }
        public string RegexMonitoring { get; init; }
        public string RegexExternalIT { get; init; }

        public Rules(IConfiguration config)
        {
            SubjectRules = config.GetSection("MailAnalysisRules:SubjectRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();
            BodyRules = config.GetSection("MailAnalysisRules:BodyRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();
            SubjectNegativeRules = config.GetSection("MailAnalysisRules:SubjectNegativeRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();
            BodyNegativeRules = config.GetSection("MailAnalysisRules:BodyNegativeRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();
            ExternalITSubjectRules = config.GetSection("MailAnalysisRules:ExternalITSubjectRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();
            ExternalITBodyRules = config.GetSection("MailAnalysisRules:ExternalITBodyRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();
            SpamSubjectRules = config.GetSection("MailAnalysisRules:SpamSubjectRules").Get<string[]>() 
                ?? Array.Empty<string>().ToArray();
            SpamBodyRules = config.GetSection("MailAnalysisRules:SpamBodyRules").Get<string[]>()
                ?? Array.Empty<string>().ToArray();

            RegexNewIssue = config.GetValue<string>("RegexRules:NewIssue")?? string.Empty;
            RegexIssueNo = config.GetValue<string>("RegexRules:IssueNo") ?? string.Empty;
            RegexMonitoring = config.GetValue<string>("RegexRules:Monitoring") ?? string.Empty;
            RegexExternalIT = config.GetValue<string>("RegexRules:ExternalIT") ?? string.Empty;
        }

    }
}
