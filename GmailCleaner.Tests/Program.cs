// See https://aka.ms/new-console-template for more information
using GmailCleaner.Common;
using GmailCleaner.Common.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");
DbContextOptionsBuilder<GmailCleanerContext> options = new();
options.UseSqlServer("Data Source=.;Initial Catalog=GmailCleaner;Integrated Security=true;TrustServerCertificate=true;");
//using (GmailCleanerContext context = new(options.Options))
//{
//    context.Database.EnsureCreated();
//    GCUser Bob = new GCUser() { Name = "Bob", Email = "bob.gmail.com" };
//    GCUserToken gCUserToken = new GCUserToken()
//    {
//        AccessToken = "asdfasdf",
//        ExpiresOn = DateTime.Now,
//        IdToken = "asdfasdf",
//        RefreshToken = "asdfasdf",
//    };
//    Bob.GCUserTokens.Add(gCUserToken);
//    context.Gcusers.Add(Bob);
//    context.SaveChanges();

//}
using (GmailCleanerContext context = new(options.Options))
{
    int tokenId = 0;
    GCUserToken? token = context.GcuserTokens.Where(t => t.UserTokenId == tokenId).Include(x => x.User).FirstOrDefault(x => x.UserTokenId == tokenId);
    GCUser? user = token.User;
}


    Console.ReadLine();