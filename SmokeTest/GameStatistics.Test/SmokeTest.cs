using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using GameStatistics.Test.Models;
using Newtonsoft.Json;
using System;
using System.Text;

namespace SmokeTests
{
    [TestFixture]
    public class SmokeTests
    {
        private HttpClient _client;
        private static string _baseUrl = "https://localhost:7170"; //Cambia esto por tu API URL
        private int _times;

        [SetUp]
        public void Setup() // Inicializar el cliente HTTP con la URL base de la API y las veces que se llamará
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
            _times = 100; // Puedes cambiar esto por la cantidad de veces que deseas llamar a la API (Afecta a todos los metodos)
        }

        [OneTimeSetUp]
        public async Task CheckApiAvailability() // Enviar una petición GET al endpoint /gamedata de la API para ver disponibilidad
        {
            var client = new HttpClient();
            var response = await client.GetAsync(_baseUrl + "/gamedata"); 
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose(); // Liberar los recursos del cliente HTTP
        }

        [NonParallelizable]
        [Order(1)]
        [Test]
        public async Task GetGameDataShouldReturnOkUnderMinimalLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // Crear una lista para almacenar las tareas
            string endpointToCall = "/gamedata";

            // ---- ACT ----
            // Enviar tantas peticiones como _times diga al endpoint /gamedata en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, _times), async (i, ct) =>
            {
                var response = _client.GetAsync(endpointToCall, ct);
                tasks.Add(response);
            });

            await Task.WhenAll(tasks.ToArray()); // Esperar a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList(); // Obtener las respuestas de las tareas

            // ---- ASSERT ----
            int totalAnswers = 0;
            Assert.Multiple(async () =>
            {
                // Verificar que todas las respuestas tengan el código de estado 200 (Ok)
                foreach (var response in responses)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"ERR: Respondió correctamente {totalAnswers} de {_times} Requests");
                    // Verificar que todas las respuestas tengan el contenido JSON válido
                    Assert.That(response.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                    // Verificamos que el contenido de la respuesta sea una lista de objetos 
                    // JSON válidos y tenga la estructura esperada usando FluentAssertions
                    var content = await response.Content.ReadAsStringAsync();
                    var parseContentToJson = JToken.Parse(content);
                    foreach (var item in parseContentToJson)
                    {
                        item.Should().HaveElement("id")
                        .And.HaveElement("gameName")
                        .And.HaveElement("category")
                        .And.HaveElement("totalBets")
                        .And.HaveElement("totalWins")
                        .And.HaveElement("averageBetAmount")
                        .And.HaveElement("popularityScore");
                    }
                    totalAnswers++;
                }
            });
        }

        [NonParallelizable]
        [Order(2)]
        [Test]
        public async Task GetValidGameDataByIDShouldReturnOkUnderMinimalLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // Creamos una lista para almacenar las tareas
            var arrayOfIDs = new String[] //Creamos una lista con algunos IDs
            {
                "000582cf-0b3d-4855-9f5e-54d3be5443cb", "000fc79c-38c1-4c37-bafe-3b5b15f405aa", "00358a92-4bd2-4cc2-a3bb-1ded6f5994ba"
            };
            Random random = new Random();
            string endpointToCall = $"/gamedata/ID";
            var rIndex = 0;

            // ---- ACT ----
            await Parallel.ForEachAsync(Enumerable.Range(0, _times), async (i, ct) => // Enviar tantas peticiones como _times diga al endpoint /gamedata en paralelo
            {
                //Enviamos un ID válido aleatorio
                rIndex = random.Next(0, arrayOfIDs.Length);
                endpointToCall = $"/gamedata/{arrayOfIDs[rIndex]}";
                var response = _client.GetAsync(endpointToCall, ct);
                tasks.Add(response);
            });

            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList();// Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            int totalAnswers = 0;
            Assert.Multiple(async () =>
            {
                foreach (var response in responses) // Verificar que todas las respuestas tengan el código de estado 200 (Ok)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.OK), $"ERR: Respondió correctamente {totalAnswers} de {_times} Requests");
                    // Verificar que todas las respuestas tengan el contenido JSON válido
                    Assert.That(response.Content.Headers.ContentType.MediaType, Is.EqualTo(expected: "application/json"));
                    // Verificamos que el contenido de la respuesta sea una lista de objetos 
                    // JSON válidos y tenga la estructura esperada usando FluentAssertions
                    var content = await response.Content.ReadAsStringAsync();
                    var parseContentToJson = JToken.Parse(content);
                    parseContentToJson.Should().HaveElement("id")
                        .And.HaveElement("gameName")
                        .And.HaveElement("category")
                        .And.HaveElement("totalBets")
                        .And.HaveElement("totalWins")
                        .And.HaveElement("averageBetAmount")
                        .And.HaveElement("popularityScore");
                    totalAnswers++;
                }
            });
        }

        [NonParallelizable]
        [Order(3)]
        [Test]
        public async Task PostValidGameDataShouldReturnCreatedUnderMinimalLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            Random random = new Random();
            string endpointToCall = "/gamedata";
            var gameDataList = new List<GameData>(); //Creamos una lista de objetos GameData
            gameDataList.Add(new GameData
            {
                GameName = "Peck a Bo",
                Category = "Dice Game",
                TotalBets = 6000,
            });
            gameDataList.Add(new GameData
            {
                GameName = "Gin Poker-Texas Hold-Em",
                Category = "Card Game",
                TotalBets = 5000,
            });
            gameDataList.Add(new GameData
            {
                GameName = "Chinchon",
                Category = "Card Game",
                TotalBets = 2000,
            });
            var rIndex = 0;
            string gameDataJson ;
            StringContent content;

            // ---- ACT ----
            await Parallel.ForEachAsync(Enumerable.Range(0, _times), async (i, ct) =>
            {
                rIndex = random.Next(0, gameDataList.Count);
                gameDataJson = JsonConvert.SerializeObject(gameDataList[rIndex], Formatting.Indented);
                content = new StringContent(gameDataJson, Encoding.UTF8, "application/json");
                var response = _client.PostAsync(endpointToCall, content, ct);
                tasks.Add(response);
            });
            await Task.WhenAll(tasks.ToArray());
            var responses = tasks.Select(t => t.Result).ToList();// Obtener las respuestas de las tareas

            // ---- ASSERT ----
            int totalAnswers = 0;
            Assert.Multiple(async () =>
            {
                foreach (var response in responses) // Verificar que todas las respuestas tengan el código de estado 201 (Created)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.Created), $"ERR: Respondió correctamente {totalAnswers} de {_times} Requests");
                    //responseContent = await response.Content.ReadAsStringAsync();
                    //gameData = JsonConvert.DeserializeObject<GameData>(responseContent);
                    var content = await response.Content.ReadAsStringAsync();
                    var parseContentToJson = JToken.Parse(content);
                    parseContentToJson.Should().HaveElement("id")
                        .And.HaveElement("gameName")
                        .And.HaveElement("category")
                        .And.HaveElement("totalBets")
                        .And.HaveElement("totalWins")
                        .And.HaveElement("averageBetAmount")
                        .And.HaveElement("popularityScore");
                    totalAnswers++;
                }
            });
        }

        [NonParallelizable]
        [Order(4)]
        [Test]
        public async Task PutValidGameDataShouldReturnNoContentUnderMinimalLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // Creamos una lista para almacenar las tareas
            var gameDataList = new List<GameData>();
            gameDataList.Add(new GameData
            {
                Id = "000582cf-0b3d-4855-9f5e-54d3be5443cb",
                GameName = "Volleyball Game",
                Category = "Recreational machines",
            });
            gameDataList.Add(new GameData
            {
                Id = "000fc79c-38c1-4c37-bafe-3b5b15f405aa",
                GameName = "Caribbean Stud Poker",
                Category = "Card Game",
            });
            var rIndex = 0;
            var rTotalBets = 0;
            Random random = new Random();
            string gameDataJson;
            GameData gameData;
            StringContent content;
            string endpointToCall = $"/gamedata/ID";

            // ---- ACT ----
            // Enviar tantas peticiones como _times diga al endpoint /gamedata en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, _times), async (i, ct) =>
            {
                //Enviamos un GameData Object aleatorio
                rIndex = random.Next(0, gameDataList.Count);
                rTotalBets = random.Next(10, 10000);
                gameData = gameDataList[rIndex];
                gameData.TotalBets = rTotalBets;
                gameDataJson = JsonConvert.SerializeObject(gameDataList[rIndex], Formatting.Indented);
                content = new StringContent(gameDataJson, Encoding.UTF8, "application/json");
                endpointToCall = $"/gamedata/{gameData.Id}";
                var response = _client.PutAsync(endpointToCall, content, ct);
                tasks.Add(response);
            });

            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList();// Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            int totalAnswers = 0;
            foreach (var response in responses) // Verificar que todas las respuestas tengan el código de estado 204 (NoContents)
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), $"ERR: Respondió correctamente {totalAnswers} de {_times} Requests");
                totalAnswers++;
            }
        }

        [NonParallelizable]
        [Test]
        public async Task DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderMinimalLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // Creamos una lista para almacenar las tareas
            var arrayOfIDs = new String[] //Creamos una lista con algunos IDs
            {
                "19e44f74-x-4284-90ea-e6f02be75613", "1cfffeb9-66a3-x-b1b3-208610be7a73", "27896d44-x-48bd-9b14-50ac09e5b0a4"
            };
            Random random = new Random();
            string endpointToCall = $"/gamedata/ID";
            var rIndex = 0;

            // ---- ACT ----
            // Enviar tantas peticiones como _times diga al endpoint /gamedata en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, _times), async (i, ct) =>
            {
                //Enviamos un ID válido aleatorio
                rIndex = random.Next(0, arrayOfIDs.Length);
                endpointToCall = $"/gamedata/{arrayOfIDs[rIndex]}";
                var response = _client.DeleteAsync(endpointToCall, ct);
                tasks.Add(response);
            });

            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList(); // Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            Assert.Multiple(async () => // Verificar que todas las respuestas tengan el código de estado 404 (NotFound)
            {
                int total_answer = 0;
                foreach (var response in responses)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.NotFound), $"ERR: Respondió correctamente {total_answer} de {_times} Requests");
                    // Verificamos que el contenido de la respuesta sea una lista de objetos 
                    // JSON válidos y tenga la estructura esperada usando FluentAssertions
                    total_answer += 1;
                }
            });
        }
    }
}
