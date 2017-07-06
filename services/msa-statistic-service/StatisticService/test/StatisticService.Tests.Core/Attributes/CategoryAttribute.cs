using System;
using StatisticService.Tests.Core.Enums;
using Xunit.Sdk;

namespace StatisticService.Tests.Core.Attributes
{
    [TraitDiscoverer("StatisticService.Tests.Core.Attributes.CategoryDiscoverer", "StatisticService.Tests.Core")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryAttribute : System.Attribute, ITraitAttribute
    {
        public CategoryAttribute(TestType category) { }
    }
}
