using System;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Net;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "example@example.com";
        public string MailFromAddress = "sportsstore@gmail.com";
        public bool UseSsl = true;
        public bool useDefaultCredantials = false;
        public SmtpDeliveryMethod DeliveryMethod = SmtpDeliveryMethod.Network;
        public string Username {
            get { return MailFromAddress.Split('@')[0]; }
        }
        public string Password = "pass";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"C:\sports_store_emails";
        public int Timeout = 20000;
    }

    public partial class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }
        
        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            emailSettings.MailToAddress = shippingDetails.Email;

            using (var smtpClient = new SmtpClient())
            {
                emailSettings.MailToAddress = shippingDetails.Email;
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.Timeout = emailSettings.Timeout;
                smtpClient.UseDefaultCredentials = emailSettings.useDefaultCredantials;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                smtpClient.DeliveryMethod = emailSettings.DeliveryMethod;

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(emailSettings.MailFromAddress);
                mailMessage.To.Add(new MailAddress(emailSettings.MailToAddress));
                mailMessage.Subject = "SportsStore: new order";
                mailMessage.Body = CombineMessage(cart, shippingDetails);

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
                
        }

        private string CombineMessage(Cart cart, ShippingDetails shippingDetails)
        {
            StringBuilder body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

            foreach (var line in cart.Lines)
            {
                var subtotal = line.Product.Price * line.Quantity;
                body.AppendFormat("{0} x {1} (subtotal: {2:c})", line.Quantity, line.Product.Name, subtotal);
            }

            body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                .AppendLine("---")
                .AppendLine("Ship to:")
                .AppendLine(shippingDetails.Name)
                .AppendLine(shippingDetails.Line1)
                .AppendLine(shippingDetails.Line2 ?? "")
                .AppendLine(shippingDetails.Line3 ?? "")
                .AppendLine(shippingDetails.City)
                .AppendLine(shippingDetails.State ?? "")
                .AppendLine(shippingDetails.Country)
                .AppendLine(shippingDetails.Zip)
                .AppendLine("---")
                .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

            return body.ToString();
        }
    }
}
