using Dapper;
using FluentAssertions;
using infrastructure.DataModels;
using Tests;
namespace test;


[TestFixture]
public class DeleteBox : PageTest
{
    [TestCase("Test delete", "description of boxes", "https://www.unfade.dk/wp-content/uploads/2022/12/Mystery-Box-Sprayer_All_5039_5.jpeg", "small", 10)]
    public async Task BoxCanSuccessfullyBeDeletedFromUi(string boxName, string description, string imageUrl, string size,int price)
    {
        // Rebuild
        Helper.TriggerRebuild();
        
        // Open cnn
        // Inserting Box that we want to delete
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.QueryFirst<Box>(
                " INSERT INTO factory_catalog.boxes (boxName,description,price,size,imageUrl) VALUES (@boxName, @description, @price, @size,@imageUrl) RETURNING *;",
                new { boxName,description,price,size,imageUrl});
        }
        
        await Page.GotoAsync(Helper.ClientAppBaseUrl);
        var row = Page.GetByTestId("row_" + boxName);
        await Page.GetByTestId("delete-box-action").ClickAsync();
        
        //ASSERT
        await Expect(row).Not.ToBeVisibleAsync(); //Article card is now nowhere to be found (notice the "Not" part)
        
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.ExecuteScalar<int>("SELECT COUNT(*) FROM factory_catalog.boxes;").Should()
                .Be(0); //And the article is also gone from the DB
        }
    }
    
    [TestCase("Test delete", "description of boxes", "https://www.unfade.dk/wp-content/uploads/2022/12/Mystery-Box-Sprayer_All_5039_5.jpeg", "small", 10)]
    public async Task ArticleCanSuccessfullyBeDeletedFromHttpClient(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();
        //Insert an article to remove from UI
        
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.QueryFirst<Box>(
                " INSERT INTO factory_catalog.boxes (boxName,description,price,size,imageUrl) VALUES (@boxName, @description, @price, @size,@imageUrl) RETURNING *;",
                new { boxName,description,price,size,imageUrl});
        }

        //ACT
        var httpResponse = await new HttpClient().DeleteAsync(Helper.ApiBaseUrl + "/boxes/1");
        
        //ASSERT
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            httpResponse.Should().BeSuccessful();
            conn.ExecuteScalar<int>("SELECT COUNT(*) FROM factory_catalog.boxes;").Should()
                .Be(0); //DB should be empty when create failed
        }
    }
}
