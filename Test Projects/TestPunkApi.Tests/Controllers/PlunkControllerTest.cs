using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestPunkApi.Controllers;
using TestPunkApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace TestPunkApi.Tests.Controllers
{
    [TestClass]
    public class PlunkControllerTest
    {
        [TestMethod]
        public void GetAllBeers()
        {
            PlunkApiController controller = new PlunkApiController();
            var response = controller.GetAllBeers("Trashy Blonde");
            Assert.IsTrue(response.TryGetContentValue(out Beer beer));
            Assert.IsNotNull(beer);
            Assert.AreEqual("2", beer.Id);
            Assert.AreEqual("Trashy Blonde", beer.name);
        }        

        [TestMethod]
        public void AddRatingToBeer()
        {            
            PlunkApiController controller = new PlunkApiController();
            var response = controller.AddRatingToBeer(new BeerRating { UserName="sandy@rdy.com",Rating=5, BeerId = 2,Comments="ttest" });
            Assert.IsTrue(response.TryGetContentValue(out string result));
            Assert.AreEqual("success", result);
        }
    }
}
