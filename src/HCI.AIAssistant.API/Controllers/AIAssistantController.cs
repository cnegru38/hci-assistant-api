using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Text;

using HCI.AIAssistant.API.Models.DTOs;
using HCI.AIAssistant.API.Models.DTOs.AIAssistantController;
using HCI.AIAssistant.API.Services;

namespace HCI.AIAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAssistantController : ControllerBase
{
    private readonly ISecretsService _secretsService;
    private readonly IAppConfigurationsService _appConfigurationsService;
    private readonly IParametricFunctions _parametricFunctions;

    public AIAssistantController(
        ISecretsService secretsService,
        IAppConfigurationsService appConfigurationsService,
        IParametricFunctions parametricFunctions
    )
    {
        _secretsService = secretsService;
        _appConfigurationsService = appConfigurationsService;
        _parametricFunctions = parametricFunctions;
    }

    /// <summary>
    /// Receives gas telemetry and returns Safe / Danger.
    /// Also sends the result back to the ESP32 via IoT Hub.
    /// </summary>
    [HttpPost("message")]
    [ProducesResponseType(typeof(AIAssistantControllerPostMessageResponseDTO), 200)]
    [ProducesResponseType(typeof(ErrorResponseDTO), 400)]
    public async Task<ActionResult> PostMessage(
        [FromBody] AIAssistantControllerPostMessageRequestDTO request)
    {
        // 🔹 Validate request
        if (!_parametricFunctions.ObjectExistsAndHasNoNullPublicProperties(request))
        {
            return BadRequest(new ErrorResponseDTO
            {
                TextErrorTitle = "AtLeastOneNullParameter",
                TextErrorMessage = "Some parameters are null or missing.",
                TextErrorTrace = _parametricFunctions.GetCallerTrace()
            });
        }

        // 🔹 Extract gas value (THIS comes from Arduino)
        double gas = request.Gas;

        // 🔹 Apply deterministic safety logic
        string textMessage =
            gas <= 400 ? "Safe" : "Danger";

        // 🔹 Build response
        var response = new AIAssistantControllerPostMessageResponseDTO
        {
            TextMessage = textMessage
        };

        // 🔹 Send result back to ESP32 (Cloud-to-Device)
        string? ioTHubConnectionString =
            _secretsService?.IOTHubSecrets?.ConnectionString;

        if (!string.IsNullOrWhiteSpace(ioTHubConnectionString))
        {
            var serviceClient =
                ServiceClient.CreateFromConnectionString(ioTHubConnectionString);

            var serializedMessage =
                JsonConvert.SerializeObject(response);

            var ioTMessage =
                new Message(Encoding.UTF8.GetBytes(serializedMessage));

            await serviceClient.SendAsync(
                _appConfigurationsService.IoTDeviceName,
                ioTMessage
            );
        }

        return Ok(response);
    }
}
