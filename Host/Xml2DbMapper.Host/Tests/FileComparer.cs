using System;
using System.IO;
using System.Security.Cryptography;

namespace Xml2DbMapper.Host.Tests
{
    static class FileComparer
    {
        public static bool CompareFileHashes(string fileName1, string fileName2)
        {
            // Compare file sizes before continuing. 
            // If sizes are equal then compare bytes.
            if (CompareFileSizes(fileName1, fileName2))
            {
                // Create an instance of System.Security.Cryptography.HashAlgorithm
                HashAlgorithm hash = new SHA1CryptoServiceProvider();
                //                HashAlgorithm hash = HashAlgorithm.Create();

                // Declare byte arrays to store our file hashes
                byte[] fileHash1;
                byte[] fileHash2;

                // Open a System.IO.FileStream for each file.
                // Note: With the 'using' keyword the streams 
                // are closed automatically.
                using (FileStream fileStream1 = new FileStream(fileName1, FileMode.Open),
                                  fileStream2 = new FileStream(fileName2, FileMode.Open))
                {
                    // Compute file hashes
                    fileHash1 = hash.ComputeHash(fileStream1);
                    fileHash2 = hash.ComputeHash(fileStream2);
                }

                return BitConverter.ToString(fileHash1) == BitConverter.ToString(fileHash2);
            }
            else
            {
                return false;
            }
        }

        private static bool CompareFileSizes(string fileName1, string fileName2)
        {
            bool fileSizeEqual = true;

            // Create System.IO.FileInfo objects for both files
            FileInfo fileInfo1 = new FileInfo(fileName1);
            FileInfo fileInfo2 = new FileInfo(fileName2);

            // Compare file sizes
            if (fileInfo1.Length != fileInfo2.Length)
            {
                // File sizes are not equal therefore files are not identical
                fileSizeEqual = false;
            }

            return fileSizeEqual;
        }
    }
}
