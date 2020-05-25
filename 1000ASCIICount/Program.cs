using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs; //Install Package in console: Install-Package Azure.Storage.Blobs -Version 12.4.2

namespace _1000ASCIICount
{
    class Program
    {
        static async Task Main(string[] args)
        {   /**********************************************************************************************
            * Sum the ASCII value - find the joker (228326687915660 = 329081602*693830) by @Sajan Chhipa
            ***********************************************************************************************/
            var urlToBlobContainerRepo = "https://inversionrecruitment.blob.core.windows.net/find-the-joker";
            var blobContainerUri = new Uri(urlToBlobContainerRepo);
            var blobContainerClient = new BlobContainerClient(blobContainerUri, null);
            string localPath = "./data/"; //Verify if need to create data folder after builded
            ulong totalASCIISum = 0ul;
            ulong totalJokerASCIISum = 0ul;

            await foreach (var blob in blobContainerClient.GetBlobsAsync())
            {
                string fileName = blob.Name;
                string localFilePath = Path.Combine(localPath, fileName);

                using (var file = File.Open(localFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                    await blobClient.DownloadToAsync(file);

                    if (file.CanRead)
                    {
                        file.Position = 0;

                        byte[] readBytes = new byte[file.Length];

                        // Detect joker file
                        if (file.Length >= 11000)
                        {
                            while (file.Read(readBytes, 0, readBytes.Length) > 0)
                            {
                                string asciiString = System.Text.Encoding.ASCII.GetString(readBytes);
                                foreach (var chr in asciiString)
                                {
                                    totalJokerASCIISum += (ulong)chr;
                                }
                            }
                        }
                        else
                        {
                            while (file.Read(readBytes, 0, readBytes.Length) > 0)
                            {
                                string asciiString = System.Text.Encoding.ASCII.GetString(readBytes);
                                foreach (var chr in asciiString)
                                {
                                    totalASCIISum += (ulong)chr;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("File Read: {0} sumASCIIValue: {1}, sumJokerASCIIValue: {2}", fileName, totalASCIISum, totalJokerASCIISum);
            }

            Console.WriteLine("ASCII value: {0}", totalASCIISum * totalJokerASCIISum);
        }
    }
}