namespace EvAutoreg.Data.Models;

public class PermissionModel : IEquatable<PermissionModel>
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string PermissionName { get; set; }
    public string Description { get; set; }

#pragma warning restore
    public bool Equals(PermissionModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PermissionName == other.PermissionName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PermissionModel) obj);
    }

    public override int GetHashCode()
    {
        return PermissionName.GetHashCode();
    }
}