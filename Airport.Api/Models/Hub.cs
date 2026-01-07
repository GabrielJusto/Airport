
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using NetTopologySuite.Geometries;

namespace Airport.Api.Models;

public class Hub
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("hubId")]
    public int HubId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    [Column(TypeName = "geography (point)")]
    public Point Location { get; set; } = null!;

    public virtual ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();

}