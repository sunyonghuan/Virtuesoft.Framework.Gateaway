using System.ComponentModel.DataAnnotations;

namespace Virtuesoft.Framework.Gateaway.Sample.Data;

public class Role
{
    [Key, StringLength(36)]
    public string ID { get; set; } = Guid.NewGuid().ToString("N");

    [StringLength(12)]
    public string Name { get; set; }

    public DateTime CreateTime { get; set; }=DateTime.Now;

    public virtual ICollection<MemberAccount> Accounts { get; set; }

    [Timestamp]
    [Comment("并发版本")]
    public byte[] Version
    {
        get;
        set;
    }
}

