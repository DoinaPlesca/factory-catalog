using System.Net;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using infrastructure.DataModels;
using Newtonsoft.Json;
using Tests;

namespace test;

[TestFixture]
public class UpdateBox : PageTest
{
    [TestCase("Chocolate box", "Marcipan chocolate", "https://unboxprofi.at/wp-content/uploads/2022/02/IMG_6111.jpg", "medium", 200)]
    public async Task BoxCanSuccessfullyBeUpdatedFromUI(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            //Insert an article to update
            conn.QueryFirst<Box>(
                "INSERT INTO factory_catalog.boxes  (boxName,description,size,price,imageUrl) VALUES (@boxName, @description, @size, @price, @imageUrl) RETURNING *;",
                new { boxName,description,size,price,imageUrl });
        }
        
        await Page.GotoAsync(Helper.ClientAppBaseUrl);
        await Page.GetByTestId("row_" + boxName).ClickAsync();
        
        await Page.GetByTestId("open-page-edit").ClickAsync();
        
        // now we are inside the edit 
        await Page.GetByTestId("edit_name_form").Locator("input").FillAsync(boxName);
        await Page.GetByTestId("edit_description_form").Locator("input").FillAsync(description);
        await Page.GetByTestId("edit_size_form").Locator("input").FillAsync(size);
        await Page.GetByTestId("edit_price_form").Locator("input").FillAsync(price.ToString());
        await Page.GetByTestId("edit_imageUrl_form").Locator("input").FillAsync(imageUrl);
        await Page.GetByTestId("edit_submit_form").ClickAsync();
        
        //BOX in DB is as is expected
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.QueryFirst<Box>("SELECT * FROM factory_catalog.boxes ").Should()
                .BeEquivalentTo(new Box()
                    { BoxId = 1, BoxName = boxName, Description = description, Size = size, Price = price, ImageUrl = imageUrl});
        }
        await Expect(Page.GetByTestId("row_" + boxName)).ToBeVisibleAsync();
    }
    
    //API test: Now we're not using the frontend, so we're "isolating" from the API layer and down (just using HttpClient, no Playwright)
    [TestCase("Chocolate box", "This is test box 1", "https://unboxprofi.at/wp-content/uploads/2022/02/IMG_6111.jpg", "small", 200)]
    [TestCase("Cardboard box", "This is text 30", "https://unboxprofi.at/wp-content/uploads/2022/02/IMG_6111.jpg", "medium", 200)]
    [TestCase("Flat box", "This is test box 2", "https://unboxprofi.at/wp-content/uploads/2022/02/IMG_6111.jpg", "medium", 200)]
    public async Task BoxCanSuccessfullyBeUpdatedFromHttpRequest(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.QueryFirst<Box>(
                "INSERT INTO factory_catalog.boxes  (boxName,description,size,price,imageUrl) VALUES (@boxName, @description, @size, @price, @imageUrl) RETURNING *;",
                new { boxName,description,size,price,imageUrl });
        }

        var testBox = new Box()
        {
            BoxId = 1, BoxName = boxName, Description = description, ImageUrl = imageUrl, Size = size,Price = price
        };
        
        //ACT
        var httpResponse = await new HttpClient().PutAsJsonAsync(Helper.ApiBaseUrl + "/boxes/1", testBox);
        var boxFromResponseBody =
            JsonConvert.DeserializeObject<Box>(await httpResponse.RequestMessage?.Content?.ReadAsStringAsync()!);
        
        //ASSERT
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.QueryFirst<Box>("SELECT * FROM factory_catalog.boxes;").Should()
                .BeEquivalentTo(boxFromResponseBody); //Should be equal to article found in DB
        }
    }
    
    // Should reject due to invalid url 
    [TestCase("Box length 5", "this is test box from tests", "3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "small", 100)]
    [TestCase("Box length 10", "this is test box from tests", "0by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "small", 100)]
    public async Task ServerSideDataValidationShouldRejectBadValues(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();
        
        var testBox = new Box()
        {
            BoxId = 1,
            BoxName = boxName, 
            Description = description, 
            Size = size,
            Price = price,
            ImageUrl = imageUrl 
        };
        
        //ACT
        var httpResponse = await new HttpClient().PutAsJsonAsync(Helper.ApiBaseUrl + "/box", testBox);
        
        //ASSERT
        httpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        await using var conn = await Helper.DataSource.OpenConnectionAsync();
        
        conn.ExecuteScalar<int>("SELECT COUNT(*) FROM factory_catalog.boxes;").Should()
            .Be(0); //DB should be empty when create failed
    }
    
     [TestCase("Box", "this is test box without correct size", "https://images.unsplash.com/photo-1696257203553-20ada15fce65?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "small", 100)]
    public async Task UIShouldDisableUpdateBtnWhenIncorrectNameLength(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
            
        await conn.ExecuteAsync(
                "INSERT INTO factory_catalog.boxes (boxName, description, imageUrl, size,price) VALUES (@boxName, @description, @imageUrl, @size,@price) RETURNING *;",
                new { boxName, description, imageUrl, size ,price});
        
        //ACT
        await Page.GotoAsync(Helper.ClientAppBaseUrl);
        await Page.GetByTestId("row_" + boxName).ClickAsync();
        
        await Page.GetByTestId("open-page-edit").ClickAsync();
        // now we are inside the edit 
        await Page.GetByTestId("edit_name_form").Locator("input").FillAsync(boxName);
        await Page.GetByTestId("edit_description_form").Locator("input").FillAsync(description);
        await Page.GetByTestId("edit_size_form").Locator("input").FillAsync(size);
        await Page.GetByTestId("edit_price_form").Locator("input").FillAsync(price.ToString());
        await Page.GetByTestId("edit_imageUrl_form").Locator("input").FillAsync(imageUrl);
        
        await Expect(Page.GetByTestId("edit_submit_form")).ToHaveAttributeAsync("aria-disabled", "true");
    }


    
    
}
    