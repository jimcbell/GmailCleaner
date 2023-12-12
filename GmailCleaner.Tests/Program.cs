// See https://aka.ms/new-console-template for more information
using GmailCleaner.Data;
using GmailCleaner.Entities;
using Microsoft.EntityFrameworkCore;

//string key = "shabadaba";
//string salt = "bananas";

//string encrypted = Cryptographer.Encrypt("Hello World!", key, salt);
//Console.WriteLine($"Encrypted: {encrypted}");

//string decrypted = Cryptographer.Decrypt(encrypted, key, salt);
//Console.WriteLine($"Decrypted: {decrypted}");


Console.WriteLine("Hello, World!");
string key = "1111111111111111";
byte[] bytes = Convert.FromBase64String(key);

DbContextOptionsBuilder<GmailCleanerContext> options = new();
options.UseSqlServer("Data Source=.;Initial Catalog=GmailCleaner;Integrated Security=true;TrustServerCertificate=true;");
using (GmailCleanerContext context = new(options.Options, key))
{
    //context.Database.EnsureCreated();
    //GCUser Bob = new GCUser() { GmailId = "asdfsdfas", Name = "Bob", Email = "bob.gmail.com" };
    //GCUserToken gCUserToken = new GCUserToken()
    //{
    //    AccessToken = "asdfasdf",
    //    ExpiresOn = DateTime.Now,
    //    IdToken = "asdfasdf",
    //    RefreshToken = "asdfasdf",
    //};
    //Bob.GCUserTokens.Add(gCUserToken);
    //context.GCUsers.Add(Bob);
    //context.SaveChanges();
    GCUser user = context.GCUsers.Where(x => x.Name == "Bob").Include(x => x.GCUserTokens).FirstOrDefault();    
    string name = user.Name;
}

//}
//using (GmailCleanerContext context = new(options.Options))
//{
//    int tokenId = 0;
//    GCUserToken? token = context.GcuserTokens.Where(t => t.UserTokenId == tokenId).Include(x => x.User).FirstOrDefault(x => x.UserTokenId == tokenId);
//    GCUser? user = token.User;
//}


Console.ReadLine();