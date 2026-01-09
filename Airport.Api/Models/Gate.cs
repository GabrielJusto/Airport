using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Airport.Api.DTOs;

namespace Airport.Api.Models;

public class Gate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("gateId")]
    public int GateId { get; set; }

    [Column("jetways")]
    public int Jetways { get; set; }

    [Column("maxPassengersPerHour")]
    public int MaxPassengersPerHour { get; set; }

    [Column("terminalId")]
    public int TerminalId { get; set; }

    public virtual Terminal Terminal { get; set; } = null!;

    public Gate() { }

    public Gate(GateInsertDto data)
    {
        Jetways = data.Jetways;
        MaxPassengersPerHour = data.MaxPassengersPerHour;
        TerminalId = data.TerminalId;
    }
}