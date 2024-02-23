﻿using HomeBankingMindHub.Lib;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DataBase
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {   new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password= Hashing.HashPassword("123456")},
                    new Client {  FirstName="Martin", LastName="Flores",Email = "juanmartinf1998@gmail.com", Password= Hashing.HashPassword("123456")}
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }
            if (!context.Accounts.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                var accountExtra = context.Clients.FirstOrDefault(c => c.Email == "juanmartinf1998@gmail.com");
                if (accountVictor != null && accountExtra != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 4500 },
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = "VIN002", Balance = 14000 },
                        new Account {ClientId = accountExtra.Id, CreationDate = DateTime.Now, Number = "VIN003", Balance = 20000 }
                };
                    context.Accounts.AddRange(accounts);
                    context.SaveChanges();
                }
            }
            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
                var account2 = context.Accounts.FirstOrDefault(c => c.Number == "VIN002");
                var account3 = context.Accounts.FirstOrDefault(c => c.Number == "VIN003");
                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                            new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT },

                            new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT },

                            new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },

                           };
                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }
                if (account2 != null)
                {
                    var transactions = new Transaction[]
                 {
                            new Transaction { AccountId = account2.Id, Amount = 20000, Date = DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT },

                            new Transaction { AccountId = account2.Id, Amount = -2000, Date = DateTime.Now.AddHours(-6), Description = "Pago de impuestos", Type = TransactionType.DEBIT }
                 };
                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }
                if (account3 != null)
                {
                    var transactions = new Transaction[]
                    {
                            new Transaction { AccountId = account3.Id, Amount = 15000, Date = DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type = TransactionType.CREDIT },

                            new Transaction { AccountId = account3.Id, Amount = -4000, Date = DateTime.Now.AddHours(-6), Description = "Pago de un prestamo", Type = TransactionType.DEBIT }
                    };
                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }
            }
            if (!context.Loans.Any())
            {
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                foreach (Loan loan in loans)
                {
                    context.Loans.Add(loan);
                }

                context.SaveChanges();

                var client1 = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                if (client1 != null)
                {
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                    if (loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loan2.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if (loan3 != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loan3.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }

                    context.SaveChanges();

                }
            }
            if (!context.Cards.Any())
            {
                //buscamos al unico cliente
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                if (client1 != null)
                {
                    //le agregamos 2 tarjetas de crédito una GOLD y una TITANIUM, de tipo DEBITO Y CREDITO RESPECTIVAMENTE
                    var cards = new Card[]
                    {
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.GOLD,
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(4),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT,
                            Color = CardColor.TITANIUM,
                            Number = "2234-6745-552-7888",
                            Cvv = 750,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        },
                    };

                    foreach (Card card in cards)
                    {
                        context.Cards.Add(card);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}