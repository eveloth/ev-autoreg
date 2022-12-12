﻿namespace Api.Contracts.Dto;

public class IssueDto
{
    public required int Id { get; set; }
    public required DateTime TimeCreated { get; set; }
    public required string Author { get; set; }
    public required string Company { get; set; }
    public required string Status { get; set; }
    public required string Priority { get; set; }
    public string? AssignedGroup { get; set; }
    public string? Assignee { get; set; }
    public required string ShortDescription { get; set; }
    public required string Description { get; set; }
    public required int RegistrarId { get; set; }
    public required string RegistrarFirstName { get; set; }
    public required string RegistrarLastName { get; set; }
    public IssueTypeDto? IssueType { get; set; }
}