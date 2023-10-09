using Dapper;
using FluentAssertions;
using infrastructure.DataModels;
using Tests;

namespace test;

[TestFixture]
public class CreateBox : PageTest
{
    // E2E testing methods -> 
    [TestCase("Box from UI test API", "this is test box from tests", "https://images.unsplash.com/photo-1696257203553-20ada15fce65?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "small", 100)]
    [TestCase("Box Test api v2", "this is another test", "https://images.unsplash.com/photo-1696621690997-14ebd5519c22?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxlZGl0b3JpYWwtZmVlZHw3fHx8ZW58MHx8fHx8&auto=format&fit=crop&w=500&q=60", "medium", 300)]
    public async Task BoxCanSuccessfullyBeCreatedFromUi(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();

        //ACT
        await Page.GotoAsync(Helper.ClientAppBaseUrl);
        await Page.GetByTestId("open-modal-action").ClickAsync();
        
        await Page.GetByTestId("box-name-modal").Locator("input").FillAsync(boxName);
        await Page.GetByTestId("box-name-description").Locator("input").FillAsync(description);
        await Page.GetByTestId("box-name-size").Locator("input").FillAsync(size);
        await Page.GetByTestId("box-name-price").Locator("input").FillAsync(price.ToString());
        await Page.GetByTestId("box-name-image-url").Locator("input").FillAsync(imageUrl);
        
        
        await Page.GetByTestId("create-box-action").ClickAsync();


        //ASSERT
        await Expect(Page.GetByTestId("row_" + boxName)).ToBeVisibleAsync();

        await using var conn = await Helper.DataSource.OpenConnectionAsync();

        var expected = new Box()
        {
            BoxId = 1, BoxName = boxName, Description = description, ImageUrl = imageUrl, Size = size,Price = price
        };
        
        conn.QueryFirst<Box>("SELECT * FROM factory_catalog.boxes;").Should()
            .BeEquivalentTo(expected); //Should be equal to article found in DB
    }

    //  Testing methods -> 
    
    
}