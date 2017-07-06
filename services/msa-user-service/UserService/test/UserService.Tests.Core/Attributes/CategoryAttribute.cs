using System;
using Xunit.Sdk;

namespace UserService.Tests.Core.Attributes
{
    [TraitDiscoverer("UserService.Tests.Core.Attributes.CategoryDiscoverer", "UserService.Tests.Core")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryAttribute : System.Attribute, ITraitAttribute
    {
        public CategoryAttribute(TestType category) { }
    }
}
