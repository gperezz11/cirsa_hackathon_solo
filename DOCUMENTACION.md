# Documentación del Proyecto de Stress Testing con xUnit

## Background del Proyecto

El proyecto de Stress Testing tiene como objetivo evaluar el rendimiento y la estabilidad de la API de estadísticas de juegos.

## Uso de la Aplicación de Stress Testing

1. **Requisitos Previos:**
   - Microsoft Visual Studio 2022 o una versión compatible.
   - Paquetes NuGet instalados: Xunit, FluentAssertions, Moq.

2. **Configuración del Entorno:**
   - Clonar el repositorio del proyecto.
   - Abrir la solución en Visual Studio.
   - Instalar los paquetes mencionados anteriormente.

3. **Ejecución de las Pruebas:**
## Smoke Test:

### Resumen
Este código representa una clase de prueba llamada "SmokeTests" que contiene varios métodos de prueba para probar la funcionalidad de una API. La clase utiliza el marco de prueba NUnit y la clase HttpClient para enviar solicitudes HTTP a los puntos finales de la API y afirmar las respuestas esperadas.

### Ejemplo de uso
```csharp
var smokeTests = new SmokeTests();
smokeTests.Setup();
smokeTests.CheckApiAvailability();
smokeTests.GetGameDataShouldReturnOkUnderMinimalLoadAsync();
smokeTests.GetValidGameDataByIDShouldReturnOkUnderMinimalLoadAsync();
smokeTests.PostValidGameDataShouldReturnCreatedUnderMinimalLoadAsync();
smokeTests.PutValidGameDataShouldReturnNoContentUnderMinimalLoadAsync();
smokeTests.DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderMinimalLoadAsync();
smokeTests.TearDown();
```

### Análisis de código
#### Principales funcionalidades
- La clase `SmokeTests` es responsable de probar la funcionalidad de una API.
- Inicializa una instancia de la clase HttpClient con la URL base de la API y el número de veces que se llamará a la API.
- Envía solicitudes HTTP a los puntos finales de la API y afirma las respuestas esperadas utilizando el marco NUnit.
- Comprueba la disponibilidad de la API enviando una solicitud GET al endpoint `/gamedata`.
- Prueba el método `GetGameDataShouldReturnOkUnderMinimalLoadAsync`, que envía múltiples solicitudes GET al endpoint `/gamedata` en paralelo y afirma que las respuestas tienen un código de estado de 200 (OK) y contenido JSON válido.
- Prueba el método `GetValidGameDataByIDShouldReturnOkUnderMinimalLoadAsync`, que envía múltiples solicitudes GET al endpoint `/gamedata/{id}` con ID válidos aleatorios en paralelo y afirma que las respuestas tienen un código de estado de 200 (OK) y contenido JSON válido.
- Prueba el método `PostValidGameDataShouldReturnCreatedUnderMinimalLoadAsync`, que envía múltiples solicitudes POST al endpoint `/gamedata` con datos aleatorios válidos del juego en paralelo y afirma que las respuestas tienen un código de estado de 201 (Creado) y contenido JSON válido.
- Prueba el método `PutValidGameDataShouldReturnNoContentUnderMinimalLoadAsync`, que envía múltiples solicitudes PUT al endpoint `/gamedata/{id}` con ID válidos aleatorios y datos del juego actualizados en paralelo y afirma que las respuestas tienen un código de estado de 204 (NoContent).
- Prueba el método `DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderMinimalLoadAsync`, que envía múltiples solicitudes DELETE al endpoint `/gamedata/{id}` con ID aleatorias no válidas en paralelo y afirma que las respuestas tienen un código de estado de 404 (NotFound).
___
#### Métodos
- `Setup()`: Inicializa la instancia de HttpClient con la URL base de la API y el número de veces que se llamará a la API.
- `CheckApiAvailability()`: envía una solicitud GET al endpoint `/gamedata` para verificar la disponibilidad de la API.
- `TearDown()`: Dispone de la instancia HttpClient para liberar los recursos.
- `GetGameDataShouldReturnOkUnderMinimalLoadAsync()`: envía múltiples solicitudes GET al endpoint `/gamedata` en paralelo y afirma las respuestas esperadas.
- `GetValidGameDataByIDShouldReturnOkUnderMinimalLoadAsync()`: envía múltiples solicitudes GET al endpoint `/gamedata/{id}` con ID válidas aleatorias en paralelo y afirma las respuestas esperadas.
- `PostValidGameDataShouldReturnCreatedUnderMinimalLoadAsync()`: envía múltiples solicitudes POST al endpoint `/gamedata` con datos aleatorios válidos del juego en paralelo y afirma las respuestas esperadas.
- `PutValidGameDataShouldReturnNoContentUnderMinimalLoadAsync()`: envía múltiples solicitudes PUT al endpoint `/gamedata/{id}` con ID válidos aleatorios y datos del juego actualizados en paralelo y afirma las respuestas esperadas.
- `DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderMinimalLoadAsync()`: envía múltiples solicitudes DELETE al endpoint `/gamedata/{id}` con ID aleatorias no válidas en paralelo y afirma las respuestas esperadas.
___
#### Campos
- `_client`: una instancia de la clase HttpClient utilizada para enviar solicitudes HTTP a la API.
- `_baseUrl`: La URL base de la API.
- `_times`: La cantidad de veces que se llamará a la API.
___


