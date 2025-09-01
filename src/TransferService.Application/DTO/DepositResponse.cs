namespace TransferService.Application.DTO
{
    public class DepositResponse
    {
        public int TransactionId { get; set; }
        public decimal NewBalance { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = "Completed";
    }
}
