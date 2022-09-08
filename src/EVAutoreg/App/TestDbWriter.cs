using System.Text.RegularExpressions;
using EVAutoreg.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace EVAutoreg.App
{
    public class TestDbWriter : IMailEventListener
    {
        private readonly EVApiWrapper _evapi;
        private readonly Rules _rules;

        public TestDbWriter(Rules rules, EVApiWrapper evapi)
        {
            _rules = rules;
            _evapi = evapi;
        }

        public async Task ProcessEvent(EmailMessage email)
        {
            var subject = email.Subject;
            var issueNo = Regex.Match(subject, _rules.RegexIssueNo).Groups[1].Value;

            await _evapi.GetIssue(issueNo);
        }
    }
}
