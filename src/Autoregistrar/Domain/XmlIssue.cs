using System.Xml.Serialization;

namespace Autoregistrar.Domain;

[XmlRoot("PROBLEM_RECORD")]
public class XmlIssue
{
    [XmlElement("ID")]
    public string Id { get; set; }

    [XmlElement("DATE_CREATED")]
    public string? TimeCreated { get; set; }

    [XmlElement("ORIGINATOR")]
    public string Author { get; set; }

    [XmlElement("CTI_CUST_LIST")]
    public string Company { get; set; }

    [XmlElement("STATUS")]
    public string Status { get; set; }

    [XmlElement("PRIORITY")]
    public string Priority { get; set; }

    [XmlElement("CTI_ASSIGNEDGROUP")]
    public string? AssignedGroup { get; set; }

    [XmlElement("ASSIGNED_TO")]
    public string? Assignee { get; set; }

    [XmlElement("SHORT_DESCR")]
    public string ShortDescription { get; set; }

    [XmlElement("DESCRIPTION")]
    public string Description { get; set; }
}