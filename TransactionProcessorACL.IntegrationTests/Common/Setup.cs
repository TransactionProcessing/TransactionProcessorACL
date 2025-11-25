using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessor.IntegrationTests.Common
{
    using System.Data;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Services;
    using Ductus.FluentDocker.Services.Extensions;
    using global::Shared.Logger;
    using Microsoft.Data.SqlClient;
    using NLog;
    using Reqnroll;
    using Shouldly;

    [Binding]
    public class Setup
    {
        public static (String usename, String password) SqlCredentials = ("sa", "thisisalongpassword123!");
        public static (String url, String username, String password) DockerCredentials = ("https://www.docker.com", "stuartferguson", "Sc0tland");

        public static async Task GlobalSetup(DockerHelper dockerHelper)
        {
            ShouldlyConfiguration.DefaultTaskTimeout = TimeSpan.FromMinutes(1);
        }
    }
}
