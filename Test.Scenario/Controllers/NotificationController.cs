using Microsoft.AspNetCore.Mvc;
using Test.Scenario.Services.Messages;

namespace Test.Scenario.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController : ControllerBase
{
    private readonly IMessageService _messageService;
    public NotificationController(IMessageService messageService) // injected here
    {
        _messageService = messageService;
    }
    [HttpGet]
    public void Notify() {
        string message = "hell";
        _messageService.Send(message);
    } 
}
