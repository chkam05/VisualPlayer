using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.Utilities
{
    public static class SystemHelper
    {

        //  CONST

        public const string APP_DATA_PATH_EV = "APPDATA";
        public const string HOME_PATH_EV = "HOMEPATH";
        public const string SYSTEM_DRIVE_PATH_EV = "SYSTEMDRIVE";
        public const string USER_NAME_EV = "USERNAME";
        public const string USER_PROFILE_PATH_EV = "USERPROFILE";


        //  METHODS

        #region DIRECTORIES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get app data path. </summary>
        /// <returns> App data path. </returns>
        public static string GetAppDataPath()
        {
            return Environment.GetEnvironmentVariable(APP_DATA_PATH_EV);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get home path. </summary>
        /// <returns> Home path. </returns>
        public static string GetHomePath()
        {
            return Environment.GetEnvironmentVariable(HOME_PATH_EV);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get system drive path. </summary>
        /// <returns> System drive path. </returns>
        public static string GetSystemDrivePath()
        {
            return Environment.GetEnvironmentVariable(SYSTEM_DRIVE_PATH_EV);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get user profile path. </summary>
        /// <returns> User profile path. </returns>
        public static string GetUserProfilePath()
        {
            return Environment.GetEnvironmentVariable(USER_PROFILE_PATH_EV);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if app data path is available. </summary>
        /// <returns> True - app data path is available; False - otherwise. </returns>
        public static bool IsAppDataPath()
        {
            return IsPathAvailable(GetAppDataPath());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if home path is available. </summary>
        /// <returns> True - home path is available; False - otherwise. </returns>
        public static bool IsHomePath()
        {
            return IsPathAvailable(GetHomePath());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if system drive path is available. </summary>
        /// <returns> True - system drive path is available; False - otherwise. </returns>
        public static bool IsSystemDrivePath()
        {
            return IsPathAvailable(GetSystemDrivePath());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if user profile path is available. </summary>
        /// <returns> True - user profile path is available; False - otherwise. </returns>
        public static bool IsUserProfilePath()
        {
            return IsPathAvailable(GetUserProfilePath());
        }

        #endregion DIRECTORIES METHODS

        #region FILES METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get directories. </summary>
        /// <param name="path"> Path. </param>
        /// <param name="searchPattern"> Search pattern. </param>
        /// <returns> Directories list or null. </returns>
        public static List<FileInfo> GetDirectories(string path, string searchPattern = null,
            bool isHidden = false, bool isSystem = false)
        {
            if (!IsPathAvailable(path))
                return null;

            var dirPaths = !string.IsNullOrEmpty(searchPattern)
                ? Directory.GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly)
                : Directory.GetDirectories(path);

            return dirPaths.Select(dp => new FileInfo(dp))
                .Where(dp
                    => IsFileHidden(dp) ? isHidden : true
                    && IsFileSystem(dp) ? isSystem : true)
                .ToList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get files and directories. </summary>
        /// <param name="path"> Path. </param>
        /// <param name="searchPattern"> Search pattern. </param>
        /// <returns> Files and directories list or null. </returns>
        public static List<FileInfo> GetFilesAndDirectories(string path, string searchPattern = null,
            IEnumerable<string> extensions = null, bool isHidden = false, bool isSystem = false)
        {
            if (!IsPathAvailable(path))
                return null;

            List<FileInfo> filedAndDirectories = new List<FileInfo>();

            filedAndDirectories.AddRange(GetDirectories(path, searchPattern, isHidden, isSystem));
            filedAndDirectories.AddRange(GetFiles(path, searchPattern, extensions, isHidden, isSystem));

            return filedAndDirectories;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get drives. </summary>
        /// <returns> Drives list. </returns>
        public static List<DriveInfo> GetDrives()
        {
            return DriveInfo.GetDrives().ToList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get files. </summary>
        /// <param name="path"> Path. </param>
        /// <param name="searchPattern"> Search pattern. </param>
        /// <returns> Files list or null. </returns>
        public static List<FileInfo> GetFiles(string path, string searchPattern = null,
            IEnumerable<string> extensions = null, bool isHidden = false, bool isSystem = false)
        {
            if (!IsPathAvailable(path))
                return null;

            var filesPaths = !string.IsNullOrEmpty(searchPattern)
                ? Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly)
                : Directory.GetFiles(path);

            return filesPaths.Select(fp => new FileInfo(fp))
                .Where(fp
                    => IsFileHidden(fp) ? isHidden : true
                    && IsFileSystem(fp) ? isSystem : true
                    && IsFileTypeOf(fp, extensions))
                .ToList();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file path is directory. </summary>
        /// <param name="filePath"> File path. </param>
        /// <returns> True - is directory; False - otherwise. </returns>
        public static bool IsDirectory(string filePath)
        {
            return Directory.Exists(filePath) && IsDirectory(new FileInfo(filePath));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file info is directory. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> True - is directory; False - otherwise. </returns>
        public static bool IsDirectory(FileInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file path is file. </summary>
        /// <param name="filePath"> File path. </param>
        /// <returns> True - is file; False - otherwise. </returns>
        public static bool IsFile(string filePath)
        {
            return File.Exists(filePath) && IsFile(new FileInfo(filePath));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file info is file. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> True - is file; False - otherwise. </returns>
        public static bool IsFile(FileInfo fileInfo)
        {
            return !IsDirectory(fileInfo);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file or directory is archive. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> True - file or directory is archive; False - otherwise. </returns>
        public static bool IsFileArchive(FileInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file or directory is hidden. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> True - file or directory is hidden; False - otherwise. </returns>
        public static bool IsFileHidden(FileInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file or directory is system. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <returns> True - file or directory is system; False - otherwise. </returns>
        public static bool IsFileSystem(FileInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.System) == FileAttributes.System;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file is type of extension. </summary>
        /// <param name="filePath"> File path. </param>
        /// <param name="extension"> File extension. </param>
        /// <returns> True - file is type of extension; False - otherwise. </returns>
        public static bool IsFileTypeOf(string filePath, string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;

            if (extension == "*.*")
                return true;

            return filePath.ToLower().EndsWith(extension.Replace("*", "").Replace(".", "").ToLower());
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file is type of one of extensions. </summary>
        /// <param name="filePath"> File path. </param>
        /// <param name="extensions"> Collection of extensions. </param>
        /// <returns> True - file is type of one of extensions; Flase - otherwise. </returns>
        public static bool IsFileTypeOf(string filePath, IEnumerable<string> extensions)
        {
            if (extensions == null || !extensions.Any())
                return true;

            return extensions.Any(e => IsFileTypeOf(filePath, e));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file is type of extension. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <param name="extension"> File extension. </param>
        /// <returns> True - file is type of extension; False - otherwise. </returns>
        public static bool IsFileTypeOf(FileInfo fileInfo, string extension)
        {
            return IsFileTypeOf(fileInfo.FullName, extension);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if file is type of one of extensions. </summary>
        /// <param name="fileInfo"> File info. </param>
        /// <param name="extensions"> Collection of extensions. </param>
        /// <returns> True - file is type of one of extensions; Flase - otherwise. </returns>
        public static bool IsFileTypeOf(FileInfo fileInfo, IEnumerable<string> extensions)
        {
            return IsFileTypeOf(fileInfo.FullName, extensions);
        }

        #endregion FILES METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check if path is available. </summary>
        /// <param name="path"> Path. </param>
        /// <returns> True - path is available; False - otherwise. </returns>
        public static bool IsPathAvailable(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return Directory.Exists(path);
        }

        #endregion UTILITY METHODS

        #region USER INFO

        //  --------------------------------------------------------------------------------
        /// <summary> Get user name. </summary>
        /// <returns> User name. </returns>
        public static string GetUserName()
        {
            return Environment.GetEnvironmentVariable(USER_NAME_EV);
        }

        #endregion USER INFO

    }
}
