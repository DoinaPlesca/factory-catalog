using Dapper;
using FluentAssertions;
using infrastructure.DataModels;
using Tests;

namespace test;

[TestFixture]
public class UpdateBox : PageTest
{
    [TestCase("Chocolate box", "Marcipan chocolate", "https://unboxprofi.at/wp-content/uploads/2022/02/IMG_6111.jpg", "medium", 200)]
    public async Task BoxCanSuccessfullyBeUpdated(string boxName, string description, string imageUrl, string size,int price)
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
        await Page.GetByTestId("row_"+boxName).ClickAsync();
        await Page.GetByTestId("open-modal-edit").ClickAsync();
        await Page.GetByTestId("edit_name_form").Locator("input").FillAsync(boxName);
        await Page.GetByTestId("edit_description_form").Locator("input").FillAsync(description);
        await Page.GetByTestId("edit_size_form").Locator("input").FillAsync(size);
        await Page.GetByTestId("edit_price_form").Locator("input").FillAsync(price.ToString());
        await Page.GetByTestId("edit_imageUrl_form").Locator("input").FillAsync(imageUrl);
        await Page.GetByTestId("edit_submit_form").ClickAsync();
        await Page.GotoAsync(Helper.ClientAppBaseUrl);

        //BOX in DB is as is expected
        await using (var conn = await Helper.DataSource.OpenConnectionAsync())
        {
            conn.QueryFirst<Box>("SELECT * FROM factory_catalog.boxes ").Should()
                .BeEquivalentTo(new Box()
                    { BoxId = 1, BoxName = boxName, Description = description, Size = size, Price = price, ImageUrl = imageUrl});
        }

        
        await Expect(Page.GetByTestId("row_" + boxName)).ToBeVisibleAsync();
    }
}
    