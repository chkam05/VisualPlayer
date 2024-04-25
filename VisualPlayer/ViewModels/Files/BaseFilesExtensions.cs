using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualPlayer.ViewModels.Files
{
    public static class BaseFilesExtensions
    {

        public static readonly FileExtension Default = FileExtension.GetDefaultFileExtension();

        //  Config files
        public static readonly FileExtension CFG = new FileExtension(new string[] { ".cfg" }, "Configuration File");
        public static readonly FileExtension INI = new FileExtension(new string[] { ".ini" }, "Initialization File");
        public static readonly FileExtension JSON = new FileExtension(new string[] { ".json" }, "JavaScript Object Notation");
        public static readonly FileExtension XML = new FileExtension(new string[] { ".xml" }, "eXtensible Markup Language");
        public static readonly FileExtension YAML = new FileExtension(new string[] { ".yaml" }, "YAML Ain't Markup Language");
        public static readonly FileExtension YML = new FileExtension(new string[] { ".yml" }, "YAML Ain't Markup Language");

        //  Documents
        public static readonly FileExtension DOC = new FileExtension(new string[] { ".doc" }, "Word Legacy Format");
        public static readonly FileExtension DOCX = new FileExtension(new string[] { ".docx" }, "Microsoft Word");
        public static readonly FileExtension CSV = new FileExtension(new string[] { ".csv" }, "Comma-Separated Values");
        public static readonly FileExtension HTML = new FileExtension(new string[] { ".html" }, "Hypertext Markup Language");
        public static readonly FileExtension ODT = new FileExtension(new string[] { ".odt" }, "OpenDocument Text");
        public static readonly FileExtension PDF = new FileExtension(new string[] { ".pdf" }, "Portable Document Format");
        public static readonly FileExtension PPT = new FileExtension(new string[] { ".ppt" }, "PowerPoint Legacy Format");
        public static readonly FileExtension PPTX = new FileExtension(new string[] { ".pptx" }, "Microsoft PowerPoint");
        public static readonly FileExtension RTF = new FileExtension(new string[] { ".rtf" }, "Rich Text Format");
        public static readonly FileExtension TXT = new FileExtension(new string[] { ".txt" }, "Plain Text");
        public static readonly FileExtension XLS = new FileExtension(new string[] { ".xls" }, "Excel Legacy Format");
        public static readonly FileExtension XLSX = new FileExtension(new string[] { ".xlsx" }, "Microsoft Excel");

        //  Executable
        public static readonly FileExtension BAT = new FileExtension(new string[] { ".bat" }, "Batch File");
        public static readonly FileExtension EXE = new FileExtension(new string[] { ".exe" }, "Executable");
        public static readonly FileExtension MSI = new FileExtension(new string[] { ".msi" }, "Microsoft Installer");
        public static readonly FileExtension PS1 = new FileExtension(new string[] { ".ps1" }, "PowerShell Script");
        public static readonly FileExtension VB = new FileExtension(new string[] { ".vb" }, "VBScript File");
        public static readonly FileExtension VBS = new FileExtension(new string[] { ".vbs" }, "VBScript File");

        //  Libraries
        public static readonly FileExtension BIN = new FileExtension(new string[] { ".bin" }, "Binary File");
        public static readonly FileExtension DLL = new FileExtension(new string[] { ".dll" }, "Dynamic Link Library");

        //  Pictures
        public static readonly FileExtension BMP = new FileExtension(new string[] { ".bmp" }, "Bitmap");
        public static readonly FileExtension GIF = new FileExtension(new string[] { ".gif" }, "Graphics Interchange Format");
        public static readonly FileExtension JPG = new FileExtension(new string[] { ".jpg" }, "Joint Photographic Experts Group");
        public static readonly FileExtension JPEG = new FileExtension(new string[] { ".jpeg" }, "Joint Photographic Experts Group");
        public static readonly FileExtension PNG = new FileExtension(new string[] { ".png" }, "Portable Network Graphics");
        public static readonly FileExtension SVG = new FileExtension(new string[] { ".svg" }, "Scalable Vector Graphics");
        public static readonly FileExtension TIFF = new FileExtension(new string[] { ".tiff" }, "Tagged Image File Format");
        public static readonly FileExtension WEBP = new FileExtension(new string[] { ".webp" }, "WebP Image");

        //  Music
        public static readonly FileExtension ACC = new FileExtension(new string[] { ".acc" }, "Advanced Audio Coding");
        public static readonly FileExtension FLAC = new FileExtension(new string[] { ".flac" }, "Free Lossless Audio Codec");
        public static readonly FileExtension M4A = new FileExtension(new string[] { ".m4a" }, "MPEG-4 Audio");
        public static readonly FileExtension MP3 = new FileExtension(new string[] { ".mp3" }, "MPEG Audio Layer III");
        public static readonly FileExtension OGG = new FileExtension(new string[] { ".ogg" }, "Ogg Vorbis");
        public static readonly FileExtension WAV = new FileExtension(new string[] { ".wav" }, "Waveform Audio File Format");
        public static readonly FileExtension WMA = new FileExtension(new string[] { ".wma" }, "Windows Media Audio");

        //  Video
        public static readonly FileExtension _3GP = new FileExtension(new string[] { ".3gp" }, "3rd Generation Partnership Project");
        public static readonly FileExtension _3GPP = new FileExtension(new string[] { ".3gpp" }, "3rd Generation Partnership Project");
        public static readonly FileExtension AVI = new FileExtension(new string[] { ".avi" }, "Audio Video Interleave");
        public static readonly FileExtension DIVX = new FileExtension(new string[] { ".divx" }, "DivX Video");
        public static readonly FileExtension M4V = new FileExtension(new string[] { ".m4v" }, "MPEG-4 Video");
        public static readonly FileExtension MKV = new FileExtension(new string[] { ".mkv" }, "Matroska Multimedia Container");
        public static readonly FileExtension MOV = new FileExtension(new string[] { ".mov" }, "Apple QuickTime Movie");
        public static readonly FileExtension MP4 = new FileExtension(new string[] { ".mp4" }, "MPEG-4 Part 14");
        public static readonly FileExtension MPEG = new FileExtension(new string[] { ".mpeg" }, "Moving Picture Experts Group");
        public static readonly FileExtension MPG = new FileExtension(new string[] { ".mpg" }, "Moving Picture Experts Group");
        public static readonly FileExtension OGV = new FileExtension(new string[] { ".ogv" }, "Ogg Video");
        public static readonly FileExtension RM = new FileExtension(new string[] { ".rm" }, "RealMedia Variable Bitrate");
        public static readonly FileExtension RMVB = new FileExtension(new string[] { ".rmvb" }, "RealMedia Variable Bitrate");
        public static readonly FileExtension VOB = new FileExtension(new string[] { ".vob" }, "DVD Video Object");
        public static readonly FileExtension WEBM = new FileExtension(new string[] { ".webm" }, "WebM");
        public static readonly FileExtension WMV = new FileExtension(new string[] { ".wmv" }, "Windows Media Video");


        public static readonly FileExtension ConfigFiles = new FileExtension(
            new string[] { ".cfg", ".ini", ".json", ".xml", ".yaml", ".yml" },
            "Configuration Files");

        public static readonly FileExtension DocumentFiles = new FileExtension(
            new string[] { ".doc", ".docx", ".csv", ".html", ".odt", ".pdf", ".ppt", ".pptx", ".rtf", ".txt", ".xls", ".xlsx"},
            "Document Files");

        public static readonly FileExtension ExecutableFiles = new FileExtension(
            new string[] { ".bat", ".exe", ".msi", ".ps1", ".vb", ".vbs" },
            "Executable Files");

        public static readonly FileExtension ImageFiles = new FileExtension(
            new string[] { ".bmp", ".gif", ".jpg", ".jpeg", ".png", ".svg", ".tiff", ".webp" },
            "Image Files");

        public static readonly FileExtension LibraryFiles = new FileExtension(
            new string[] { ".bin", ".dll" },
            "Library Files");

        public static readonly FileExtension MusicFiles = new FileExtension(
            new string[] { ".acc", ".flac", ".m4a", ".mp3", ".ogg", ".wav", ".wma" },
            "Music Files");

        public static readonly FileExtension SupportedFiles = new FileExtension(
            new string[] { ".mp3" },
            "Supported Files");

        public static readonly FileExtension VideoFiles = new FileExtension(
            new string[] { ".3gp", ".3gpp", ".avi", ".divx", ".m4v", ".mkv", ".mov", ".mp4", ".mpeg", ".mpg", ".ogv", ".rm", ".rmvb", ".vob", ".webm", ".wmv" },
            "Video Files");

    }
}
