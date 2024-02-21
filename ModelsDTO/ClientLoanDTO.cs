using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.ModelsDTO
{
    public class ClientLoanDTO
    {
        public long LoanId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int Payments { get; set; }

        public ClientLoanDTO(ClientLoan clientLoan)
        {
            LoanId = clientLoan.LoanId;
            Name = clientLoan.Loan.Name;
            Amount = clientLoan.Amount;
            Payments = int.Parse(clientLoan.Payments);
        }
    }
}
