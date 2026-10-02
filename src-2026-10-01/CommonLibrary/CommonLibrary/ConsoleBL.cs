using System;
using System.Runtime.InteropServices;

namespace CommonLibrary;

public static class ConsoleBL
{
	private static readonly IntPtr HWND_TOP = IntPtr.Zero;

	private const uint SWP_NOSIZE = 1u;

	private const uint SWP_SHOWWINDOW = 64u;

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	public static void MoveConsole(int x = 100, int y = 50, int width = 1400, int height = 650)
	{
		IntPtr consoleHandle = GetConsoleWindow();
		if (consoleHandle == IntPtr.Zero)
		{
			Console.WriteLine("Не удалось получить дескриптор окна консоли.");
		}
		else if (!MoveWindow(consoleHandle, x, y, width, height, bRepaint: true))
		{
			Console.WriteLine("MoveWindow вернул ошибку: " + Marshal.GetLastWin32Error());
		}
	}

	public static void SetPosConsole(int x = 100, int y = 50)
	{
		IntPtr consoleHandle = GetConsoleWindow();
		if (consoleHandle == IntPtr.Zero)
		{
			Console.WriteLine("Не удалось получить дескриптор окна консоли.");
		}
		else
		{
			SetWindowPos(consoleHandle, HWND_TOP, x, y, 0, 0, 65u);
		}
	}
}
