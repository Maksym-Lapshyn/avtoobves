using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace Avtoobves.Infrastructure
{
    public abstract class BaseRepository
    {
        protected const string BucketName = "avtoobves-images";
        protected const string CdnAddress = "https://d1ucqhcti31ovy.cloudfront.net";
        
        protected readonly AWSCredentials AwsCredentials;
        protected readonly Context Context;

        protected BaseRepository(Context context, IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AwsCredentials");
            var accessKey = awsSection.GetValue<string>("AccessKey");
            var secretKey = awsSection.GetValue<string>("SecretKey");
            AwsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            Context = context;
        }

        protected void DeleteImages(params string[] imageNames)
        {
            if (imageNames == default)
            {
                return;
            }

            using var s3Client = new AmazonS3Client(AwsCredentials, RegionEndpoint.EUCentral1);
            foreach (var imageName in imageNames)
            {
                try
                {
                    s3Client.DeleteObjectAsync(BucketName, imageName).Wait();
                }
                catch (AmazonS3Exception)
                {
                    // Object may not exist - ignore
                }
            }
        }

        protected AmazonS3Client CreateS3Client()
        {
            return new AmazonS3Client(AwsCredentials, RegionEndpoint.EUCentral1);
        }

        protected TransferUtility CreateTransferUtility(AmazonS3Client s3Client)
        {
            return new TransferUtility(s3Client);
        }
    }
} 