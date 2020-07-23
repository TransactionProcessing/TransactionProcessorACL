namespace TransactionProcessor.IntegrationTests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EstateDetails
    {
        #region Fields

        /// <summary>
        /// The contracts
        /// </summary>
        private readonly List<Contract> Contracts;

        private readonly Dictionary<String, Guid> Merchants;

        private readonly Dictionary<String, Dictionary<String, String>> MerchantUsers;

        private readonly Dictionary<String, Dictionary<String, String>> MerchantUsersTokens;

        private readonly Dictionary<String, Guid> Operators;

        #endregion

        #region Constructors

        private EstateDetails(Guid estateId,
                              String estateName)
        {
            this.EstateId = estateId;
            this.EstateName = estateName;
            this.Merchants = new Dictionary<String, Guid>();
            this.Operators = new Dictionary<String, Guid>();
            this.MerchantUsers = new Dictionary<String, Dictionary<String, String>>();
            this.MerchantUsersTokens = new Dictionary<String, Dictionary<String, String>>();
            this.TransactionResponses = new Dictionary<(Guid merchantId, String transactionNumber, String transactionType), String>();
            this.Contracts = new List<Contract>();
        }

        #endregion

        #region Properties

        public String AccessToken { get; private set; }

        public Guid EstateId { get; }

        public String EstateName { get; }

        public String EstatePassword { get; private set; }

        public String EstateUser { get; private set; }

        private Dictionary<(Guid merchantId, String transactionNumber, String transactionType), String> TransactionResponses { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the contract.
        /// </summary>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <param name="operatorId">The operator identifier.</param>
        public void AddContract(Guid contractId,
                                String contractName,
                                Guid operatorId)
        {
            this.Contracts.Add(new Contract
                               {
                                   ContractId = contractId,
                                   Description = contractName,
                                   OperatorId = operatorId,
                               });
        }

        public void AddMerchant(Guid merchantId,
                                String merchantName)
        {
            this.Merchants.Add(merchantName, merchantId);
        }

        public void AddMerchantUser(String merchantName,
                                    String userName,
                                    String password)
        {
            if (this.MerchantUsers.ContainsKey(merchantName))
            {
                Dictionary<String, String> merchantUsersList = this.MerchantUsers[merchantName];
                if (merchantUsersList.ContainsKey(userName) == false)
                {
                    merchantUsersList.Add(userName, password);
                }
            }
            else
            {
                Dictionary<String, String> merchantUsersList = new Dictionary<String, String>();
                merchantUsersList.Add(userName, password);
                this.MerchantUsers.Add(merchantName, merchantUsersList);
            }
        }

        public void AddMerchantUserToken(String merchantName,
                                         String userName,
                                         String token)
        {
            if (this.MerchantUsersTokens.ContainsKey(merchantName))
            {
                Dictionary<String, String> merchantUsersList = this.MerchantUsersTokens[merchantName];
                if (merchantUsersList.ContainsKey(userName) == false)
                {
                    merchantUsersList.Add(userName, token);
                }
            }
            else
            {
                Dictionary<String, String> merchantUsersList = new Dictionary<String, String>();
                merchantUsersList.Add(userName, token);
                this.MerchantUsersTokens.Add(merchantName, merchantUsersList);
            }
        }

        public void AddOperator(Guid operatorId,
                                String operatorName)
        {
            this.Operators.Add(operatorName, operatorId);
        }

        public void AddTransactionResponse(Guid merchantId,
                                           String transactionNumber,
                                           String transactionType,
                                           String transactionResponse)
        {
            this.TransactionResponses.Add((merchantId, transactionNumber, transactionType), transactionResponse);
        }

        public static EstateDetails Create(Guid estateId,
                                           String estateName)
        {
            return new EstateDetails(estateId, estateName);
        }

        /// <summary>
        /// Gets the contract.
        /// </summary>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns></returns>
        public Contract GetContract(String contractName)
        {
            if (this.Contracts.Any() == false)
            {
                return null;
            }

            return this.Contracts.Single(c => c.Description == contractName);
        }

        /// <summary>
        /// Gets the contract.
        /// </summary>
        /// <param name="contractId">The contract identifier.</param>
        /// <returns></returns>
        public Contract GetContract(Guid contractId)
        {
            return this.Contracts.Single(c => c.ContractId == contractId);
        }

        public Guid GetMerchantId(String merchantName)
        {
            return this.Merchants.Single(m => m.Key == merchantName).Value;
        }

        public String GetMerchantUserToken(String merchantName)
        {
            KeyValuePair<String, Dictionary<String, String>> x = this.MerchantUsersTokens.SingleOrDefault(x => x.Key == merchantName);

            if (x.Value != null)
            {
                return x.Value.First().Value;
            }

            return string.Empty;
        }

        public Guid GetOperatorId(String operatorName)
        {
            return this.Operators.Single(o => o.Key == operatorName).Value;
        }

        public String GetTransactionResponse(Guid merchantId,
                                             String transactionNumber,
                                             String transactionType)
        {
            KeyValuePair<(Guid merchantId, String transactionNumber, String transactionType), String> transactionResponse =
                this.TransactionResponses
                    .Where(t => t.Key.merchantId == merchantId && t.Key.transactionNumber == transactionNumber && t.Key.transactionType == transactionType)
                    .SingleOrDefault();

            return transactionResponse.Value;
        }

        public void SetEstateUser(String userName,
                                  String password)
        {
            this.EstateUser = userName;
            this.EstatePassword = password;
        }

        public void SetEstateUserToken(String accessToken)
        {
            this.AccessToken = accessToken;
        }

        #endregion
    }
}