using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsKiller;

public static class Win32
{
    public const int WM_GETTEXT = 0x0D;

    public const int WM_GETTEXTLENGTH = 0x0E;

    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
    public static extern int GetCursorPos(ref Point lpPoint);

    [DllImport("User32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);

    [DllImport("User32.dll", EntryPoint = "SendMessage")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr WindowFromPoint(Point p);

    [DllImport("user32.dll")]
    public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, Point p);

    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "GetClassName", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);
    
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }
}