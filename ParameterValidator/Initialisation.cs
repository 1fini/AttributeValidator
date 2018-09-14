using Puresharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Puresharp.Legacy;

namespace AttributeValidator
{
    public static class Initialisation
    {
        [Startup]
        static public void AutoSubscription()
        {
            foreach(var type in System.Reflection.Assembly.GetAssembly(typeof(Initialisation)).DefinedTypes)
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

    public class EmailValidationAspect : Aspect
    {
        public override IEnumerable<Advisor> Manage(MethodBase method)
        {
            yield return Advice
                .For(method)
                .Parameter<EmailAddressAttribute>()
                .Validate((_Parameter, _Attribute, _Value) =>
                {
                    if (_Value == null) { throw new ArgumentNullException(_Parameter.Name); }
                    try { new MailAddress(_Value.ToString()); }
                    catch (Exception exception) { throw new ArgumentException(_Parameter.Name, exception); }
                });
        }
    }

    public class CreditCardValidationAspect : Aspect
    {
        public override IEnumerable<Advisor> Manage(MethodBase method)
        {
            yield return Advice
                .For(method)
                .Parameter<CreditCardAttribute>()
                .Validate((_Parameter, _Attribute, _Value) =>
                {
                    if (_Value == null) { throw new ArgumentNullException(_Parameter.Name); }
                    try { if (!(new CreditCardAttribute().IsValid(_Value))) { throw new Exception("Credit card not valid."); } ; }
                    catch (Exception exception) { throw new ArgumentException(_Parameter.Name, exception); }
                });
        }
    }
}

namespace Puresharp.Legacy
{
    static public class Validation
    {
        static public Advisor Validate<T>(this Advisor.Parameter<T> @this, Action<ParameterInfo, T, string> validate)
            where T : Attribute
        {
            return @this.Visit((_Parameter, _Attribute) => new Validator<T>(_Parameter, _Attribute, validate));
        }
    }

    public class Validator<T> : IVisitor
        where T : Attribute
    {
        private ParameterInfo m_Parameter;
        private T m_Attribute;
        private Action<ParameterInfo, T, string> m_Validate;

        public Validator(ParameterInfo parameter, T attribute, Action<ParameterInfo, T, string> validate)
        {
            this.m_Parameter = parameter;
            this.m_Attribute = attribute;
            this.m_Validate = validate;
        }

        void IVisitor.Visit<T>(Func<T> value)
        {
            var _value = value();
            this.m_Validate(this.m_Parameter, this.m_Attribute, _value == null ? null : _value.ToString());
        }
    }
}
