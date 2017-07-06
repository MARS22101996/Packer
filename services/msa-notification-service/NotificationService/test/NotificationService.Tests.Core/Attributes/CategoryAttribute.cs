using System;
using Xunit.Sdk;

namespace NotificationService.Tests.Core.Attributes
{
    [TraitDiscoverer("NotificationService.Tests.Core.Attributes.CategoryDiscoverer", "NotificationService.Tests.Core")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CategoryAttribute : System.Attribute, ITraitAttribute
    {
        public CategoryAttribute(TestType category) { }
    }
}
