using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleCalculator;

public interface IAppThemeService
{
    bool IsDark { get; }

    ValueTask SetThemeAsync(bool darkMode, CancellationToken ct);
}