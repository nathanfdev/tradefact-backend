using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Tradefact.Api.Infrastructure.Helpers
{
    public static class FileHelpers
    {
        // If you require a check on specific characters in the IsValidFileExtensionAndSignature
        // method, supply the characters in the _allowedChars field.
        private static readonly byte[] AllowedChars = { };
        // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
        // and the official specifications for the file types you wish to add.
        private static readonly Dictionary<string, List<byte[]>> FileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".doc", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A , 0xE1 },
                    new byte[] { 0x0D, 0x44, 0x4F, 0x43 },
                    new byte[] { 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00 },
                    new byte[] { 0xDB, 0xA5, 0x2D, 0x00 },
                    new byte[] { 0xEC, 0xA5, 0xC1, 0x00 },
                }
            },
            { ".docx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 },
                }
            },
            { ".xls", new List<byte[]>
                {
                    new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1},
                    new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x10},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x1F},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x22},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x23},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x28},
                    new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x29},
                }
            },
            { ".xlsx", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04},
                    new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00},
                }
            },
            { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
            { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },
            { ".bmp", new List<byte[]>
                {
                    new byte[] { 0x42, 0x4D },
                }
            },
            { ".zip", new List<byte[]>
                {
                    new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                    new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45 },
                    new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                    new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                    new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                    new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
                }
            },
            { ".mp4", new List<byte[]>
                {
                    new byte[] { 0, 0, 0, 0x20, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D },
                    new byte[] { 0, 0, 0, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 },
                }
            },
            { ".aac", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xF1 },
                    new byte[] { 0xFF, 0xF9 },
                }
            },
            { ".mp3", new List<byte[]>
                {
                    new byte[] { 0x49, 0x44, 0x33 },
                }
            },
            { ".wav", new List<byte[]>
                {
                    new byte[] { 0x52, 0x49, 0x46, 0x46 },
                }
            },
            { ".wma", new List<byte[]>
                {
                    new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 },
                }
            },
            { ".mpg", new List<byte[]>
                {
                    new byte[] { 0, 0, 0x01, 0xBA },
                    new byte[] { 0, 0, 0x01, 0xB3 },
                }
            },
            { ".mpeg", new List<byte[]>
                {
                    new byte[] { 0, 0, 0x01, 0xBA },
                    new byte[] { 0, 0, 0x01, 0xB3 },
                }
            },
            { ".avi", new List<byte[]>
                {
                    new byte[] { 0x52, 0x49, 0x46, 0x46 },
                }
            }
        };


        public static async Task<byte[]> ProcessStreamedFile(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition,
            ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await section.Body.CopyToAsync(memoryStream);

                    // Check if the file is empty or exceeds the size limit.
                    if (memoryStream.Length == 0)
                    {
                        modelState.AddModelError("File", "The file is empty.");
                    }
                    else if (memoryStream.Length > sizeLimit)
                    {
                        var megabyteSizeLimit = sizeLimit / 1048576;
                        modelState.AddModelError("File", $"The file exceeds {megabyteSizeLimit:N1} MB.");
                    }
                    else if (!IsValidFileExtensionAndSignature(contentDisposition.FileName.Value, memoryStream, permittedExtensions))
                    {
                        modelState.AddModelError("File", "The file type isn't permitted or the file's signature doesn't match the file's extension.");
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("File", $"The upload failed. Please contact the Help Desk for support. Error: {ex.HResult}");
                // Log the exception
            }

            return new byte[0];
        }

        private static bool IsValidFileExtensionAndSignature(string fileName, Stream data, IEnumerable<string> permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            data.Position = 0;

            using (var reader = new BinaryReader(data))
            {
                if (ext.Equals(".txt") || ext.Equals(".csv") || ext.Equals(".prn"))
                {
                    if (AllowedChars.Length == 0)
                    {
                        // Limits characters to ASCII encoding.
                        for (var i = 0; i < data.Length; i++)
                        {
                            if (reader.ReadByte() > sbyte.MaxValue)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // Limits characters to ASCII encoding and
                        // values of the _allowedChars array.
                        for (var i = 0; i < data.Length; i++)
                        {
                            var b = reader.ReadByte();
                            if (b > sbyte.MaxValue ||
                                !AllowedChars.Contains(b))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                // Uncomment the following code block if you must permit
                // files whose signature isn't provided in the _fileSignature
                // dictionary. We recommend that you add file signatures
                // for files (when possible) for all file types you intend
                // to allow on the system and perform the file signature
                // check.

                //No file signatures found for the extensions below
                List<string> extsWithNoFileSignature = new List<string> { ".ac3", ".dts", ".mov", ".m2ts" };

                if (extsWithNoFileSignature.Contains(ext) && !FileSignature.ContainsKey(ext))
                {
                    return true;
                }


                // File signature check
                // --------------------
                // With the file signatures provided in the _fileSignature
                // dictionary, the following code tests the input content's
                // file signature.
                var signatures = FileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
