using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Common.Models;

[Table("GCUser")]
public partial class GCUser
{
    [Key]
    public int UserId { get; set; }
    [Required]
    public string GmailId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<GCUserToken> GCUserTokens { get; } = new List<GCUserToken>();
}
