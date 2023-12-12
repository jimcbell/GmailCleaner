using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Entities;

[Table("GCUserToken")]
public partial class GCUserToken
{
    [Key]
    public int UserTokenId { get; set; }

    public int UserId { get; set; }
    [EncryptColumn]
    public string? AccessToken { get; set; }
    [EncryptColumn]
    public string? RefreshToken { get; set; }
    [EncryptColumn]
    public string? IdToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("GCUserTokens")]
    public virtual GCUser User { get; set; } = null!;
}
