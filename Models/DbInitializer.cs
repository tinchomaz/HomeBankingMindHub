using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {   new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123456"},
                    new Client {  FirstName="Martin", LastName="Flores",Email = "juanmartinf1998@gmail.com", Password="123456"}
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }
            if (!context.Account.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                var accountExtra = context.Clients.FirstOrDefault(c => c.Email == "juanmartinf1998@gmail.com");
                if (accountVictor != null && accountExtra != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = string.Empty, Balance = 0 },
                        new Account {ClientId = accountExtra.Id, CreationDate = DateTime.Now, Number = string.Empty, Balance = 1 }
                };
                context.Account.AddRange(accounts);
                context.SaveChanges();

                }
            }
        }
    }
}
