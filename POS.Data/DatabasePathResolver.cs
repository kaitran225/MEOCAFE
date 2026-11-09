using Microsoft.Extensions.Configuration;

namespace POS.Data;

public static class DatabasePathResolver
{
    public const string DefaultRelativePath = "data/hienxanh.db";

    public static string Resolve(IConfiguration configuration)
    {
        var configured = configuration["Database:Path"];
        if (string.IsNullOrWhiteSpace(configured))
            configured = DefaultRelativePath;

        if (Path.IsPathRooted(configured))
            return Path.GetFullPath(configured);

        var baseDir = AppContext.BaseDirectory;
        var candidate = Path.GetFullPath(Path.Combine(baseDir, configured));
        if (File.Exists(candidate))
            return candidate;

        var dir = new DirectoryInfo(baseDir);
        string? repoCandidate = null;
        for (var i = 0; i < 8 && dir.Parent != null; i++)
        {
            dir = dir.Parent;
            var up = Path.GetFullPath(Path.Combine(dir.FullName, configured));
            if (File.Exists(up))
                return up;
            if (repoCandidate == null && File.Exists(Path.Combine(dir.FullName, "HienXanh.sln")))
                repoCandidate = up;
        }

        return repoCandidate ?? candidate;
    }
}
