using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Entities;

[Table("GCUser")]
public partial class GCUser
{
    [Key]
    public int UserId { get; set; }
    [Required]
    public string GmailId { get; set; } = null!;
    [EncryptColumn]
    public string Name { get; set; } = null!;
    [EncryptColumn]
    public string Email { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public int Usages { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<GCUserToken> GCUserTokens { get; } = new List<GCUserToken>();
    [InverseProperty("User")]
    public virtual ICollection<GCMessage> GCMessages { get; } = new List<GCMessage>();
}
