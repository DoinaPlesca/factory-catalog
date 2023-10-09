using Dapper;
// using FluentAssertions;
// using infrastructure.DataModels;
// using Microsoft.Playwright;
// using Microsoft.Playwright.NUnit;
// using Newtonsoft.Json;
// using NUnit.Framework;
// using Tests;
// namespace test;
//
//
// [TestFixture]
// public class DeleteBox : PageTest
// {
//     [TestCase("Test delete", "description of boxes", "https://www.unfade.dk/wp-content/uploads/2022/12/Mystery-Box-Sprayer_All_5039_5.jpeg", "small", 10)]
//     public async Task BoxCanSuccessfullyBeDeletedFromUi(string boxName, string description, string imageUrl, string size,int price)
//     {
//         
//         Helper.TriggerRebuild();
//         await using (var conn = await Helper.DataSource.OpenConnectionAsync())
//         {
//             conn.QueryFirst<Box>(
//                 " INSERT INTO factory_catalog.boxes (boxName,description,price,size,imageUrl) VALUES (@boxName, @description, @price, @size,@imageUrl) RETURNING *;",
//                 new { boxName,description,price,size,imageUrl});
//         }
//         
//         
//         
//     }
// }
