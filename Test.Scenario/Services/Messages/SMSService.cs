namespace Test.Scenario.Services.Messages;

public class SMSService : IMessageService
{
    public void Send(string msg) => Console.WriteLine("SMS sent: " + msg);
}

