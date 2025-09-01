using TransferService.Domain.Enums;

namespace TransferService.Application.DTO
{
    public class TransactionResponse
    {
        public int TransactionId { get; set; }
        public TransactionStatus Status { get; set; }
        public decimal Balance { get; set; }
        public TransactionType Type { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
