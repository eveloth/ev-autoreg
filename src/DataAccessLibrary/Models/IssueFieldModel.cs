using System.Data.Common;

namespace DataAccessLibrary.Models;

public class IssueFieldModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string FieldName { get; set; }

#pragma warning restore
}