
using Microsoft.EntityFrameworkCore;

namespace Airport.Api.Database;

public class AirportDbContext(DbContextOptions<AirportDbContext> options) : DbContext(options)
{

}