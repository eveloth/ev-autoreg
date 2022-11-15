﻿namespace DataAccessLibrary.DisplayModels;

public class UserProfile
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public Role? Role { get; set; } = null;

#pragma warning restore
}