## Average Load Test:
### Resumen
La clase "AverageLoadTest" es un dispositivo de prueba que contiene métodos de prueba para probar el rendimiento y el comportamiento de una API en condiciones de carga promedio. Utiliza la clase `HttpClient` para enviar solicitudes HTTP a los puntos finales de la API y afirma las respuestas esperadas.

### Ejemplo de uso
```csharp
// Crea una instancia de la clase AverageLoadTest
var averageLoadTest = new AverageLoadTest();

// Configura HttpClient con la URL base de la API y las solicitudes por segundo
averageLoadTest.Setup();

// Verifique la disponibilidad de la API enviando una solicitud GET al endpoint /gamedata
averageLoadTest.CheckApiAvailability();

// Probar el comportamiento de la API al obtener datos válidos del juego
averageLoadTest.GetValidGameDataShouldReturnOkUnderAverageLoadAsync();

// Probar el comportamiento de la API al obtener datos válidos del juego por ID
averageLoadTest.GetValidGameDataByIDShouldReturnOkUnderAverageLoadAsync();

// Probar el comportamiento de la API al poner datos válidos del juego
averageLoadTest.PutValidGameDataShouldReturnNoContentUnderPikeLoadAsync();

// Probar el comportamiento de la API al eliminar datos del juego no válidos por ID
averageLoadTest.DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderPikeLoadAsync();

// Probar el comportamiento de la API al publicar datos válidos del juego
averageLoadTest.PostValidGameData_ShouldReturnCreatedUnderPikeLoadAsync();
```

### Análisis de código
#### Principales funcionalidades
- Configurar HttpClient con la URL base de la API y las solicitudes por segundo
- Verifique la disponibilidad de la API enviando una solicitud GET al endpoint /gamedata
- Pruebe el comportamiento de la API al obtener datos válidos del juego.
- Pruebe el comportamiento de la API al obtener datos válidos del juego por ID
- Probar el comportamiento de la API al poner datos válidos del juego.
- Pruebe el comportamiento de la API al eliminar datos del juego no válidos por ID
- Pruebe el comportamiento de la API al publicar datos válidos del juego.
___
#### Métodos
- `Setup()`: Inicializa el HttpClient con la URL base de la API y las solicitudes por segundo.
- `CheckApiAvailability()`: envía una solicitud GET al endpoint /gamedata para verificar la disponibilidad de la API.
- `GetValidGameDataShouldReturnOkUnderAverageLoadAsync()`: prueba el comportamiento de la API al obtener datos válidos del juego.
- `GetValidGameDataByIDShouldReturnOkUnderAverageLoadAsync()`: prueba el comportamiento de la API al obtener datos válidos del juego por ID.
- `PutValidGameDataShouldReturnNoContentUnderPikeLoadAsync()`: prueba el comportamiento de la API al colocar datos válidos del juego.
- `DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderPikeLoadAsync()`: prueba el comportamiento de la API al eliminar datos de juego no válidos por ID.
- `PostValidGameData_ShouldReturnCreatedUnderPikeLoadAsync()`: prueba el comportamiento de la API al publicar datos válidos del juego.
___
#### Campos
- `_client`: una instancia de la clase HttpClient utilizada para enviar solicitudes HTTP a la API.
- `_baseUrl`: La URL base de la API.
- `_requestsPerSecond`: un array de números enteros que representan el número de solicitudes que se enviarán por segundo.
___


