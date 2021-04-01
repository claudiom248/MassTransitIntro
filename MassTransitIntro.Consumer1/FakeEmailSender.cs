using System;
using System.Threading.Tasks;

namespace MassTransitIntro.Consumer1
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendAsync(Email email)
        {
            Console.WriteLine("#########################################################################################");
            Console.WriteLine($"Sent email from {email.From} to {string.Join(',', email.To)}.");
            Console.WriteLine($"Subject {email.Subject}");
            Console.WriteLine($"Content -> {email.Body}");
            Console.WriteLine("#########################################################################################");

            return Task.CompletedTask;
        }
    }
}
