using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Data.Models;

[XmlRoot("PROBLEM_RECORD")]
public class XmlIssueModel
{
    public int Id { get; set; }

    [XmlElement("ID"), NotNull]
    public string? IssueNo { get; set; }

    [XmlElement("DATE_CREATED"), NotNull]
    public string? DateCreated { get; set; }

    [XmlElement("ORIGINATOR")]
    public string Author { get; set; } = string.Empty;

    [XmlElement("CTI_CUST_LIST")]
    public string Company { get; set; } = string.Empty;

    [XmlElement("STATUS"), NotNull]
    public string? Status { get; set; }

    [XmlElement("PRIORITY"), NotNull]
    public string? Priority { get; set; }

    [XmlElement("CTI_ASSIGNEDGROUP")]
    public string AssignedGroup { get; set; } = string.Empty;

    [XmlElement("ASSIGNED_TO")]
    public string Assignee { get; set; } = string.Empty;

    [XmlElement("SHORT_DESCR"), NotNull]
    public string? Description { get; set; }
}
