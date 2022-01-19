using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Virtuesoft.Framework.Gateaway.Sample.Data;

public class MemberAccount
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long No { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [Key,StringLength(36)]
    public string ID { get; set; }=Guid.NewGuid().ToString("N");
    /// <summary>
    /// 
    /// </summary>
    [StringLength(12)]
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [StringLength(20)]
    public string Phone { get; set; } = $"+86{DateTimeOffset.Now.ToUnixTimeSeconds()}";

    public virtual Role Role { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Age { get; set; } = new Random().Next(20, 35);
    /// <summary>
    /// 
    /// </summary>
    public bool Status { get; set; } = true;
    public DateTime CreateTime { get; set; } = DateTime.Now;
    [Timestamp]
    [Comment("并发版本")]
    public byte[] Version
    {
        get;
        set;
    }
}

