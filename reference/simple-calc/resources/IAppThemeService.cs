using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleCalculator.ThemeService;

public interface IAppThemeService
{
    bool IsDark { get; }
    ValueTask SetThemeAsync(bool darkMode, CancellationToken ct);
}
