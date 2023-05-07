using MySpot.Application.Services;
using MySpot.Core.Abstractions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MySpot.api")]
namespace MySpot.Infrastructure.Time;

public sealed class Clock : IClock
{
    public DateTime Current() => DateTime.UtcNow;
}
