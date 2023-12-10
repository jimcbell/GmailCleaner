using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailCleaner.Common.Models;
[Table("GCMessage")]
public partial class GCMessage
{
    [Key]
    public int MessageId { get; set; }  
    public string MessageGmailId { get; set; } = null!;
    public string Snippet { get; set; } = null!;
    public string UnsubscribeLink { get; set; } = null!;    
    public string From { get; set; } = null!;
    [ForeignKey("UserId")]
    [InverseProperty("GCMessages")]
    public virtual GCUser User { get; set; } = null!;

}
