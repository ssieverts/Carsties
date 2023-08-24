using MongoDB.Driver;
using MongoDB.Entities;
using System.Net.NetworkInformation;
using SearchService.Models;
using System.Text.Json;
using SearchService;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        if (app == null)
        {
            throw new ArgumentNullException("app object is null.");
        }

        var settings =
            MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection"));

        await DB.InitAsync("SearchDb", settings);

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

       using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();

        Console.WriteLine(items.Count + " returned from auction service");

        if (items.Count > 0) await DB.SaveAsync(items);

    }
}