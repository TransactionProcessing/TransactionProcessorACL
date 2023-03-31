namespace TransactionProcessorACL.Factories
{
    using DataTransferObjects.Responses;
    using Models;
    using TransactionProcessor.DataTransferObjects;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.Factories.IModelFactory" />
    public class ModelFactory : IModelFactory
    {
        #region Methods

        public LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse)
        {
            if (processLogonTransactionResponse == null)
            {
                return null;
            }

            LogonTransactionResponseMessage logonTransactionResponseMessage = new LogonTransactionResponseMessage();

            logonTransactionResponseMessage.ResponseMessage = processLogonTransactionResponse.ResponseMessage;
            logonTransactionResponseMessage.ResponseCode = processLogonTransactionResponse.ResponseCode;
            logonTransactionResponseMessage.MerchantId = processLogonTransactionResponse.MerchantId;
            logonTransactionResponseMessage.EstateId = processLogonTransactionResponse.EstateId;

            return logonTransactionResponseMessage;
        }

        public SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse)
        {
            if (processSaleTransactionResponse == null)
            {
                return null;
            }

            SaleTransactionResponseMessage saleTransactionResponseMessage = new SaleTransactionResponseMessage();

            saleTransactionResponseMessage.ResponseMessage = processSaleTransactionResponse.ResponseMessage;
            saleTransactionResponseMessage.ResponseCode = processSaleTransactionResponse.ResponseCode;
            saleTransactionResponseMessage.MerchantId = processSaleTransactionResponse.MerchantId;
            saleTransactionResponseMessage.EstateId = processSaleTransactionResponse.EstateId;
            saleTransactionResponseMessage.AdditionalResponseMetaData = processSaleTransactionResponse.AdditionalTransactionMetadata;

            return saleTransactionResponseMessage;
        }

        public ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse)
        {
            if (processReconciliationResponse == null)
            {
                return null;
            }

            ReconciliationResponseMessage reconciliationResponseMessage = new ReconciliationResponseMessage();

            reconciliationResponseMessage.ResponseMessage = processReconciliationResponse.ResponseMessage;
            reconciliationResponseMessage.ResponseCode = processReconciliationResponse.ResponseCode;
            reconciliationResponseMessage.MerchantId = processReconciliationResponse.MerchantId;
            reconciliationResponseMessage.EstateId = processReconciliationResponse.EstateId;
            return reconciliationResponseMessage;
        }

        public GetVoucherResponseMessage ConvertFrom(GetVoucherResponse model)
        {
            if (model == null)
            {
                return null;
            }

            GetVoucherResponseMessage responseMessage = new GetVoucherResponseMessage
                                                        {
                                                            ContractId = model.ContractId,
                                                            EstateId = model.EstateId,
                                                            VoucherCode = model.VoucherCode,
                                                            VoucherId = model.VoucherId,
                                                            ExpiryDate = model.ExpiryDate,
                                                            Value = model.Value,
                                                            ResponseMessage = model.ResponseMessage,
                                                            ResponseCode = model.ResponseCode,
                                                            Balance = model.Balance
                                                        };

            return responseMessage;
        }

        public RedeemVoucherResponseMessage ConvertFrom(RedeemVoucherResponse model)
        {
            if (model == null)
            {
                return null;
            }

            RedeemVoucherResponseMessage responseMessage = new RedeemVoucherResponseMessage
                                                           {
                                                               VoucherCode = model.VoucherCode,
                                                               Balance = model.Balance,
                                                               ResponseMessage = model.ResponseMessage,
                                                               ContractId = model.ContractId,
                                                               EstateId = model.EstateId,
                                                               ExpiryDate = model.ExpiryDate,
                                                               ResponseCode = model.ResponseCode
                                                           };

            return responseMessage;
        }

        #endregion
    }
}