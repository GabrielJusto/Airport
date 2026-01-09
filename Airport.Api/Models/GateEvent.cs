using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Api.Models;

public class GateEvent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("gateEventId")]
    public int GateEventId { get; set; }

    [Column("StartDate")]
    public DateTimeOffset StartDate { get; set; }
    [Column("EndDate")]
    public DateTimeOffset EndDate { get; set; }
    [Column("gateId")]
    public int GateId { get; set; }
    [Column("gateEventTypeId")]
    public virtual Gate Gate { get; set; } = null!;
    public int GateEventTypeId { get; set; }
    public virtual GateEventType GateEventType { get; set; } = null!;


}