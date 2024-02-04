namespace HomeBankingMinHub.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client {  FirstName="Victor", LastName="Coronado",Email = "vcoronado@gmail.com", Password="123456"}
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

        }
    }
}
