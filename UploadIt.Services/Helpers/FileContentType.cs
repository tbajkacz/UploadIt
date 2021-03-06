﻿using Microsoft.AspNetCore.StaticFiles;

namespace UploadIt.Services.Helpers
{
    public class FileContentType
    {
        public static string Get(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
