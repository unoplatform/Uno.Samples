param([int]$AppPid)
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public class Win32Helper {
  [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
'@
$p = Get-Process -Id $AppPid -ErrorAction SilentlyContinue
if ($p -and $p.MainWindowHandle -ne 0) {
  [Win32Helper]::ShowWindow($p.MainWindowHandle, 3) | Out-Null
  Write-Output "Maximized"
} else { Write-Output "No window" }
