using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TicketService.Tests.Core.Attributes
{
    public class CategoryDiscoverer : ITraitDiscoverer
    {
        private const string Key = "Category";

        /// <summary>
        /// Gets the trait values from the Category attribute.
        /// </summary>
        /// <param name="traitAttribute">The trait attribute containing the trait values.</param>
        /// <returns>The trait values.</returns>
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
            yield return new KeyValuePair<string, string>(Key, ctorArgs[0].ToString());
        }
    }
}