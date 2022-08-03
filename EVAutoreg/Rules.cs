using Microsoft.Extensions.Configuration;

namespace EVAutoreg
{
    public class Rules
    {
        public string[] SubjectRules { get; init; }
        public string[] BodyRules { get; init; }
        public string[] SubjectNegativeRules { get; init; }
        public string[] BodyNegativeRules { get; init; }
        public string[] ExternalITSubjectRules { get; init; }
        public string[] ExternalITBodyRules { get; init; }

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
        }
        
    }
}
