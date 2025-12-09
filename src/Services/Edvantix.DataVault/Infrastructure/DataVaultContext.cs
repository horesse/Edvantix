using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.DataVault.Infrastructure;

public sealed class DataVaultContext(DbContextOptions<DataVaultContext> options) : PostgresContext(options);
