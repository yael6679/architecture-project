using MailKit.Net.Smtp;
using MimeKit;

public class EmailService : IEmailService
{
    public async Task SendWinnerEmailAsync(string targetEmail, string userName, string giftName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("המכירה הסינית", "your-email@gmail.com"));
        message.To.Add(new MailboxAddress(userName, targetEmail));
        message.Subject = "מזל טוב! זכית בפרס במכירה הסינית 🎁";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <div style='direction: rtl; font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>שלום {userName},</h2>
                    <p style='font-size: 1.1rem;'>אנחנו נרגשים לבשר לך שעלית בגורל כזוכה הגדול במתנה:</p>
                    <div style='background: #f1f8e9; padding: 15px; border-radius: 5px; font-weight: bold; font-size: 1.5rem; color: #2e7d32; text-align: center;'>
                        {giftName}
                    </div>
                    <p>ניצור איתך קשר בהקדם לתיאום קבלת הפרס.</p>
                    <hr>
                    <p style='font-size: 0.8rem; color: #777;'>נשלח באופן אוטומטי על ידי מערכת המכירה הסינית.</p>
                </div>"
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("shira13098@gmail.com", "siynvuvpwfgeqrdo");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
