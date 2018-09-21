using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;

namespace AttributeValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var toto = new Toto();
                toto.Hello("toto");
                toto.Hi("toto@gmail.com");                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error found : parameter {ex.Message} : {ex.InnerException.Message}");
            }
        }

        public class Toto
        {
            [OperationContract]
            public void Hello([EmailAddress] string email)
            {
                Console.WriteLine($"Your email address is correct.");
            }

            [OperationContract]
            public void Hi([CreditCard] string creditcardnumber)
            {
                Console.WriteLine($"Your credit card number is valid.");
            }
        }
    }
}
