using GameStatistics.Test.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace SpykeTests
{
    [TestFixture]
    public class SpikeTest
    {
        private HttpClient _client;
        private static string _baseUrl = "https://localhost:7170"; //Cambia esto por tu API URL

        [SetUp]
        public void Setup() // Inicializar el cliente HTTP con la URL base de la API y las veces que se llamará
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl); 
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
        public async Task GetValidGameDataShouldReturnOkUnderPikeLoadAsync()
        {
            // ---- ARRANGE ---
            int timesToCall = 750;
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            string endpointToCall = "/gamedata";

            // ---- ACT ----
            // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
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
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"ERR: Respondió correctamente {totalAnswers} de {timesToCall} Requests");
                    Assert.That(response.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                    totalAnswers++;
                }
            });

        }


        [NonParallelizable]
        [Order(2)]
        [Test]
        public async Task GetValidGameDataByIDShouldReturnOkUnderPikeLoadAsync()
        {
            // ---- ARRANGE ---
            int timesToCall = 600;
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            var arrayOfIDs = new String[]
            {
                "febe6431-b71e-4571-b7e4-8fdab85fc98e"
            };
            Random random = new Random();
            string endpointToCall = $"/gamedata/ID";
            var rIndex = 0;

            // ---- ACT ----
            // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata/ID en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
            {
                endpointToCall = $"/gamedata/{arrayOfIDs[0]}";
                var response = _client.GetAsync(endpointToCall, ct);
                tasks.Add(response);
            });

            // Esperamos a que todas las peticiones se completen
            await Task.WhenAll(tasks.ToArray());
            // Obtenemos las respuestas de las tareas
            var responses = tasks.Select(t => t.Result).ToList();

            // ---- ASSERT ----
            int totalAnswers = 0;
            Assert.Multiple(async () =>
            {
                // Verificar que todas las respuestas tengan el código de estado 200 (Ok)
                foreach (var response in responses)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.OK), $"ERR: Respondió correctamente {totalAnswers} de {timesToCall} Requests");
                    Assert.That(response.Content.Headers.ContentType.MediaType, Is.EqualTo(expected: "application/json"));
                    totalAnswers++;
                }
            });

        }

        [NonParallelizable]
        [Order(3)]
        [Test]
        public async Task PutValidGameDataShouldReturnNoContentUnderPikeLoadAsync()
        {
            // ---- ARRANGE ---
            int timesToCall = 350;
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            var gameDataList = new List<GameData>()
            {
                new GameData
            {
                Id = "febe6431-b71e-4571-b7e4-8fdab85fc98e",
                GameName = "Volleyball Game",
                Category = "Recreational machines",
                TotalBets = 0,
            }
            };
            var rTotalBets = 0;
            Random random = new Random();
            string gameDataJson;
            GameData gameData;
            StringContent content;
            string endpointToCall = $"/gamedata/";

            // ---- ACT ----
            // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
            {
                //Enviamos un GameData Object aleatorio
                rTotalBets = random.Next(10, 10000);
                gameData = gameDataList[0];
                gameData.TotalBets = rTotalBets;
                gameDataJson = JsonConvert.SerializeObject(gameData, Formatting.Indented);
                content = new StringContent(gameDataJson, Encoding.UTF8, "application/json");
                var response = _client.PutAsync($"{endpointToCall}{gameData.Id}", content, ct);
                tasks.Add(response);
            });

            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList(); // Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            // Verificar que todas las respuestas tengan el código de estado 204 (NoContents)
            int totalAnswers = 0;
            foreach (var response in responses)
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), $"ERR: Respondió correctamente {totalAnswers} de {timesToCall} Requests");
                totalAnswers++;
            }

        }


        [NonParallelizable]
        [Order(4)]
        [Test]
        public async Task DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderPikeLoadAsync()
        {
            // ---- ARRANGE ---
            int timesToCall = 4000;
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // Creamos una lista para almacenar las tareas
            var arrayOfIDs = new String[] //Creamos una lista con algunos IDs
            {
                "19e44f74-x-4284-90ea-e6f02be75613", "1cfffeb9-66a3-x-b1b3-208610be7a73", "27896d44-x-48bd-9b14-50ac09e5b0a4"
            };
            Random random = new Random();
            string endpointToCall = $"/gamedata/ID";
            var rIndex = 0;

            // ---- ACT ----
            // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata en paralelo
            await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
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
            Assert.Multiple(async () =>
            {
                // Verificar que todas las respuestas tengan el código de estado 404 (NotFound)
                int total_answer = 0;
                foreach (var response in responses)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.NotFound), $"ERR: Respondió correctamente {total_answer} de {timesToCall} Requests");
                    total_answer += 1;
                }
            });

        }

        [NonParallelizable]
        [Order(5)]
        [Test]
        public async Task PostValidGameData_ShouldReturnCreatedUnderPikeLoadAsync()
        {
            // ---- ARRANGE ---
            int timesToCall = 300;
            var client = new HttpClient();
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            Random random = new Random();
            string endpointToCall = "/gamedata";
            var gameDataList = new List<GameData>() //Creamos una lista de objetos GameData
            {
                new GameData
            {
                GameName = "Peck a Bo",
                Category = "Dice Game",
                TotalBets = 6000,
            },new GameData
            {
                GameName = "Gin Poker-Texas Hold-Em",
                Category = "Card Game",
                TotalBets = 5000,
            },new GameData
            {
                GameName = "Chinchon",
                Category = "Card Game",
                TotalBets = 2000,
            }
            };
            var rIndex = 0;
            string gameDataJson;
            StringContent content;

            // ---- ACT ----
            await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
            {
                rIndex = random.Next(0, gameDataList.Count);
                gameDataJson = JsonConvert.SerializeObject(gameDataList[rIndex], Formatting.Indented);
                content = new StringContent(gameDataJson, Encoding.UTF8, "application/json");
                var response = _client.PostAsync(endpointToCall, content, ct);
                tasks.Add(response);
            });
            await Task.WhenAll(tasks.ToArray()); // Obtener las respuestas de las tareas
            var responses = tasks.Select(t => t.Result).ToList();

            // ---- ASSERT ----
            int totalAnswers = 0;

            foreach (var response in responses) // Verificar que todas las respuestas tengan el código de estado 201 (Created)
            {
                Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.Created), $"ERR: Respondió correctamente {totalAnswers} de {timesToCall} Requests");
                totalAnswers++;
            }

        }

    }
}
