// <fileheader>

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Microsoft.Data.Sqlite;

#nullable enable

namespace KoreCommon;

public sealed class KoreBinaryDataManager : IDisposable
{
    private readonly string        _connectionString;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool                   _disposed;

    // --------------------------------------------------------------------------------------------

    public KoreBinaryDataManager(string dbFilePath)
    {
        _connectionString = $"Data Source={dbFilePath};Mode=ReadWriteCreate;Cache=Shared";
        EnsureSchema();
    }

    // --------------------------------------------------------------------------------------------

    private void EnsureSchema()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
            cmd.ExecuteNonQuery();
        }

        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS BlobTable
            (
                DataName  TEXT PRIMARY KEY,
                DataBytes BLOB NOT NULL
            );
            ";
            cmd.ExecuteNonQuery();
        }
    }

    // --------------------------------------------------------------------------------------------

    public bool Set(string dataName, ReadOnlySpan<byte> dataBytes)
    {
        if (string.IsNullOrWhiteSpace(dataName) || dataBytes.Length == 0) return false;

        _semaphore.Wait();
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText =
            @"
            INSERT INTO BlobTable (DataName, DataBytes)
            VALUES ($name, $bytes)
            ON CONFLICT(DataName) DO UPDATE SET DataBytes=excluded.DataBytes;
            ";

            var pName = cmd.CreateParameter(); pName.ParameterName = "$name"; pName.DbType = DbType.String; pName.Value = dataName;
            var pBytes = cmd.CreateParameter(); pBytes.ParameterName = "$bytes"; pBytes.DbType = DbType.Binary; pBytes.Value = dataBytes.ToArray();

            cmd.Parameters.Add(pName);
            cmd.Parameters.Add(pBytes);

            return cmd.ExecuteNonQuery() > 0;
        }
        finally { _semaphore.Release(); }
    }

    // --------------------------------------------------------------------------------------------

    public bool Delete(string dataName)
    {
        _semaphore.Wait();
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM BlobTable WHERE DataName=$name;";
            cmd.Parameters.AddWithValue("$name", dataName);

            return cmd.ExecuteNonQuery() > 0;
        }
        finally { _semaphore.Release(); }
    }

    // --------------------------------------------------------------------------------------------

    public byte[] Get(string dataName)
    {
        _semaphore.Wait();
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT DataBytes FROM BlobTable WHERE DataName=$name;";
            cmd.Parameters.AddWithValue("$name", dataName);

            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            if (!reader.Read()) return Array.Empty<byte>();

            // Efficient read for medium blobs (~150 KB)
            return (byte[])reader["DataBytes"];
        }
        finally { _semaphore.Release(); }
    }

    // --------------------------------------------------------------------------------------------

    public int NumEntries()
    {
        _semaphore.Wait();
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM BlobTable;";

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
        finally { _semaphore.Release(); }
    }

    // --------------------------------------------------------------------------------------------

    public List<string> List()
    {
        var result = new List<string>();

        _semaphore.Wait();
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT DataName FROM BlobTable ORDER BY DataName;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) result.Add(reader.GetString(0));
        }
        finally { _semaphore.Release(); }

        return result;
    }

    // --------------------------------------------------------------------------------------------

    public bool DataExists(string dataName)
    {
        _semaphore.Wait();
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT 1 FROM BlobTable WHERE DataName=$name LIMIT 1;";
            cmd.Parameters.AddWithValue("$name", dataName);

            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }
        finally { _semaphore.Release(); }
    }

    // --------------------------------------------------------------------------------------------

    public void Dispose()
    {
        if (_disposed) return;
        _semaphore.Dispose();
        _disposed = true;
    }
}
