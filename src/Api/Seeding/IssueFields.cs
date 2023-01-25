using Api.Domain;
using DataAccessLibrary.Models;

namespace Api.Seeding;

public class IssueFields
{
    private static List<IssueFieldModel> GetIssueFields()
    {
        var issueFieldNames = new[]
        {
            "Author",
            "Company",
            "Priority",
            "ShortDescription",
            "Description"
        };

        return issueFieldNames
            .Select(fieldName => new IssueFieldModel { FieldName = fieldName })
            .ToList();
    }

    public static List<IssueFieldModel> DefaultIssueFileds => GetIssueFields();
}