﻿namespace VulnerableApp4APISecurity.Core.DTO.Card;

public class CardCreateRequest
{
    public string? Nickname { get; set; }
    public string? Number { get; set; }
    public string? ExpireDate { get; set; }
    public string? Cve { get; set; }
    public string? Password { get; set; }
}