## SpikeTest:

### Resumen
La clase `SpikeTest` es un dispositivo de prueba que contiene varios métodos de prueba para probar el comportamiento de una API. Utiliza la clase `HttpClient` para enviar solicitudes HTTP a los puntos finales de la API y afirma las respuestas esperadas.

### Ejemplo de uso
```csharp
// Crea una instancia de la clase `SpikeTest`
var spikeTest = new SpikeTest();

// Ejecutar los métodos de prueba
spikeTest.GetValidGameDataShouldReturnOkUnderPikeLoadAsync();
spikeTest.GetValidGameDataByIDShouldReturnOkUnderPikeLoadAsync();
spikeTest.PutValidGameDataShouldReturnNoContentUnderPikeLoadAsync();
spikeTest.DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderPikeLoadAsync();
spikeTest.PostValidGameData_ShouldReturnCreatedUnderPikeLoadAsync();
```

### Análisis de código
#### Principales funcionalidades
Las principales funcionalidades de la clase `SpikeTest` son:
- Inicializando el `HttpClient` con la URL base de la API en el método `Setup`.
- Verificar la disponibilidad de la API enviando una solicitud GET al endpoint `/gamedata` en el método `CheckApiAvailability`.
- Enviar múltiples solicitudes paralelas a diferentes puntos finales de API y afirmar las respuestas esperadas en los métodos de prueba.
___
#### Métodos
- `Setup`: Inicializa el `HttpClient` con la URL base de la API.
- `CheckApiAvailability`: envía una solicitud GET al endpoint `/gamedata` para verificar la disponibilidad de la API.
- `TearDown`: Dispone del `HttpClient` para liberar los recursos.
- `GetValidGameDataShouldReturnOkUnderPikeLoadAsync`: envía múltiples solicitudes GET paralelas al endpoint `/gamedata` y afirma que todas las respuestas tienen un código de estado de 200 (OK).
- `GetValidGameDataByIDShouldReturnOkUnderPikeLoadAsync`: envía múltiples solicitudes GET paralelas al endpoint `/gamedata/{ID}` con diferentes ID y afirma que todas las respuestas tienen un código de estado de 200 (OK).
- `PutValidGameDataShouldReturnNoContentUnderPikeLoadAsync`: envía múltiples solicitudes PUT paralelas al endpoint `/gamedata/{ID}` con diferentes ID y afirma que todas las respuestas tienen un código de estado de 204 (Sin contenido).
- `DeleteUnvalidGameDataByID_ShouldReturnNotFoundUnderPikeLoadAsync`: envía múltiples solicitudes DELETE paralelas al endpoint `/gamedata/{ID}` con diferentes ID y afirma que todas las respuestas tienen un código de estado de 404 (NotFound).
- `PostValidGameData_ShouldReturnCreatedUnderPikeLoadAsync`: envía múltiples solicitudes POST paralelas al endpoint `/gamedata` con diferentes datos del juego y afirma que todas las respuestas tienen un código de estado de 201 (Creado).
___
#### Campos
- `_client`: una instancia de la clase `HttpClient` utilizada para enviar solicitudes HTTP a la API.
- `_baseUrl`: La URL base de la API.
___

## Instalación del Proyecto

Para instalar y ejecutar este proyecto, debes seguir los siguientes pasos:

1. Clona o descarga el repositorio de GitHub en tu computadora. Puedes usar el botón verde "Code" que aparece en la parte superior derecha de la página del repositorio, o puedes usar el siguiente comando en una terminal:

```bash
git clone https://github.com/gperezz11/cirsa_hackathon_solo.git
```

2. Instalar los siguientes paquetes en cada solución en su administrador de paquetes NuGGet:
- Average Load
```Average Load
FluentAssertions.Json
NUnit
```
- Smoke Test
```Smoke Test
coverlet.collector
FluentAssertions.Json
FluentAssertions
NUnit
```
- Spike Test
```Spike Test
FluentAssertions.Json
NUnit
```

3. Seguir el paso a paso de uso de cada solución que se encuentra en la parte superior, explicando cada test y programa en detalle

## Autores

 - [gperezz11](https://github.com/gperezz11)
