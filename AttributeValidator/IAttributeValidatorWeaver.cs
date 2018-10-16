using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puresharp.AttributeValidator
{
    /// <summary>
    /// Interface to be implemented to bind an aspect
    /// </summary>
    public interface IAttributeValidatorWeaver
    {
        void Weave(Aspect validationAspect);
    }
}
