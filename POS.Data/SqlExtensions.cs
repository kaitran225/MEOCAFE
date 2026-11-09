using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace POS.Data;

public static class SqlExtensions
{
    public static DbCommand CreateTextCommand(this DbConnection connection, string commandText)
    {
        var command = connection.CreateCommand();
        command.CommandText = commandText;
        return command;
    }

    public static void AddWithValue(this DbParameterCollection parameters, string parameterName, object? value)
    {
        if (parameters is SqliteParameterCollection sqlite)
        {
            sqlite.AddWithValue(parameterName, value ?? DBNull.Value);
            return;
        }

        throw new NotSupportedException("SQLite parameters are required.");
    }

    public static bool GetBooleanCompat(this DbDataReader reader, int ordinal)
        => !reader.IsDBNull(ordinal) && Convert.ToInt32(reader.GetValue(ordinal)) != 0;

    public static DateTime ReadDateTime(this DbDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return default;
        var value = reader.GetValue(ordinal);
        return value switch
        {
            DateTime dt => dt,
            string s when DateTime.TryParse(s, out var parsed) => parsed,
            _ => Convert.ToDateTime(value)
        };
    }
}
