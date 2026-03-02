# ServerStatus

A real-time terminal dashboard for monitoring remote server performance metrics over SSH. Displays CPU, memory, disk, and network statistics in a live-updating TUI and sends email alerts reporting on downtime.

## Features

- Live dashboard refreshing every second
- CPU usage, load averages, and top CPU-consuming processes
- Memory usage including buffers, cache, and top memory-consuming processes
- Disk filesystem usage and I/O metrics
- Network interface status, active connections, and traffic data
- ICMP ping latency display
- Cross-platform support for Linux, Windows, and macOS targets

## Requirements

- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) runtime
- SSH access to the target server
- SMTP server for email alerts
- Credentials file at `~/.creds/ServerStatus.json` (see below)

## Credentials File

Create `~/.creds/ServerStatus.json` with the following structure:

```json
{
  "Host": "your-server-hostname-or-ip",
  "User": "ssh-username",
  "Password": "ssh-password",
  "TcpPort": 22,
  "SmtpHost": "smtp.example.com",
  "SmtpPort": 587,
  "AdminEmail": "admin@example.com",
  "OperatingSystem": "linux"
}
```

| Field             | Description                                          |
|-------------------|------------------------------------------------------|
| `Host`            | Hostname or IP address of the server to monitor      |
| `User`            | SSH username                                         |
| `Password`        | SSH password                                         |
| `TcpPort`         | TCP port to check connectivity on                    |
| `SmtpHost`        | SMTP server hostname for email alerts                |
| `SmtpPort`        | SMTP server port (typically `587` for TLS)           |
| `AdminEmail`      | Email address to receive downtime alerts             |
| `OperatingSystem` | Target OS: `linux`, `windows`, or `macos`            |

## Build & Run

```bash
# Build
dotnet build

# Run
dotnet run

# Release build
dotnet build -c Release
./bin/Release/net9.0/ServerStatus
```

## Usage

Once running, the dashboard renders and refreshes automatically. Press **`q`** to quit.

## Dependencies

| Package              | Version    | Purpose                        |
|----------------------|------------|--------------------------------|
| `SSH.NET`            | 2025.1.0   | SSH connectivity and commands  |
| `Newtonsoft.Json`    | 13.0.3     | Credentials file parsing       |
