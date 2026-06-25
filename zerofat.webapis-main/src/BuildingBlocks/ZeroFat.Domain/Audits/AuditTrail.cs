using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Domain.Audits;
public class AuditTrail
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType UserId { get; set; }
    public string? Operation { get; set; }
    public string? Entity { get; set; }
    public ApplicationModule? Module { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string? PreviousValues { get; set; }
    public string? NewValues { get; set; }
    public string? ModifiedProperties { get; set; }
    public string? PrimaryKey { get; set; }
}
