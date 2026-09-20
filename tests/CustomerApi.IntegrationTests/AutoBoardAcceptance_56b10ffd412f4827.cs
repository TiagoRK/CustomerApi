using System.Text.Json;
using CustomerApi.Domain.Idempotency;
using CustomerApi.Domain.Logging;
using CustomerApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerApi.IntegrationTests;

// Trusted host capture helper (appended by test infrastructure)
// test_AutoBoardAcceptance_56b10ffd412f4827_Host

[TestFixture]
public class AutoBoardAcceptance_56b10ffd412f4827
{
  private static string FindAppsettingsPath()
  {
    // Walk from the test binary directory upward until we find the solution root
    // by locating src/CustomerApi.LogWorker/appsettings.json
    var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
    while (dir != null)
    {
      var candidate = Path.Combine(dir.FullName, "src", "CustomerApi.LogWorker", "appsettings.json");
      if (File.Exists(candidate))
        return candidate;
      dir = dir.Parent;
    }
    throw new FileNotFoundException("Cannot locate src/CustomerApi.LogWorker/appsettings.json by walking ancestors.");
  }

  private static string FindProjectDir()
  {
    var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
    while (dir != null)
    {
      var candidate = Path.Combine(dir.FullName, "src", "CustomerApi.LogWorker");
      if (Directory.Exists(candidate))
        return candidate;
      dir = dir.Parent;
    }
    throw new DirectoryNotFoundException("Cannot locate src/CustomerApi.LogWorker by walking ancestors.");
  }

  [Test]
  public void test_AutoBoardAcceptance_56b10ffd412f4827_C1()
  {
    var appsettingsPath = FindAppsettingsPath();

    Assert.That(File.Exists(appsettingsPath), Is.True,
        $"Expected appsettings.json at: {appsettingsPath}");

    var content = File.ReadAllText(appsettingsPath);
    Assert.That(content, Does.Contain("\"ConnectionStrings\""),
        "appsettings.json must contain a 'ConnectionStrings' section");

    using var doc = JsonDocument.Parse(content);
    var root = doc.RootElement;
    Assert.That(root.TryGetProperty("ConnectionStrings", out var csSection), Is.True,
        "appsettings.json root must have a 'ConnectionStrings' property");
    Assert.That(csSection.ValueKind, Is.EqualTo(JsonValueKind.Object),
        "'ConnectionStrings' must be a JSON object");
    Assert.That(csSection.EnumerateObject().MoveNext(), Is.True,
        "'ConnectionStrings' section must contain at least one connection string entry");
  }

  [Test]
  public async Task test_AutoBoardAcceptance_56b10ffd412f4827_C2()
  {
    // The real DB connection string is normally injected by the test runner via the environment variable.
    // Fall back to the LogWorker's own appsettings.json connection string when it isn't injected (e.g. local runs).
    var dbConnStr = Environment.GetEnvironmentVariable("AUTOBOARD_ACCEPTANCE_DB_DOTNET");
    if (string.IsNullOrEmpty(dbConnStr))
    {
      var appsettingsPath = FindAppsettingsPath();
      using var doc = JsonDocument.Parse(File.ReadAllText(appsettingsPath));
      dbConnStr = doc.RootElement.GetProperty("ConnectionStrings").GetProperty("Database").GetString();
    }

    Assert.That(dbConnStr, Is.Not.Null.And.Not.Empty,
        "AUTOBOARD_ACCEPTANCE_DB_DOTNET must be set by the test infrastructure or resolvable from appsettings.json");

    var projectDir = FindProjectDir();

    // Start the real LogWorker host via the trusted capture helper.
    // Pass the isolated real DB connection string so the actual startup wires LogDbContext to it.
    await using var capture = await test_AutoBoardAcceptance_56b10ffd412f4827_Host.StartAsync(
        typeof(CustomerApi.LogWorker.LogWorkerService).Assembly,
        new[]
        {
                  "--contentRoot", projectDir,
                  "--ConnectionStrings:Database", dbConnStr!
        });

    var host = capture.Host;

    // Use the DI-resolved LogDbContext (registered by the real startup) to create the schema
    // and assert persistence via the real database.
    using var scope = host.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();

    // Ensure schema exists in the isolated database
    await dbContext.Database.EnsureCreatedAsync();

    // Write a log entry through the real ILogEntryChannel
    var channel = host.Services.GetRequiredService<ILogEntryChannel>();

    var entryId = Guid.NewGuid();
    var entry = new LogEntry
    {
      Id = entryId,
      Endpoint = "/api/test-logworker",
      Method = "GET",
      Direction = LogDirection.Request,
      CreatedAt = DateTimeOffset.UtcNow
    };

    await channel.WriteAsync(entry, CancellationToken.None);

    // Poll the real database until the LogWorkerService persists the row
    LogEntry? persisted = null;
    var deadline = DateTimeOffset.UtcNow.AddSeconds(15);
    while (DateTimeOffset.UtcNow < deadline)
    {
      await Task.Delay(200);
      // Re-query via a fresh context to avoid caching
      using var pollScope = host.Services.CreateScope();
      var pollCtx = pollScope.ServiceProvider.GetRequiredService<LogDbContext>();
      persisted = await pollCtx.LogEntries
          .AsNoTracking()
          .FirstOrDefaultAsync(e => e.Id == entryId);
      if (persisted != null)
        break;
    }

    Assert.That(persisted, Is.Not.Null,
        "LogDbContext must have persisted the LogEntry via the LogWorker's own connection string within the deadline");
    Assert.That(persisted!.Endpoint, Is.EqualTo("/api/test-logworker"),
        "Persisted entry endpoint must match");
    Assert.That(persisted.Method, Is.EqualTo("GET"),
        "Persisted entry method must match");
    Assert.That(persisted.Direction, Is.EqualTo(LogDirection.Request),
        "Persisted entry direction must match");
  }
}

