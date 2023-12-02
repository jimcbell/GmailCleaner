using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GmailCleaner.Common.Models;

[Table("GCUserToken")]
public partial class GCUserToken
{
    [Key]
    public int UserTokenId { get; set; }

    public int UserId { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public string? IdToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiresOn { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("GCUserTokens")]
    public virtual GCUser User { get; set; } = null!;
}
