﻿using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.Models;

public class User
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public List<Character>? Characters { get; set; }
}