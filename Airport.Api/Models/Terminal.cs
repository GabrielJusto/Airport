using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Airport.Api.Models;

public class Terminal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("terminalId")]
    public int TerminalId { get; set; }

    [Column("hubId")]
    public int HubId { get; set; }
    public virtual Hub Hub { get; set; } = null!;
    public virtual ICollection<Gate> Gates { get; set; } = new List<Gate>();
}