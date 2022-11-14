using System;

namespace Nop.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// Represents custom attribute for UPS code
    /// </summary>
    //dodato
    [AttributeUsage(AttributeTargets.All)]
    public class UPSCodeAttribute : Attribute
    {
        #region Ctor


        public UPSCodeAttribute(string codeValue)
        {
            Code = codeValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a code value
        /// </summary>
        public string Code { get; }

        #endregion
    }
}