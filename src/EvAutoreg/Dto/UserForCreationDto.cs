﻿using System.Diagnostics.CodeAnalysis;

namespace EvAutoreg.Dto;

public class UserForCreationDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}