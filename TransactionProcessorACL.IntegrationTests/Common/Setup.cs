using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessor.IntegrationTests.Common
{
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using global::TransactionProcessing.Shared.Logger;
    using Microsoft.Data.SqlClient;
    using NLog;
    using Reqnroll;
    using Shouldly;

    [Binding]
    public class Setup
    {
        public static (String usename, String password) SqlCredentials = ("sa", $"Aa1!{Convert.ToHexString(RandomNumberGenerator.GetBytes(24))}");

        public static (String url, String username, String password)? DockerCredentials
        {
            get
            {
                String? username = Environment.GetEnvironmentVariable("DOCKER_USERNAME");
                String? password = Environment.GetEnvironmentVariable("DOCKER_PASSWORD");

                return String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password)
                    ? null
                    : ("https://www.docker.com", username, password);
            }
        }

        public static async Task GlobalSetup(DockerHelper dockerHelper)
        {
            ShouldlyConfiguration.DefaultTaskTimeout = TimeSpan.FromMinutes(5);
        }
    }
}
