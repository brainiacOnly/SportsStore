using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public partial class InternetEmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public InternetEmailOrderProcessor()
        {
            emailSettings = new EmailSettings();
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            ChangeCredentials(emailSettings);   //do not open!
            
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailSettings.MailFromAddress);
            mail.To.Add(new MailAddress(emailSettings.MailToAddress));
            mail.Subject = "SportsStore: new order";
            mail.Body = CombineMessage(cart, shippingDetails);
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(emailSettings.MailFromAddress, emailSettings.Password);
            client.Send(mail);
            client.Dispose();
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

        private void ChangeCredentials(EmailSettings emailSettings)
        {
            emailSettings.MailFromAddress = "xxx@gmail.com";
            emailSettings.Password = "yyy";
        }
    }
}
