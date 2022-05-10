using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class PlatformOptions
    {
        public bool CacheEnabled { get; set; } = true;

        public TimeSpan? CacheAbsoluteExpiration { get; set; }
        public TimeSpan? CacheSlidingExpiration { get; set; }

        [Required]
        public string LocalUploadFolderPath { get; set; } = "App_Data/Uploads";

        public int PageSizeMaxValue { get; set; } = 100;
    }

}
