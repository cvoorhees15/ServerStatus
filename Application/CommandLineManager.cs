using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerStatus.Util;

class CommandLineManager
{
    private readonly object _displayLock = new object();
    private bool _isDisplayMode = false;

    public CommandLineManager()
    {
        Console.WriteLine(@"
  ____                             ____  _        _             
 / ___|  ___ _ ____   _____ _ __  / ___|| |_ __ _| |_ _   _ ___ 
 \___ \ / _ \ '__\ \ / / _ \ '__| \___ \| __/ _` | __| | | / __|
  ___) |  __/ |   \ V /  __/ |     ___) | || (_| | |_| |_| \__ \
 |____/ \___|_|    \_/ \___|_|    |____/ \__\__,_|\__|\__,_|___/

");
    }

    public void ShowUsage()
    {
        Console.WriteLine("Usage: ServerStatus [options]");
        Console.WriteLine();
        Console.WriteLine("Connection Options:");
        Console.WriteLine("  --ssh         Use SSH connection (default)");
        Console.WriteLine("  --tcp         Use TCP connection");
        Console.WriteLine("  --ping        Use ping connection");
        Console.WriteLine();
        Console.WriteLine("Features:");
        Console.WriteLine("  • Real-time streaming dashboard (btop-style)");
        Console.WriteLine("  • Live server performance metrics");
        Console.WriteLine("  • Auto-refreshing display every second");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  ServerStatus --ssh             # Stream server metrics via SSH");
        Console.WriteLine("  ServerStatus --tcp             # Stream server metrics via TCP");
        Console.WriteLine("  ServerStatus                   # Use default SSH connection");
        Console.WriteLine();
        Console.WriteLine("Controls:");
        Console.WriteLine("  q             Quit application");
        Console.WriteLine();
    }

    public bool QuitApp { get; private set; } = false;

    public ConnectionBase ParseArgs(string[] args)
    {
        // Check for help flags
        if (args.Contains("--help") || args.Contains("-h"))
        {
            ShowUsage();
            Environment.Exit(0);
        }

        if (args.Contains("--tcp"))
        {
            return new TCPConnection(Credentials.Host!, Credentials.TcpPort);
        }
        else if (args.Contains("--ssh"))
        {
            return new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
        }
        else if (args.Contains("--ping"))
        {
            return new PingConnection(Credentials.Host!);
        }
        else // default to ssh for now
        {
            return new SshConnection(Credentials.Host!, Credentials.User!, Credentials.Password!);
        }
    }

    public void StartKeyboardHandling(CancellationTokenSource cts)
    {
        Logger.Instance.LogInfo("Enter 'q' to quit...");

        var inputThread = new Thread(() =>
        {
            while (!QuitApp)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (char.ToLower(key.KeyChar) == 'q')
                    {
                        QuitApp = true;
                        cts.Cancel();
                        Logger.Instance.LogInfo("Quitting...");
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        });

        inputThread.IsBackground = true;
        inputThread.Start();
    }

    public void StartDisplayMode()
    {
        lock (_displayLock)
        {
            _isDisplayMode = true;
            Console.Clear();
            Console.CursorVisible = false;
        }
    }

    public void EndDisplayMode()
    {
        lock (_displayLock)
        {
            _isDisplayMode = false;
            Console.CursorVisible = true;
            Console.Clear();
        }
    }

    public void DisplayServerMetrics(string cpuMetrics, string memoryMetrics, string diskMetrics, string networkMetrics)
    {
        lock (_displayLock)
        {
            if (!_isDisplayMode) return;

            Console.SetCursorPosition(0, 0);

            var displayContent = CreateFormattedDisplay(cpuMetrics, memoryMetrics, diskMetrics, networkMetrics);
            Console.Write(displayContent);

            FillRemainingLines();
        }
    }

    private string CreateFormattedDisplay(string cpuMetrics, string memoryMetrics, string diskMetrics, string networkMetrics)
    {
        var sb = new StringBuilder();
        var width = Console.WindowWidth;
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        sb.AppendLine(CreateHeader("SERVER STATUS DASHBOARD", timestamp, width));
        sb.AppendLine();

        sb.AppendLine(CreateSection("CPU PERFORMANCE", cpuMetrics, width));
        sb.AppendLine();

        sb.AppendLine(CreateSection("MEMORY USAGE", memoryMetrics, width));
        sb.AppendLine();

        sb.AppendLine(CreateSection("DISK USAGE", diskMetrics, width));
        sb.AppendLine();

        sb.AppendLine(CreateSection("NETWORK ACTIVITY", networkMetrics, width));
        sb.AppendLine();

        sb.AppendLine(CreateFooter("Press 'q' to quit", width));

        return sb.ToString();
    }

    private string CreateHeader(string title, string timestamp, int width)
    {
        var borderChar = '═';
        var cornerChar = '╔';
        var endCornerChar = '╗';

        var headerText = $" {title} ";
        var timeText = $" {timestamp} ";
        var totalHeaderLength = headerText.Length + timeText.Length;
        var padding = Math.Max(0, width - totalHeaderLength - 4);

        var topBorder = cornerChar + new string(borderChar, width - 2) + endCornerChar;
        var headerLine = $"║{headerText}{new string(' ', padding)}{timeText}║";
        var bottomBorder = '╚' + new string(borderChar, width - 2) + '╝';

        return topBorder + Environment.NewLine + headerLine + Environment.NewLine + bottomBorder;
    }

    private string CreateSection(string title, string content, int width)
    {
        var sb = new StringBuilder();
        var titleLine = $"┌─ {title} {new string('─', Math.Max(0, width - title.Length - 6))}┐";
        sb.AppendLine(titleLine);

        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Take(3))
            {
                var cleanLine = line.Trim();
                if (cleanLine.Length > width - 4)
                {
                    cleanLine = cleanLine.Substring(0, width - 7) + "...";
                }
                sb.AppendLine($"│ {cleanLine.PadRight(width - 4)} │");
            }
        }
        else
        {
            sb.AppendLine($"│ {"No data available".PadRight(width - 4)} │");
        }

        var bottomLine = $"└{new string('─', width - 2)}┘";
        sb.AppendLine(bottomLine);

        return sb.ToString();
    }

    private string CreateFooter(string text, int width)
    {
        var padding = Math.Max(0, (width - text.Length - 2) / 2);
        return $"┌{new string('─', width - 2)}┐" + Environment.NewLine +
               $"│{new string(' ', padding)}{text}{new string(' ', width - text.Length - padding - 2)}│" + Environment.NewLine +
               $"└{new string('─', width - 2)}┘";
    }

    private void FillRemainingLines()
    {
        var currentLine = Console.CursorTop;
        var windowHeight = Console.WindowHeight;

        for (int i = currentLine; i < windowHeight - 1; i++)
        {
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }
    }

    public void DisplayProgressBar(string label, double percentage, int width = 50)
    {
        lock (_displayLock)
        {
            if (!_isDisplayMode) return;

            var filled = (int)(percentage / 100.0 * width);
            var empty = width - filled;

            var bar = $"[{new string('█', filled)}{new string('░', empty)}] {percentage:F1}%";
            Console.WriteLine($"{label}: {bar}");
        }
    }

    public void ShowLoadingSpinner(string message, CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            var spinnerChars = new[] { '|', '/', '-', '\\' };
            var index = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                lock (_displayLock)
                {
                    if (_isDisplayMode)
                    {
                        Console.SetCursorPosition(0, Console.WindowHeight - 1);
                        Console.Write($"{message} {spinnerChars[index % spinnerChars.Length]}");
                    }
                }

                index++;
                await Task.Delay(250, cancellationToken);
            }
        }, cancellationToken);
    }
}