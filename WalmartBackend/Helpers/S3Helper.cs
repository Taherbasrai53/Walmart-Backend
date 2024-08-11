using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace WalmartBackend.Helpers
{
    public interface IS3Helper
    {
        Task<string> UploadFileAsync(IFormFile file, string bucketName, string productname);
    }

    public class S3Helper:IS3Helper
    {
        private readonly IAmazonS3 _s3Client;

        public S3Helper(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string bucketName, string productname)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            using (var newMemoryStream = new MemoryStream())
            {
                file.CopyTo(newMemoryStream);

                // Generate a consistent key
                string uniqueFileName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{productname}_{file.FileName}";

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = uniqueFileName,
                    BucketName = bucketName
                };

                await fileTransferUtility.UploadAsync(uploadRequest);

                string fileUrl = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = uniqueFileName,
                    Expires = DateTime.UtcNow.AddYears(1) // Adjust expiration as needed
                });

                return fileUrl;
            }
        }



    }
}
