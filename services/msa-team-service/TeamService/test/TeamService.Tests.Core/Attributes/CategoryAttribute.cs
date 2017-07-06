using System;
using TeamService.Tests.Core.Enums;
using Xunit.Sdk;

namespace TeamService.Tests.Core.Attributes
{
    [TraitDiscoverer("TeamService.Tests.Core.Attributes.CategoryDiscoverer", "TeamService.Tests.Core")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryAttribute : System.Attribute, ITraitAttribute
    {
        public CategoryAttribute(TestType category) { }
    }
}
