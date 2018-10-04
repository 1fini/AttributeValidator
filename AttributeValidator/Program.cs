using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using Puresharp;

namespace AttributeValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var toto = new Toto();
                //toto.Hello("toto");
                //toto.Hi("toto@gmail.com");
                //toto.AddPhone("+33676573012");
                toto.AddEmailCheckOnCustomAttribute("toto@gmail.com");
                toto.AddEmailCheckOnCustomAttribute("toto");
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
                Console.WriteLine($"Your email address {email} is correct.");
            }

            [OperationContract]
            public void Hi([CreditCard] string creditcardnumber)
            {
                Console.WriteLine($"Your credit card number {creditcardnumber} is valid.");
            }

            [OperationContract]
            public void AddPhone([Phone] string phoneNumber)
            {
                Console.WriteLine($"Your phone number {phoneNumber} is considered valid.");
            }

            [OperationContract]
            public void AddUrl([Url] string url)
            {
                Console.WriteLine($"Your phone number {url} is considered valid.");
            }

            [CustomValidation]
            public void AddEmailCheckOnCustomAttribute([EmailAddress] string email)
            {
                Console.WriteLine($"Your address {email} has passed the check.");
            }
        }

        /// <summary>
        /// Defines a new attribute to use on the methods to be verfied
        /// </summary>
        public class CustomValidationAttribute : Attribute
        {
        }

        /// <summary>
        /// Defines a new Weaver that can bind an aspect on an attribute
        /// </summary>
        public class CustomValidationWeaver : IAttributeValidatorWeaver
        {
            public void Weave(Aspect validationAspect)
            {
                validationAspect.Weave<Pointcut<CustomValidationAttribute>>();
            }
        }
    }
}
