using Microsoft.Extensions.Configuration;
using POS.Data;

namespace POS.Avalonia.Services;

public interface IDatabaseSyncService
{
    string DatabasePath { get; }
    void CheckpointForGitSync();
}

public sealed class DatabaseSyncService : IDatabaseSyncService
{
    public DatabaseSyncService(IConfiguration configuration)
    {
        DatabasePath = DatabasePathResolver.Resolve(configuration);
    }

    public string DatabasePath { get; }

    public void CheckpointForGitSync() => DatabaseCheckpoint.WalCheckpointTruncate(DatabasePath);
}
