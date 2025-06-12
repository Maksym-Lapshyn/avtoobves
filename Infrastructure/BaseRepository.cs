using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace Avtoobves.Infrastructure
{
    public abstract class BaseRepository
    {
        private readonly AWSCredentials _awsCredentials;
        
        protected const string BucketName = "avtoobves-images";
        protected const string CdnAddress = "https://d1ucqhcti31ovy.cloudfront.net";

        
        protected readonly Context Context;

        protected BaseRepository(Context context, IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AwsCredentials");
            var accessKey = awsSection.GetValue<string>("AccessKey");
            var secretKey = awsSection.GetValue<string>("SecretKey");
            _awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            Context = context;
        }

        protected async Task DeleteImagesAsync(params string[] imageNames)
        {
            if (imageNames == default)
            {
                return;
            }

            using var s3Client = new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);
            
            foreach (var imageName in imageNames)
            {
                try
                {
                    await s3Client.DeleteObjectAsync(BucketName, imageName);
                }
                catch (AmazonS3Exception)
                {
                    // Object may not exist - ignore
                }
            }
        }

        protected AmazonS3Client CreateS3Client()
        {
            return new AmazonS3Client(_awsCredentials, RegionEndpoint.EUCentral1);
        }

        protected static TransferUtility CreateTransferUtility(AmazonS3Client s3Client)
        {
            return new TransferUtility(s3Client);
        }
    }
} 