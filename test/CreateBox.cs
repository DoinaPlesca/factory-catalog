using System.Net;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using infrastructure.DataModels;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Newtonsoft.Json;
using NUnit.Framework;
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
            .BeEquivalentTo(expected); 
    }
    
    [TestCase("Box", "this is test box from tests", "https://images.unsplash.com/photo-1696257203553-20ada15fce65?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "small", 100)]
    public async Task UIShouldPresentErrorToastIncorrectHeadlineRequires5Length(string boxName, string description, string imageUrl, string size,int price)
    {
        //ARRANGE
        Helper.TriggerRebuild();
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
            
        await conn.ExecuteAsync(
                "INSERT INTO factory_catalog.boxes (boxName, description, imageUrl, size,price) VALUES (@boxName, @description, @imageUrl, @size,@price) RETURNING *;",
                new { boxName, description, imageUrl, size ,price});
        
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
        var toastCssClasses = await Page.Locator("ion-toast").GetAttributeAsync("class");
        var classes = toastCssClasses?.Split(' ');
        
        classes.Should().Contain("ion-color-danger");
        
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
            
            conn.ExecuteScalar<int>("SELECT COUNT(*) FROM factory_catalog.boxes;").Should().Be(1); //DB should have just the pre-existing article, and not also the new one
    }


    [TestCase("VinBox", "6 wall", "small",30, "https://coolimage.com/img.jpg")]
    public async Task BoxCanSuccessfullyBeCreatedFromHttpRequest(string boxName, string description,string size,int price, string imageUrl)
    
    {
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
        
        var httpResponse = await new HttpClient().PostAsJsonAsync(Helper.ApiBaseUrl + "/boxes", testBox);
        
        var boxFromResponseBody =
            JsonConvert.DeserializeObject<Box>(await httpResponse.RequestMessage?.Content?.ReadAsStringAsync()!);

        //ASSERT
        await using var conn = await Helper.DataSource.OpenConnectionAsync();
        
        conn.QueryFirst<Box>("SELECT * FROM factory_catalog.boxes;").Should()
            .BeEquivalentTo(boxFromResponseBody);
    }
    
    
    [TestCase("ss", "this is test box from tests", "3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "not small", 100)]
    [TestCase("ddd", "this is test box from tests", "0by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80", "not big", 100)]
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
        var httpResponse = await new HttpClient().PostAsJsonAsync(Helper.ApiBaseUrl + "/box", testBox);
        //ASSERT
        httpResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        await using var conn = await Helper.DataSource.OpenConnectionAsync();
        
        conn.ExecuteScalar<int>("SELECT COUNT(*) FROM factory_catalog.boxes;").Should()
            .Be(0); //DB should be empty when create failed
    }
    

}