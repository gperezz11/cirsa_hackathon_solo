using AverageLoadTest.Models;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace AverageLoadTest
{
    [TestFixture]
    public class AverageLoadTest
    {
        private HttpClient _client;
        private static string _baseUrl = "https://localhost:7170"; //Cambia esto por tu API URL
        private int[] _requestsPerSecond; // Lista de n�meros de peticiones que se enviar�n en cada segundo
        [SetUp]
        public void Setup() // Inicializar el cliente HTTP con la URL base de la API y las veces que se llamar�
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
            _requestsPerSecond = new int[] { 20, 40, 80, 15 };
        }

        [OneTimeSetUp]
        public async Task CheckApiAvailability() // Enviar una petici�n GET al endpoint /gamedata de la API para ver disponibilidad
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
        public async Task GetValidGameDataShouldReturnOkUnderAverageLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // La lista para almacenar las tareas
            string endpointToCall = "/gamedata"; // El endpoint de la API que se quiere probar
            var stopwatch = new Stopwatch(); // El reloj para medir el tiempo transcurrido

            // ---- ACT ----
            int timesToCall;
            stopwatch.Start();
            for (int i = 0; i < _requestsPerSecond.Length; i++)
            {
                timesToCall = _requestsPerSecond[i];
                await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
                {
                    var response = _client.GetAsync(endpointToCall, ct);
                    tasks.Add(response);
                });

                var elapsed = stopwatch.ElapsedMilliseconds;
                var iterationElapsed = elapsed % 1000;

                if (iterationElapsed < 1000 )  // Comprobar si ha pasado un segundo
                {
                    await Task.Delay(Math.Max(0, (int)(1000 - iterationElapsed))); // Esperar la diferencia de tiempo hasta el siguiente segundo
                }
            }
            stopwatch.Stop();
            
            await Task.WhenAll(tasks.ToArray()); // Esperar a que todas las peticiones se completen de forma as�ncrona
            var responses = tasks.Select(t => t.Result).ToList(); // Obtener las respuestas de las tareas

            // ---- ASSERT ----
            int totalAnswers = 0;
            Assert.Multiple(() => {
                foreach (var response in responses) // Verificar que todas las respuestas tengan el c�digo de estado 200 (Ok)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), $"ERR: Respondi� correctamente {totalAnswers} Requests");
                    Assert.That(response.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                    totalAnswers++;
                }
            });

        }

        [NonParallelizable]
        [Order(2)]
        [Test]
        public async Task GetValidGameDataByIDShouldReturnOkUnderAverageLoadAsync()
        {
            // ---- ARRANGE ---
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            var arrayOfIDs = new String[]
            {
                "012b368e-0c07-4b91-900b-6da750cb6be5"
            };
            string endpointToCall = $"/gamedata/";
            var stopwatch = new Stopwatch(); // El reloj para medir el tiempo transcurrido
            int timesToCall;

            // ---- ACT ----
            // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata/ID en paralelo
            for (int i = 0; i < _requestsPerSecond.Length; i++)
            {
                timesToCall = _requestsPerSecond[i];
                await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
                {
                    var response = _client.GetAsync(endpointToCall + arrayOfIDs[0], ct);
                    tasks.Add(response);
                });
                var elapsed = stopwatch.ElapsedMilliseconds;
                var iterationElapsed = elapsed % 1000;

                if (iterationElapsed < 1000)  // Comprobar si ha pasado un segundo
                {
                    await Task.Delay(Math.Max(0, (int)(1000 - iterationElapsed))); // Esperar la diferencia de tiempo hasta el siguiente segundo
                }
            }
            stopwatch.Stop();
            
            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList(); // Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            int totalAnswers = 0;
            Assert.Multiple(async () =>
            {
                // Verificar que todas las respuestas tengan el c�digo de estado 200 (Ok)
                foreach (var response in responses)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.OK), $"ERR: Respondi� correctamente {totalAnswers} Requests");
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
            int timesToCall;
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>();
            var gameDataList = new List<GameData>()
            {
                new GameData
                    {
                        Id = "012b368e-0c07-4b91-900b-6da750cb6be5",
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
            var stopwatch = new Stopwatch(); // El reloj para medir el tiempo transcurrido
            string endpointToCall = $"/gamedata/";

            // ---- ACT ----
            stopwatch.Start();
            for (int i = 0; i < _requestsPerSecond.Length; i++)
            {
                timesToCall = _requestsPerSecond[i];
                // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata en paralelo
                await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
                {
                    //Enviamos un TotalBets aleatorio
                    rTotalBets = random.Next(10, 10000);
                    gameData = gameDataList[0];
                    gameData.TotalBets = rTotalBets;
                    gameDataJson = JsonConvert.SerializeObject(gameData, Formatting.Indented);
                    content = new StringContent(gameDataJson, Encoding.UTF8, "application/json");
                    var response = _client.PutAsync($"{endpointToCall}{gameData.Id}", content, ct);
                    tasks.Add(response);
                });

                var elapsed = stopwatch.ElapsedMilliseconds;
                var iterationElapsed = elapsed % 1000;

                if (iterationElapsed < 1000)  // Comprobar si ha pasado un segundo
                {
                    await Task.Delay(Math.Max(0, (int)(1000 - iterationElapsed))); // Esperar la diferencia de tiempo hasta el siguiente segundo
                }
            }
            stopwatch.Stop();

            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList(); // Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            // Verificar que todas las respuestas tengan el c�digo de estado 204 (NoContents)
            int totalAnswers = 0;
            foreach (var response in responses)
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent), $"ERR: Respondi� correctamente {totalAnswers} Requests");
                totalAnswers++;
            }

        }

        [NonParallelizable]
        [Order(4)]
        [Test]
        public async Task DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderPikeLoadAsync()
        {
            // ---- ARRANGE ---
            int timesToCall;
            var tasks = new ConcurrentBag<Task<HttpResponseMessage>>(); // Creamos una lista para almacenar las tareas
            var arrayOfIDs = new String[] //Creamos una lista con algunos IDs
            {
                "19e44f74-x-4284-90ea-e6f02be75613", "1cfffeb9-66a3-x-b1b3-208610be7a73", "27896d44-x-48bd-9b14-50ac09e5b0a4"
            };
            Random random = new Random();
            string endpointToCall = $"/gamedata/";
            var rIndex = 0;
            var stopwatch = new Stopwatch(); // El reloj para medir el tiempo transcurrido

            // ---- ACT ----
            stopwatch.Start();
            for (int i = 0; i < _requestsPerSecond.Length; i++)
            {
                // Enviar tantas peticiones como timesToCall diga al endpoint /gamedata en paralelo
                timesToCall = _requestsPerSecond[i]; 
                await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
                {
                    //Enviamos un ID v�lido aleatorio
                    rIndex = random.Next(0, arrayOfIDs.Length);
                    var response = _client.DeleteAsync(endpointToCall + arrayOfIDs[rIndex], ct);
                    tasks.Add(response);
                });
                var elapsed = stopwatch.ElapsedMilliseconds;
                var iterationElapsed = elapsed % 1000;

                if (iterationElapsed < 1000)  // Comprobar si ha pasado un segundo
                {
                    await Task.Delay(Math.Max(0, (int)(1000 - iterationElapsed))); // Esperar la diferencia de tiempo hasta el siguiente segundo
                }
            }
            stopwatch.Stop();

            await Task.WhenAll(tasks.ToArray()); // Esperamos a que todas las peticiones se completen
            var responses = tasks.Select(t => t.Result).ToList(); // Obtenemos las respuestas de las tareas

            // ---- ASSERT ----
            Assert.Multiple(async () =>
            {
                // Verificar que todas las respuestas tengan el c�digo de estado 404 (NotFound)
                int total_answer = 0;
                foreach (var response in responses)
                {
                    Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.NotFound), $"ERR: Respondi� correctamente {total_answer} Requests");
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
            int timesToCall;
            var stopwatch = new Stopwatch(); // El reloj para medir el tiempo transcurrido
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
            stopwatch.Start();
            for (int i = 0; i < _requestsPerSecond.Length; i++)
            {
                timesToCall = _requestsPerSecond[i];
                await Parallel.ForEachAsync(Enumerable.Range(0, timesToCall), async (i, ct) =>
                {
                    rIndex = random.Next(0, gameDataList.Count);
                    gameDataJson = JsonConvert.SerializeObject(gameDataList[rIndex], Formatting.Indented);
                    content = new StringContent(gameDataJson, Encoding.UTF8, "application/json");
                    var response = _client.PostAsync(endpointToCall, content, ct);
                    tasks.Add(response);
                });

                var elapsed = stopwatch.ElapsedMilliseconds;
                var iterationElapsed = elapsed % 1000;

                if (iterationElapsed < 1000)  // Comprobar si ha pasado un segundo
                {
                    await Task.Delay(Math.Max(0, (int)(1000 - iterationElapsed))); // Esperar la diferencia de tiempo hasta el siguiente segundo
                }
            }
            stopwatch.Stop();

            await Task.WhenAll(tasks.ToArray()); // Obtener las respuestas de las tareas
            var responses = tasks.Select(t => t.Result).ToList();

            // ---- ASSERT ----
            int totalAnswers = 0;

            foreach (var response in responses) // Verificar que todas las respuestas tengan el c�digo de estado 201 (Created)
            {
                Assert.That(response.StatusCode, Is.EqualTo(expected: HttpStatusCode.Created), $"ERR: Respondi� correctamente {totalAnswers} Requests");
                totalAnswers++;
            }

        }

    }
}