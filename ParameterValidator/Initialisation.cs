using Puresharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace ParameterValidator
{
    public static class Initialisation
    {
        [Startup]
        static public void AutoInscription()
        {
            foreach(var type in System.Reflection.Assembly.GetAssembly(typeof(EmailValidator)).DefinedTypes)
            {
                if (type.BaseType == typeof(Aspect))
                {
                    var aspectConstructorInfo = type.GetConstructor(new Type[] { });
                    var aspect = (Aspect)aspectConstructorInfo.Invoke(BindingFlags.CreateInstance, null, new Object[] { }, System.Globalization.CultureInfo.CurrentCulture);
                    aspect.Weave<Pointcut<Service>>();
                }
            }

            
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Service : Attribute
    {

    }

    public class EmailValidator : IValidator
    {
        public void Validate<T>(ParameterInfo parameter, T value)
        {
            try
            {
                new MailAddress(value.ToString());
            }
            catch(Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }

    public class EmailValidationAspect : Aspect
    {
        public override IEnumerable<Func<IAdvice>> Advise(MethodBase method)
        {
            yield return Validation<EmailAddressAttribute>.With<EmailValidator>(method);
        }
    }

    public class CreditCardValidator : IValidator
    {
        public void Validate<T>(ParameterInfo parameter, T value)
        {
            new CreditCardAttribute().IsValid(value);
        }
    }

    public class CreditCardValidationAspect : Aspect
    {
        public override IEnumerable<Func<IAdvice>> Advise(MethodBase method)
        {
            yield return Validation<CreditCardAttribute>.With<CreditCardValidator>(method);
        }
    }
}
