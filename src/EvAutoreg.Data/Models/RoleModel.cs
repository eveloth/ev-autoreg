﻿namespace EvAutoreg.Data.Models;

public class RoleModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string RoleName { get; set; }
    public bool IsPrivelegedRole { get; set; }

#pragma warning restore
}