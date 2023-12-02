using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.EFCore.Models;

public partial class User
{
    [Key]
    public Guid Id { get; set; }

    public string GmailId { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public string? Name { get; set; }

    public DateTime? AccessTokenExpiresAt { get; set; }
}
