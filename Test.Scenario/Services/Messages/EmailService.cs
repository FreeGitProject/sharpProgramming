namespace Test.Scenario.Services.Messages;

public class EmailService : IMessageService
{
    public void Send(string msg) => Console.WriteLine("Email sent: " + msg);
}

