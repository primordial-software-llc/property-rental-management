using System;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace Tests
{
    class Factory
    {
        public static RegionEndpoint HomeRegion => RegionEndpoint.USEast1;
        public static AWSCredentials CreateCredentialsFromProfile()
        {
            var chain = new CredentialProfileStoreChain();
            var profile = "lakeland-mi-pueblo";
            if (!chain.TryGetAWSCredentials(profile, out AWSCredentials awsCredentials))
            {
                throw new Exception($"AWS credentials not found for \"{profile}\" profile.");
            }
            return awsCredentials;
        }
    }
}
