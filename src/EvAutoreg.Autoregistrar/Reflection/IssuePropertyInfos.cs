using System.Reflection;

namespace EvAutoreg.Autoregistrar.Reflection;

public class IssuePropertyInfos
{
    public IssuePropertyInfos(PropertyInfo[] xmlIssueOptionsProps, PropertyInfo[] xmlIssueProps)
    {
        XmlIssueOptionsProps = xmlIssueOptionsProps;
        XmlIssueProps = xmlIssueProps;
    }

    public PropertyInfo[] XmlIssueOptionsProps { get; }
    public PropertyInfo[] XmlIssueProps { get; }
}