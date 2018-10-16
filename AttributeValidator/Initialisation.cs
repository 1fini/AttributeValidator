using Puresharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Puresharp.Legacy;
using System.ServiceModel;

namespace Puresharp.AttributeValidator
{
    internal static class Initialisation
    {
        static Initialisation()
        {
            var additionnalWeavers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(_A => _A.GetTypes()).Where(_T => typeof(IAttributeValidatorWeaver).IsAssignableFrom(_T) && _T.IsClass).Select(_T => Activator.CreateInstance(_T) as IAttributeValidatorWeaver).ToArray();

            foreach (var type in System.Reflection.Assembly.GetAssembly(typeof(Initialisation)).DefinedTypes)
            {
                if (type.BaseType == typeof(Aspect))
                {
                    var aspectConstructorInfo = type.GetConstructor(new Type[] { });
                    var aspect = (Aspect)aspectConstructorInfo.Invoke(BindingFlags.CreateInstance, null, new Object[] { }, System.Globalization.CultureInfo.CurrentCulture);
                    if (additionnalWeavers.Length == 0)
                    {
                        aspect.Weave<Pointcut<OperationContractAttribute>>();
                    }
                    else
                    {
                        foreach (var aw in additionnalWeavers)
                        {
                            aw.Weave(aspect);
                        }
                    }
                }
            }
        }

        [Startup]
        static public void AutoInscription()
        {
        }
    }
}


