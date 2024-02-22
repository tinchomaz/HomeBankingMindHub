using HomeBankingMindHub.Models;
using HomeBankingMindHub.ModelsDTO;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        List<LoanDTO> GetAllLoans(out int statusCode,out string? message);
        void SaveLoan(LoanApplicationDTO loanApplicationDTO, out int statusCode, out string? message,string email);
    }
}
