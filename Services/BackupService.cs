using System;
using System.IO;

namespace MASA.RetailPOS.App.Services;

public class BackupService
{
    private readonly string _dbFileName = "MASA_RetailPOS.db";
    private readonly string _backupFolder = "Backups";

    public BackupService()
    {
        if (!Directory.Exists(_backupFolder))
        {
            Directory.CreateDirectory(_backupFolder);
        }
    }

    /// <summary>
    /// Runs a silent daily backup if one hasn't been created today.
    /// </summary>
    public void RunDailyBackup()
    {
        try
        {
            string todayBackupName = $"Backup_{DateTime.Now:yyyyMMdd}.db";
            string backupPath = Path.Combine(_backupFolder, todayBackupName);

            if (!File.Exists(backupPath) && File.Exists(_dbFileName))
            {
                File.Copy(_dbFileName, backupPath);
            }
        }
        catch
        {
            // Fail silently for auto-backup
        }
    }

    /// <summary>
    /// Manually creates a backup at the specified destination path.
    /// </summary>
    public bool CreateManualBackup(string destinationPath)
    {
        try
        {
            if (File.Exists(_dbFileName))
            {
                File.Copy(_dbFileName, destinationPath, overwrite: true);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Restores the database from a backup file. Requires application restart to take full effect safely.
    /// </summary>
    public bool RestoreBackup(string sourcePath)
    {
        try
        {
            if (File.Exists(sourcePath))
            {
                // Note: In a real production app, we should ensure all DB connections are closed before overwriting.
                File.Copy(sourcePath, _dbFileName, overwrite: true);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
