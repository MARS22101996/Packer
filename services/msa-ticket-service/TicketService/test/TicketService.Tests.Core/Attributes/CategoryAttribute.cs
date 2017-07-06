using System;
using TicketService.Tests.Core.Enums;
using Xunit.Sdk;

namespace TicketService.Tests.Core.Attributes
{
    /// <summary>
    /// Apply this attribute to your test method to specify a category.
    /// </summary> 
    [TraitDiscoverer("TicketService.Tests.Core.Attributes.CategoryDiscoverer", "TicketService.Tests.Core")]
    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute : Attribute, ITraitAttribute
    {
        public CategoryAttribute(TestType testType) { }
    }
}