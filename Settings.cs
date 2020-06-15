using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeStorage
{
    static class Settings
    {
        public static readonly string HOST = "192.168.56.56";
        public static readonly string NAME = "dmitry";
        public static readonly string KEY = "1";

        public static readonly string ENCRYPTION_EXTENSION = ".cyph";
        public static readonly string BACKUP_FILENAME = "backup.zip";
        public static readonly string HOME_DIR = "/upload";
        public static readonly string RUN_REG_SCRIPT = "sudo sh addUser.sh";
    }
}
