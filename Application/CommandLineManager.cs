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

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLineManager"/> class and displays the application banner.
    /// </summary>
    public CommandLineManager()
    {
        Logger.Instance.LogInfo("Welcome to Server Status");
    }

    /// <summary>
    /// Displays the usage information for the ServerStatus application, including connection options, features, examples, and controls.
    /// </summary>
    public void ShowUsage()
    {

    }

    public bool QuitApp { get; private set; } = false;

    /// <summary>
    /// Starts a background thread to handle keyboard input for quitting the application.
    /// </summary>
    /// <param name="cts">The cancellation token source used to cancel operations when quitting.</param>
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

    /// <summary>
    /// Starts the display mode by clearing the console and hiding the cursor for real-time metrics display.
    /// </summary>
    public void StartDisplay()
    {
        lock (_displayLock)
        {
            _isDisplayMode = true;
            Console.Clear();
            Console.CursorVisible = false;

            // Enable buffered logging to prevent console interference
            Logger.Instance.EnableDisplayMode();
        }
    }

    /// <summary>
    /// Stops the display by showing the cursor and clearing the console.
    /// </summary>
    public void StopDisplay()
    {
        lock (_displayLock)
        {
            _isDisplayMode = false;
            Console.CursorVisible = true;
            Console.Clear();

            // Disable buffered logging to return to normal console output
            Logger.Instance.DisableDisplayMode();
        }
    }

    /// <summary>
    /// Displays the server metrics in a formatted dashboard layout on the console.
    /// </summary>
    /// <param name="cpuMetrics">The CPU performance metrics as a string.</param>
    /// <param name="memoryMetrics">The memory usage metrics as a string.</param>
    /// <param name="diskMetrics">The disk usage metrics as a string.</param>
    /// <param name="networkMetrics">The network activity metrics as a string.</param>
    /// <param name="latency">The server latency in milliseconds.</param>
    public void DisplayServerMetrics(string cpuMetrics, string memoryMetrics, string diskMetrics, string networkMetrics, long latency)
    {
        lock (_displayLock)
        {
            if (!_isDisplayMode) return;

            Console.SetCursorPosition(0, 0);

            var displayContent = CreateFormattedDisplay(cpuMetrics, memoryMetrics, diskMetrics, networkMetrics, latency);
            Console.Write(displayContent);

            FillRemainingLines();
        }
    }

    /// <summary>
    /// Creates a formatted display string for the server metrics dashboard.
    /// </summary>
    /// <param name="cpuMetrics">The CPU performance metrics.</param>
    /// <param name="memoryMetrics">The memory usage metrics.</param>
    /// <param name="diskMetrics">The disk usage metrics.</param>
    /// <param name="networkMetrics">The network activity metrics.</param>
    /// <param name="latency">The server latency in milliseconds.</param>
    /// <returns>A formatted string representing the complete dashboard display.</returns>
    private string CreateFormattedDisplay(string cpuMetrics, string memoryMetrics, string diskMetrics, string networkMetrics, long latency)
    {
        var sb = new StringBuilder();
        var width = Console.WindowWidth;
        var height = Console.WindowHeight;
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Calculate available space for content
        var headerHeight = 8; // Header takes ~8 lines
        var footerHeight = 4; // Footer takes ~4 lines
        var logsPanelHeight = 10; // Logs panel takes ~10 lines
        var sectionBorders = 10; // Top and bottom borders for 5 sections (2 lines each)
        var minSpacing = 5; // Minimum spacing between sections
        var availableContentHeight = height - headerHeight - footerHeight - logsPanelHeight - sectionBorders - minSpacing;

        // Calculate optimal lines per section (divided by 4, not including logs)
        var linesPerSection = Math.Max(10, availableContentHeight / 4);

        sb.AppendLine(@"
  ____                             ____  _        _
 / ___|  ___ _ ____   _____ _ __  / ___|| |_ __ _| |_ _   _ ___
 \___ \ / _ \ '__\ \ / / _ \ '__| \___ \| __/ _` | __| | | / __|
  ___) |  __/ |   \ V /  __/ |     ___) | || (_| | |_| |_| \__ \
 |____/ \___|_|    \_/ \___|_|    |____/ \__\__,_|\__|\__,_|___/

");
        sb.AppendLine($"Timestamp: {timestamp}  |  Latency: {latency}ms");
        sb.AppendLine();

        sb.AppendLine(CreateSection("CPU PERFORMANCE", cpuMetrics, width, linesPerSection));
        sb.AppendLine();

        sb.AppendLine(CreateSection("MEMORY USAGE", memoryMetrics, width, linesPerSection));
        sb.AppendLine();

        sb.AppendLine(CreateSection("DISK USAGE", diskMetrics, width, linesPerSection));
        sb.AppendLine();

        sb.AppendLine(CreateSection("NETWORK ACTIVITY", networkMetrics, width, linesPerSection));
        sb.AppendLine();

        sb.AppendLine(CreateLogsSection(width));
        sb.AppendLine();

        sb.AppendLine(CreateFooter("Press 'q' to quit", width));

        return sb.ToString();
    }

    /// <summary>
    /// Creates a formatted header string for the dashboard with title and timestamp.
    /// </summary>
    /// <param name="title">The title of the dashboard.</param>
    /// <param name="timestamp">The current timestamp.</param>
    /// <param name="width">The width of the console window.</param>
    /// <returns>A formatted header string.</returns>
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

    /// <summary>
    /// Creates a formatted section string for a specific metric category.
    /// </summary>
    /// <param name="title">The title of the section.</param>
    /// <param name="content">The content to display in the section.</param>
    /// <param name="width">The width of the console window.</param>
    /// <param name="maxLines">The maximum number of lines to display in the section.</param>
    /// <returns>A formatted section string.</returns>
    private string CreateSection(string title, string content, int width, int maxLines = 10)
    {
        var sb = new StringBuilder();
        var titleLine = $"┌─ {title} {new string('─', Math.Max(0, width - title.Length - 6))}┐";
        sb.AppendLine(titleLine);

        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var processedLines = ProcessStructuredContent(lines, width - 4);
            
            // Limit total lines to fit in display - dynamically calculated based on terminal size
            var linesToShow = Math.Min(processedLines.Count, maxLines);
            for (int i = 0; i < linesToShow; i++)
            {
                sb.AppendLine($"│ {processedLines[i].PadRight(width - 4)} │");
            }
            
            // Add "more data" indicator if truncated
            if (processedLines.Count > linesToShow)
            {
                var moreText = $"... {processedLines.Count - linesToShow} more lines";
                sb.AppendLine($"│ {moreText.PadRight(width - 4)} │");
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

    /// <summary>
    /// Processes the structured content lines to format them appropriately for display.
    /// </summary>
    /// <param name="lines">The array of content lines.</param>
    /// <param name="maxWidth">The maximum width for each line.</param>
    /// <returns>A list of processed and formatted content lines.</returns>
    private List<string> ProcessStructuredContent(string[] lines, int maxWidth)
    {
        var result = new List<string>();
        
        foreach (var line in lines)
        {
            var cleanLine = line.Trim();
            if (string.IsNullOrEmpty(cleanLine)) continue;
            
            // Handle section headers (lines ending with ':')
            if (cleanLine.EndsWith(":"))
            {
                result.Add($"▶ {cleanLine}");
                continue;
            }
            
            // Handle long lines by wrapping or truncating intelligently
            if (cleanLine.Length <= maxWidth)
            {
                result.Add($"  {cleanLine}");
            }
            else
            {
                // For data lines, try to preserve important information
                if (cleanLine.Contains("%") || cleanLine.Contains("GB") || cleanLine.Contains("MB") || cleanLine.Contains("KB"))
                {
                    // This looks like metrics data, try to preserve the numbers
                    var truncated = cleanLine.Substring(0, maxWidth - 3) + "...";
                    result.Add($"  {truncated}");
                }
                else
                {
                    // Regular text, just truncate
                    var truncated = cleanLine.Substring(0, maxWidth - 3) + "...";
                    result.Add($"  {truncated}");
                }
            }
        }
        
        return result;
    }

    /// <summary>
    /// Creates a formatted logs section displaying recent log entries from the Logger.
    /// </summary>
    /// <param name="width">The width of the console window.</param>
    /// <returns>A formatted logs section string.</returns>
    private string CreateLogsSection(int width)
    {
        var sb = new StringBuilder();
        var maxLogLines = 8; // Fixed number of log lines
        var logs = Logger.Instance.GetRecentLogs(maxLogLines);

        var titleLine = $"┌─ RECENT LOGS {new string('─', Math.Max(0, width - 16))}┐";
        sb.AppendLine(titleLine);

        // Always display exactly maxLogLines lines
        for (int i = 0; i < maxLogLines; i++)
        {
            if (i < logs.Count)
            {
                var displayStr = logs[i].ToDisplayString();

                // Truncate if too long
                if (displayStr.Length > width - 4)
                {
                    displayStr = displayStr[..(width - 7)] + "...";
                }

                sb.AppendLine($"│ {displayStr.PadRight(width - 4)} │");
            }
            else
            {
                // Pad with empty lines to maintain fixed height
                sb.AppendLine($"│ {new string(' ', width - 4)} │");
            }
        }

        var bottomLine = $"└{new string('─', width - 2)}┘";
        sb.AppendLine(bottomLine);

        return sb.ToString();
    }

    /// <summary>
    /// Creates a formatted footer string for the dashboard.
    /// </summary>
    /// <param name="text">The text to display in the footer.</param>
    /// <param name="width">The width of the console window.</param>
    /// <returns>A formatted footer string.</returns>
    private string CreateFooter(string text, int width)
    {
        var padding = Math.Max(0, (width - text.Length - 2) / 2);
        return $"┌{new string('─', width - 2)}┐" + Environment.NewLine +
               $"│{new string(' ', padding)}{text}{new string(' ', width - text.Length - padding - 2)}│" + Environment.NewLine +
               $"└{new string('─', width - 2)}┘";
    }

    /// <summary>
    /// Fills the remaining lines of the console with spaces to clear any leftover content.
    /// </summary>
    private void FillRemainingLines()
    {
        var currentLine = Console.CursorTop;
        var windowHeight = Console.WindowHeight;

        for (int i = currentLine; i < windowHeight - 1; i++)
        {
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }
    }

    /// <summary>
    /// Displays a progress bar with the specified label and percentage.
    /// </summary>
    /// <param name="label">The label for the progress bar.</param>
    /// <param name="percentage">The percentage value (0-100).</param>
    /// <param name="width">The width of the progress bar in characters.</param>
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

    /// <summary>
    /// Shows a loading spinner with the specified message in a background task.
    /// </summary>
    /// <param name="message">The message to display with the spinner.</param>
    /// <param name="cancellationToken">The cancellation token to stop the spinner.</param>
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