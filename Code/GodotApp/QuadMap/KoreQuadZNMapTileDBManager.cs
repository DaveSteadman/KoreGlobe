using System;

using KoreCommon;

#nullable enable

// Functions to load and save the tile database file, as well as determine existence of tiles.
// - Adds the layer of tile processing on top of the KoreBinaryDataManager's plain DB accesses

public static class KoreQuadZNMapTileDBManager
{
    public static KoreBinaryDataManager? DBManager = null;

    // Lock object, so we can ensure only one reader or writer at a time
    private static readonly object _lock = new();

    // --------------------------------------------------------------------------------------------

    // Usage: KoreQuadZNMapTileDBManager.CreateOrOpenDB();
    public static void CreateOrOpenDB()
    {
        KoreCentralLog.AddEntry($"KoreQuadZNMapTileDBManager: CreateOrOpenDB");

        string dbPath = "UnitTestArtefacts/test_db.sqlite";

        DBManager = new KoreBinaryDataManager(dbPath);

        int count = DBManager.NumEntries();
        KoreCentralLog.AddEntry($"KoreQuadZNMapTileDBManager: Opened TileDB.sqlite ({count} entries)");
    }

    // --------------------------------------------------------------------------------------------

    public static bool SetBytesForName(string name, byte[] data)
    {
        lock (_lock)
        {
            if (DBManager == null)
            {
                KoreCentralLog.AddEntry($"KoreQuadZNMapTileDBManager: ERROR: DBManager is null in SetBytesForName");
                return false;
            }

            bool result = DBManager.Set(name, data);
            if (!result)
            {
                KoreCentralLog.AddEntry($"KoreQuadZNMapTileDBManager: ERROR: Failed to set data for {name}");
            }
            return result;
        }
    }

    // --------------------------------------------------------------------------------------------

    // Usage: bool exists = KoreQuadZNMapTileDBManager.HasBytesForName(name);

    public static bool HasBytesForName(string name)
    {
        return DBManager?.DataExists(name) ?? false;
    }

    // --------------------------------------------------------------------------------------------

    public static byte[] BytesForName(string name)
    {
        lock (_lock)
        {
            if (DBManager == null)
            {
                KoreCentralLog.AddEntry($"KoreQuadZNMapTileDBManager: ERROR: DBManager is null in BytesForName");
                return Array.Empty<byte>();
            }

            byte[] data = DBManager.Get(name);
            if (data.Length == 0)
            {
                KoreCentralLog.AddEntry($"KoreQuadZNMapTileDBManager: No data found for {name}");
            }
            return data;
        }
    }

    // --------------------------------------------------------------------------------------------

}