// Trusted test harness: captures the host built by the actual assembly entrypoint.
// No application registrations are copied into this helper.
internal sealed class test_AutoBoardAcceptance_56b10ffd412f4827_Host : System.IAsyncDisposable
{
  private static readonly System.Threading.SemaphoreSlim Gate = new(1, 1);
  private static readonly System.Threading.AsyncLocal<bool> Invoking = new();
  private readonly System.Collections.Generic.List<System.IDisposable> subscriptions = new();
  private readonly System.Threading.Tasks.TaskCompletionSource<Microsoft.Extensions.Hosting.IHost> built =
      new(System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
  private System.Threading.Tasks.Task? execution;
  public Microsoft.Extensions.Hosting.IHost Host { get; private set; } = null!;

  private sealed class Observer<T>(System.Action<T> next) : System.IObserver<T>
  {
    public void OnNext(T value) => next(value);
    public void OnError(System.Exception error) { }
    public void OnCompleted() { }
  }

  public static async System.Threading.Tasks.Task<test_AutoBoardAcceptance_56b10ffd412f4827_Host> StartAsync(
      System.Reflection.Assembly assembly, string[] args)
  {
    if (!await Gate.WaitAsync(System.TimeSpan.FromSeconds(40)).ConfigureAwait(false))
      throw new System.TimeoutException("Another acceptance host is still running");
    var capture = new test_AutoBoardAcceptance_56b10ffd412f4827_Host();
    try
    {
      capture.subscriptions.Add(System.Diagnostics.DiagnosticListener.AllListeners.Subscribe(
          new Observer<System.Diagnostics.DiagnosticListener>(listener =>
          {
            if (listener.Name != "Microsoft.Extensions.Hosting") return;
            capture.subscriptions.Add(listener.Subscribe(
                    new Observer<System.Collections.Generic.KeyValuePair<string, object?>>(ev =>
                    {
                    if (Invoking.Value && ev.Key == "HostBuilt" && ev.Value is Microsoft.Extensions.Hosting.IHost host)
                      capture.built.TrySetResult(host);
                  })));
          })));
      capture.execution = System.Threading.Tasks.Task.Run(async () =>
      {
        Invoking.Value = true;
        try
        {
          var entry = assembly.EntryPoint ?? throw new System.InvalidOperationException("Assembly has no entrypoint");
          var result = entry.Invoke(null, entry.GetParameters().Length == 0 ? null : new object[] { args });
          if (result is System.Threading.Tasks.Task task) await task.ConfigureAwait(false);
          if (!capture.built.Task.IsCompleted)
            capture.built.TrySetException(new System.InvalidOperationException("Entrypoint did not build a Generic Host"));
        }
        catch (System.Exception error)
        {
          capture.built.TrySetException(error.InnerException ?? error);
          throw;
        }
        finally { Invoking.Value = false; }
      });
      capture.Host = await capture.built.Task.WaitAsync(System.TimeSpan.FromSeconds(40)).ConfigureAwait(false);
      var lifetime = (Microsoft.Extensions.Hosting.IHostApplicationLifetime)capture.Host.Services.GetService(
          typeof(Microsoft.Extensions.Hosting.IHostApplicationLifetime))!;
      var started = new System.Threading.Tasks.TaskCompletionSource<bool>(System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);
      using var registration = lifetime.ApplicationStarted.Register(() => started.TrySetResult(true));
      var ready = await System.Threading.Tasks.Task.WhenAny(started.Task, capture.execution).WaitAsync(System.TimeSpan.FromSeconds(40)).ConfigureAwait(false);
      await ready.ConfigureAwait(false);
      if (!started.Task.IsCompletedSuccessfully) throw new System.InvalidOperationException("Host exited before startup completed");
      return capture;
    }
    catch { await capture.DisposeAsync(); throw; }
  }

  public async System.Threading.Tasks.ValueTask DisposeAsync()
  {
    try
    {
      if (Host is not null && execution?.IsCompleted != true)
      {
        using var deadline = new System.Threading.CancellationTokenSource(System.TimeSpan.FromSeconds(10));
        await Host.StopAsync(deadline.Token).ConfigureAwait(false);
      }
      if (execution is not null) await execution.WaitAsync(System.TimeSpan.FromSeconds(15)).ConfigureAwait(false);
    }
    finally
    {
      Host?.Dispose();
      foreach (var subscription in subscriptions) subscription.Dispose();
      Gate.Release();
    }
  }
}
