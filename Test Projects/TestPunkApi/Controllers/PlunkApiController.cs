using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using TestPunkApi.Models;

namespace TestPunkApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("Plunk")]
    public class PlunkApiController : ApiController
    {
        readonly Uri _baseAddress = new Uri("https://api.punkapi.com/v2/");
        readonly string _data = string.Empty;
        public PlunkApiController()
        {
            var client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
            var response = client.GetAsync(client.BaseAddress + "beers").Result;
            if (response.IsSuccessStatusCode)
            {
                _data = response.Content.ReadAsStringAsync().Result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="beerRating"></param>
        /// <returns></returns>
        [ValidateEmailFilter]
        [HttpPost]
        [Route("AddRatingToBeer")]
        public HttpResponseMessage AddRatingToBeer(BeerRating beerRating)
        {
            try
            {
                JArray beers = null;
                HttpResponseMessage httpResponseMessage;
                if (!string.IsNullOrWhiteSpace(_data))
                {
                    beers = JArray.Parse(_data);
                    if (beers.All(x => x.SelectToken("id").ToString() != beerRating.BeerId.ToString()))
                    {

                        httpResponseMessage = new HttpResponseMessage()
                        {
                            Content = new ObjectContent<object>(new { message = "Please enter valid id with this no beer exists.", error = "1" }, new JsonMediaTypeFormatter()),
                            ReasonPhrase = "Invalid request",
                            StatusCode = HttpStatusCode.BadRequest
                        };
                        return httpResponseMessage;
                    }
                }
                if (beerRating != null && (beerRating.Rating == 0 || beerRating.Rating > 5))
                {
                    httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<object>(new { message = "Please enter rating range between 1 to 5 only.", error = "1" }, new JsonMediaTypeFormatter()),
                        ReasonPhrase = "Invalid request",
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return httpResponseMessage;
                }
                var jObject = (JObject)beers.FirstOrDefault(x => beerRating != null && x.SelectToken("id").ToString() == beerRating.BeerId.ToString());
                string json = JsonConvert.SerializeObject(beerRating, Formatting.Indented);
                var dataFile = Path.Combine("E:\\Test Projects\\TestPunkApi\\", "Database.json");
                //Reading the file 
                var jsonData = System.IO.File.ReadAllText(dataFile);
                var db = JsonConvert.DeserializeObject<DbJson>(jsonData);
                db.userRatings.Add(beerRating);
                jsonData = JsonConvert.SerializeObject(db);
                System.IO.File.WriteAllText(dataFile, jsonData);
                httpResponseMessage = new HttpResponseMessage
                {
                    Content = new ObjectContent<object>(new { message = "success", error = "0" }, new JsonMediaTypeFormatter()),
                    StatusCode = HttpStatusCode.OK
                };
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Failure");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="beerName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllBeers")]
        public HttpResponseMessage GetAllBeers(string beerName)
        {
            try
            {
                HttpResponseMessage httpResponseMessage;
                if (string.IsNullOrWhiteSpace(beerName))
                {
                    httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<object>(new { message = "Please enter valid name.", error = "1" }, new JsonMediaTypeFormatter()),
                        ReasonPhrase = "Invalid request",
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    return httpResponseMessage;
                }
                JArray beers = null;
                if (!string.IsNullOrWhiteSpace(_data))
                {
                    beers = JArray.Parse(_data);
                    if (beers.All(x => x.SelectToken("name").ToString() != beerName))
                    {

                        httpResponseMessage = new HttpResponseMessage()
                        {
                            Content = new ObjectContent<object>(new { message = "Please enter valid name with this no beer exists.", error = "1" }, new JsonMediaTypeFormatter()),
                            ReasonPhrase = "Invalid request",
                            StatusCode = HttpStatusCode.BadRequest
                        };
                        return httpResponseMessage;
                    }
                }
                JObject jObject = (JObject)beers.FirstOrDefault(x => x.SelectToken("name").ToString() == beerName);//.FirstOrDefault().Select(x => new { Id = x.SelectToken("id").ToString(), name = x.SelectToken("name").ToString(), description = x.SelectToken("description").ToString() });
                var beer = new Beer
                {
                    Id = (int)jObject.SelectToken("id"),
                    description = jObject.SelectToken("description").ToString(),
                    name = jObject.SelectToken("name").ToString()
                };
                var dataFile = Path.Combine("E:\\Test Projects\\TestPunkApi\\", "Database.json");                
                var jsonData = System.IO.File.ReadAllText(dataFile);
                var db = JsonConvert.DeserializeObject<DbJson>(jsonData);
                if(!db.searchbeers.Any(x => x.Id.ToString() == beer.Id.ToString()))
                {
                    db.searchbeers.Add(beer);
                    jsonData = JsonConvert.SerializeObject(db);
                    File.WriteAllText(dataFile, jsonData);
                }                
                var ratings = db.userRatings.Where(x => x.BeerId.ToString() == beer.Id.ToString()).ToList();
                httpResponseMessage = new HttpResponseMessage()
                {
                    Content = new ObjectContent<object>(new { message = new { beer.Id,beer.name,beer.description, ratings }, error = "0" }, new JsonMediaTypeFormatter()),
                    StatusCode = HttpStatusCode.OK
                };
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Failure");
            }
        }
    }
}
