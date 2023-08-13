using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WindowsKiller;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private bool m_isRuning;
    private string m_title;

    private Process? m_process;

    public MainWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        InitializeComponent();

        m_title = Title;

        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(100);
        timer.Tick += Tick;
        timer.Start();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            OnRunSwitch();
        }
    }

    private void OnRunSwitch()
    {
        m_isRuning = !m_isRuning;

        RunSwitchButton.Content = m_isRuning ? "停止捕获" : "开始捕获";
    }

    private void RunSwitchButton_OnClick(object sender, RoutedEventArgs e) => OnRunSwitch();

    private void ClearButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (m_process == null)
        {
            MessageBox.Show("请先捕获窗口程序");
            return;
        }

        if (m_process.HasExited)
        {
            MessageBox.Show("此窗口信息已过时，请重新捕获窗口程序");
            return;
        }

        var path = m_process.MainModule?.FileName!;
        var result = MessageBox.Show($"窗口程序路径为：\n{path}\n确认要强制终结进程并清理程序文件吗？", "", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                m_process.Kill();
                File.Copy(path, $"{path}.bak", true);
                m_process.Dispose();
                m_process = null;
                
                Thread.Sleep(100);
                File.WriteAllBytes(path, Array.Empty<byte>());

                MessageBox.Show("清理成功");
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}", "清理失败");
            }

            TitleTextBox.Text = m_title;
            IntPtrTextBox.Text = string.Empty;
            ProcessIdTextBox.Text = string.Empty;
            ClassNameTextBox.Text = string.Empty;
            TitleTextBox.Text = string.Empty;
            ExePathTextBox.Text = string.Empty;
        }
    }

    private void Tick(object? sender, EventArgs e)
    {
        if (!m_isRuning) return;

        var point = new Win32.Point();
        _ = Win32.GetCursorPos(ref point);
        Title = $"{m_title} [X={point.X}, Y={point.Y}]";

        var curHwnd = Win32.WindowFromPoint(point);
        Win32.ScreenToClient(curHwnd, ref point);

        var hwnd = Win32.ChildWindowFromPoint(curHwnd, point);
        if (hwnd != IntPtr.Zero)
            curHwnd = hwnd;

        hwnd = Win32.GetParent(curHwnd);
        while (hwnd != IntPtr.Zero)
        {
            curHwnd = hwnd;
            hwnd = Win32.GetParent(curHwnd);
        }

        IntPtrTextBox.Text = curHwnd.ToString("x8");

        var pId = 0;
        _ = Win32.GetWindowThreadProcessId(curHwnd, ref pId);
        ProcessIdTextBox.Text = pId.ToString();

        var len = Win32.SendMessage(curHwnd, Win32.WM_GETTEXTLENGTH, 0, 0);
        var sb = new StringBuilder(len + 1);
        _ = Win32.GetClassName(curHwnd, sb, sb.Capacity);
        ClassNameTextBox.Text = sb.ToString();

        _ = Win32.GetWindowText(curHwnd, sb, sb.Capacity);
        TitleTextBox.Text = sb.ToString();

        m_process = Process.GetProcessById(pId);
        ExePathTextBox.Text = m_process.MainModule?.FileName;
    }
}