namespace TransactionProcessor.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DataTransferObjects;
    using EstateManagement.IntegrationTesting.Helpers;
    using global::Shared.Logger;
    using Newtonsoft.Json;
    using Reqnroll;
    using Shared.IntegrationTesting;
    using Shouldly;

    /// <summary>
    /// 
    /// </summary>
    public class TestingContext
    {
        #region Fields

        /// <summary>
        /// The clients
        /// </summary>
        private readonly List<ClientDetails> Clients;

        /// <summary>
        /// The estates
        /// </summary>
        public readonly List<EstateDetails1> Estates;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingContext"/> class.
        /// </summary>
        public TestingContext()
        {
            this.Estates = new List<EstateDetails1>();
            this.Clients = new List<ClientDetails>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public String AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the docker helper.
        /// </summary>
        /// <value>
        /// The docker helper.
        /// </value>
        public DockerHelper DockerHelper { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public NlogLogger Logger { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the client details.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        /// <param name="grantType">Type of the grant.</param>
        public void AddClientDetails(String clientId,
                                     String clientSecret,
                                     String grantType)
        {
            this.Clients.Add(ClientDetails.Create(clientId, clientSecret, grantType));
        }

        /// <summary>
        /// Adds the estate details.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="estateName">Name of the estate.</param>
        public void AddEstateDetails(Guid estateId,
                                     String estateName,
                                     String estateReference)
        {
            this.Estates.Add(new EstateDetails1(EstateDetails.Create(estateId, estateName, estateReference)));
        }

        /// <summary>
        /// Gets the client details.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public ClientDetails GetClientDetails(String clientId)
        {
            ClientDetails clientDetails = this.Clients.SingleOrDefault(c => c.ClientId == clientId);

            clientDetails.ShouldNotBeNull();

            return clientDetails;
        }

        /// <summary>
        /// Gets the estate details.
        /// </summary>
        /// <param name="tableRow">The table row.</param>
        /// <returns></returns>
        public EstateDetails1 GetEstateDetails(DataTableRow tableRow)
        {
            String estateName = ReqnrollTableHelper.GetStringRowValue(tableRow, "EstateName");

            EstateDetails1 estateDetails = this.Estates.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);

            estateDetails.ShouldNotBeNull();

            return estateDetails;
        }

        /// <summary>
        /// Gets the estate details.
        /// </summary>
        /// <param name="estateName">Name of the estate.</param>
        /// <returns></returns>
        public EstateDetails1 GetEstateDetails(String estateName)
        {
            EstateDetails1 estateDetails = this.Estates.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);

            estateDetails.ShouldNotBeNull();

            return estateDetails;
        }

        /// <summary>
        /// Gets the estate details.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <returns></returns>
        public EstateDetails1 GetEstateDetails(Guid estateId)
        {
            EstateDetails1 estateDetails = this.Estates.SingleOrDefault(e => e.EstateDetails.EstateId == estateId);

            estateDetails.ShouldNotBeNull();

            return estateDetails;
        }

        //public async Task<GetVoucherResponse> GetVoucherByTransactionNumber(String estateName, String merchantName, Int32 transactionNumber)
        //{
        //    EstateDetails estate = this.GetEstateDetails(estateName);
        //    Guid merchantId = estate.GetMerchantId(merchantName);
        //    var serialisedMessage = estate.GetTransactionResponse(merchantId, transactionNumber.ToString(), "Sale");
        //    SaleTransactionResponse transactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponse>(serialisedMessage,
        //                                                                                                         new JsonSerializerSettings
        //                                                                                                         {
        //                                                                                                             TypeNameHandling = TypeNameHandling.All
        //                                                                                                         });
        //    GetVoucherResponse voucher = await this.DockerHelper.TransactionProcessorClient.GetVoucherByTransactionId(this.AccessToken,
        //                                                                                                              estate.EstateId,
        //                                                                                                              transactionResponse.TransactionId,
        //                                                                                                              CancellationToken.None);

        //    return voucher;
        //}

        #endregion
    }
}