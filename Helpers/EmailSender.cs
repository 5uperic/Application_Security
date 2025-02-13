using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public class EmailSender
    {
        private readonly string apiKey = "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";  // Replace with your SendGrid API key

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("andriclim@gmail.com", "AS_assignment");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            
            var response = await client.SendEmailAsync(msg);
            
            // Log response for debugging
            Console.WriteLine($"Status Code: {response.StatusCode}");
            if ((int)response.StatusCode >= 400)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"Error response: {responseBody}");
            }
        }
    }
}
