using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Net;

namespace RDL
{
    [TestFixture]
    public class Tests
    {
        private const string URL_RDL = "https://petstore.swagger.io/v2";
        private string NewPet;
        private string NewPetId;

        private string NewPetData = @"{ ""id"": 0, ""category"": { ""id"": 0, ""name"": ""string"" }, ""name"": ""doggie"", ""photoUrls"": [ ""string"" ], ""tags"": [ { ""id"": 0, ""name"": ""string"" } ], ""status"": ""available""}";
        
        [Test, Order(1)]
        public void AddNewPet ()
        {
            string PetData = NewPetData;
            RestClient client = new RestClient(URL_RDL);
            RestRequest request =
                new RestRequest("pet", Method.POST);

            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", PetData, ParameterType.RequestBody);

            IRestResponse response =
                client.Execute(request);
            
            HttpStatusCode statusCode = response.StatusCode;
            int StatusCode = (int)statusCode;

            PetData pd = new JsonDeserializer().
                Deserialize<PetData>(response);

            NewPet = response.Content;
            NewPetId = pd.ID;
            
            Assert.That(StatusCode, Is.EqualTo(200));
        }

        [Test, Order(2)]
        public void GetPet()
        {
            string Pet = NewPet;

            RestClient client = new RestClient(URL_RDL);
            RestRequest request =
                new RestRequest("pet/"+NewPetId, Method.GET);

            IRestResponse response =
                client.Execute(request);
            
            HttpStatusCode statusCode = response.StatusCode;
            int StatusCode = (int)statusCode;
            Assert.That(StatusCode, Is.EqualTo(200));
            Assert.That(response.Content, Is.EqualTo(Pet));
            
            
            //PetData pd = new JsonDeserializer().
            //    Deserialize<PetData>(response);
            //Assert.That(pd.ID, Is.EqualTo("123457626"));
            //Assert.That(pd.Name, Is.EqualTo("Cat 3"));
            //Assert.That(pd.PhotoUrls[0], Is.EqualTo("url1"));
            //Assert.That(pd.PhotoUrls[1], Is.EqualTo("url2"));
            //Assert.That(pd.Tags[0].ID, Is.EqualTo("1"));
            //Assert.That(pd.Tags[0].Name, Is.EqualTo("tag3"));
            //Assert.That(pd.Tags[1].ID, Is.EqualTo("2"));
            //Assert.That(pd.Tags[1].Name, Is.EqualTo("tag4"));
            //Assert.That(pd.Status, Is.EqualTo("pending"));
        }

        [Test, Order(3)]
        public void InvalidPetId()
        {
            RestClient client = new RestClient(URL_RDL);
            RestRequest request =
                new RestRequest("pet/[|]`~!@#$%^&*(", Method.GET);
            IRestResponse response =
                client.Execute(request);

            HttpStatusCode statusCode = response.StatusCode;
            int StatusCode = (int)statusCode;

            Assert.That(StatusCode, Is.EqualTo(400));
        }

        [Test, Order(4)]
        public void PetNotFound()
        {
            RestClient client = new RestClient(URL_RDL);
            RestRequest request =
                new RestRequest("pet/123414", Method.GET);
            
            IRestResponse response =
                client.Execute(request);

            HttpStatusCode statusCode = response.StatusCode;
            int StatusCode = (int)statusCode;

            Assert.That(StatusCode, Is.EqualTo(404));

        }

        [Test, Order(5)]
        public void DeletPet()
        {
            string CheckDelPet = @"{""code"":200,""type"":""unknown"",""message"":""" + NewPetId + "\"}";
            RestClient client = new RestClient(URL_RDL);
            RestRequest request =
                    new RestRequest("pet/"+NewPetId, Method.DELETE);
            IRestResponse response =
            client.Execute(request);

            //DelPetData dpd = new JsonDeserializer().
            //    Deserialize<DelPetData>(response);

            HttpStatusCode statusCode = response.StatusCode;
            int StatusCode = (int)statusCode;

            Assert.That(StatusCode, Is.EqualTo(200));
            Assert.That(response.Content, Is.EqualTo(CheckDelPet));
            
        }

        public void DelPetsID()
        {
            RestClient client = new RestClient(URL_RDL);
            for (long i = 0; i<=999; i++) 
            {
                RestRequest request =
                    new RestRequest("pet/"+i, Method.GET);

                IRestResponse response =
                    client.Execute(request);
                PetData pd = new JsonDeserializer().
                    Deserialize<PetData>(response);
                HttpStatusCode statusCode = response.StatusCode;
                int StatusCode = (int)statusCode;

                if (StatusCode == 200)
                {
                    RestRequest del = new RestRequest("pet/"+pd.ID, Method
                        .DELETE);
                    IRestResponse respDel =
                    client.Execute(del);
                    Console.WriteLine("ID: " + pd.ID);
                    Console.WriteLine(respDel.Content);

                }
                else
                {
                    Console.WriteLine("ID: "+ i + StatusCode);
                    Console.WriteLine("Respose: " + response.Content);
                }
                  
            }

        }


        public class PetData
        {
            [JsonProperty("id")]
            public string ID { get; set; }                  
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("photoUrls")]
            public List<String> PhotoUrls { get; set; }
            [JsonProperty("tags")]
            public List<Tag> Tags { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }

            public class Tag
            {
                [JsonProperty("id")]
                public string ID { get; set; }
                [JsonProperty("name")]
                public string Name { get; set; }
            }

        }

        public class DelPetData
        {
            [JsonProperty("code")]
            public int Code { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}

