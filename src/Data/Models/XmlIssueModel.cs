using System.Xml.Serialization;

namespace Data.DataAccess.Models;

[XmlRoot("PROBLEM_RECORD")]
public class XmlIssueModel
{
    public int Id { get; set; }
    [XmlElement("ID")] public string IssueNo { get; set; }
    [XmlElement("DATE_CREATED")] public string DateCreated { get; set; }
    [XmlElement("ORIGINATOR")] public string Author { get; set; } = string.Empty;
    [XmlElement("CTI_CUST_LIST")] public string Company { get; set; } = string.Empty;
    [XmlElement("STATUS")] public string Status { get; set; }
    [XmlElement("PRIORITY")] public string Priority { get; set; }
    [XmlElement("CTI_ASSIGNEDGROUP")] public string AssignedGroup { get; set; } = string.Empty;
    [XmlElement("ASSIGNED_TO")] public string Assignee { get; set; } = string.Empty;
    [XmlElement("SHORT_DESCR")] public string Description { get; set; }
}