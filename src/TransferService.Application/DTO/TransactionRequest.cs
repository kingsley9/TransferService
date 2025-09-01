namespace TransferService.Application.DTO
{
    public class TransactionRequest
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }

        public int? TargetAccountId { get; set; }
        public string Pin { get; set; } = String.Empty;
    }
}
