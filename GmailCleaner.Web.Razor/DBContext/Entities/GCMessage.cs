using EntityFrameworkCore.EncryptColumn.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailCleaner.Entities;
[Table("GCMessage")]
public partial class GCMessage
{
    [Key]
    public int MessageId { get; set; }
    public string MessageGmailId { get; set; } = null!;
    [EncryptColumn]
    public string Snippet { get; set; } = null!;
    [EncryptColumn]
    public string UnsubscribeLink { get; set; } = null!;
    [EncryptColumn]
    public string From { get; set; } = null!;
    [ForeignKey("UserId")]
    [InverseProperty("GCMessages")]
    public virtual GCUser User { get; set; } = null!;

}
