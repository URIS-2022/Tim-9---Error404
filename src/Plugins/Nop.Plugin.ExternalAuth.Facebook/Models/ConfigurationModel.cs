using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.ExternalAuth.Facebook.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>

    [Obsolete("Obsolete with custom message")] //izmenjeno dodat obsolete
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientKeyIdentifier")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Facebook.ClientSecret")]
        public string ClientSecret { get; set; }
    }
}