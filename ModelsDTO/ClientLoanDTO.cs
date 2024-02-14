namespace HomeBankingMindHub.ModelsDTO
{
    public class ClientLoanDTO
    {
        public long LoanId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int Payments { get; set; }
    }
}
