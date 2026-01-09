
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airport.Api.Models;

public class GateEventType
{
    [Key]
    [Column("gateEventTypeId")]
    public int GateEventTypeId { get; set; }

    [Column("description")]
    public string Description { get; set; } = null!;
}