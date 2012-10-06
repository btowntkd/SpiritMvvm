using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
    [TestClass]
    public class DependsOnAttributeTest
    {
        /// <summary>
        /// Internal testing class which contains many property dependencies mapped;
        /// some are simple dependancy mappings, and some contain complex multiple & circular
        /// dependancies.
        /// </summary>
        public class SimpleClassWithDependantProperties
        {
            /// <summary>
            /// Contains the list of const property names in string form.
            /// </summary>
            public static class PropNames
            {
                public const string BaseProperty = "BaseProperty";
                public const string DependsOnBasePropertyDirect = "DependsOnBasePropertyDirect";
                public const string DependsOnBasePropertyIndirectOne = "DependsOnBasePropertyIndirectOne";
                public const string DependsOnBasePropertyIndirectTwo = "DependsOnBasePropertyIndirectTwo";
                public const string IndependantProperty = "IndependantProperty";
                public const string MultiCircularBaseProperty = "MultiCircularBaseProperty";
                public const string MultiCircularDependencyOne = "MultiCircularDependencyOne";
                public const string MultiCircularDependencyTwo = "MultiCircularDependencyTwo";
                public const string MultiCircularDependencyThree = "MultiCircularDependencyThree";
                public const string SelfDependantPropertyDirect = "SelfDependantPropertyDirect";
            }

            public int BaseProperty { get; set; }

            [DependsOn(PropNames.BaseProperty)]
            public int DependsOnBasePropertyDirect { get; set; }

            [DependsOn(PropNames.DependsOnBasePropertyDirect)]
            public int DependsOnBasePropertyIndirectOne { get; set; }

            [DependsOn(PropNames.DependsOnBasePropertyIndirectOne)]
            public int DependsOnBasePropertyIndirectTwo { get; set; }

            public int IndependantProperty { get; set; }

            public int MultiCircularBaseProperty { get; set; }

            [DependsOn(PropNames.MultiCircularBaseProperty)]
            [DependsOn(PropNames.MultiCircularDependencyTwo)]
            [DependsOn(PropNames.MultiCircularDependencyThree)]
            public int MultiCircularDependencyOne { get; set; }

            [DependsOn(PropNames.MultiCircularBaseProperty)]
            [DependsOn(PropNames.MultiCircularDependencyOne)]
            [DependsOn(PropNames.MultiCircularDependencyThree)]
            public int MultiCircularDependencyTwo { get; set; }

            [DependsOn(PropNames.MultiCircularBaseProperty)]
            [DependsOn(PropNames.MultiCircularDependencyOne)]
            [DependsOn(PropNames.MultiCircularDependencyTwo)]
            public int MultiCircularDependencyThree { get; set; }

            [DependsOn(PropNames.SelfDependantPropertyDirect)]
            public int SelfDependantPropertyDirect { get; set; }
        }

        /// <summary>
        /// Ensures that the constructor completes successfully for any input.
        /// </summary>
        [TestMethod]
        public void Constructor_Success()
        {
            DependsOnAttribute attribute = new DependsOnAttribute("RandomProp");
            Assert.IsNotNull(attribute);
        }

        [TestMethod]
        public void GetDirectDependants_ReturnValue_ContainsOnlyDirectlyDependantProperties()
        {
            //Retrieve the list of all properties which are directly dependant on "BaseProperty"
            var results = DependsOnAttribute.GetDirectDependants(typeof(SimpleClassWithDependantProperties),
                SimpleClassWithDependantProperties.PropNames.BaseProperty);

            //We expect only a single property to be returned: "DependsOnBasePropertyDirect."
            //All other properties are either not dependant or indirectly dependant.
            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyDirect),
                "Did not find expected result property: {0}", 
                SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyDirect);
            Assert.IsTrue(results.Count() == 1, "Received more properties than were expected.");
        }

        [TestMethod]
        public void GetDirectDependants_ReturnValue_DoesNotContainSelf()
        {
            //Retrieve the list of all properties which are directly dependant on "BaseProperty"
            var results = DependsOnAttribute.GetDirectDependants(typeof(SimpleClassWithDependantProperties),
                SimpleClassWithDependantProperties.PropNames.SelfDependantPropertyDirect);

            //We expect only a single property to be returned: "DependsOnBasePropertyDirect."
            //All other properties are either not dependant or indirectly dependant.
            Assert.IsFalse(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.SelfDependantPropertyDirect),
                "Results should not contain the property which was provided as the source");
        }

        /// <summary>
        /// Ensures that the <see cref="DependsOnAttribute.GetAllDependants"/> method
        /// returns all properties which are dependant (directly or indirectly) on a
        /// given property, and ONLY those properties.
        /// </summary>
        [TestMethod]
        public void GetAllDependants_ReturnValue_ContainsOnlyDirectAndIndirectDependants()
        {
            //Retrieve the list of all properties which are dependant on "BaseProperty,"
            //either directly or indirectly.
            var results = DependsOnAttribute.GetAllDependants(typeof(SimpleClassWithDependantProperties),
                SimpleClassWithDependantProperties.PropNames.BaseProperty);

            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyDirect),
                "Did not find expected result property: {0}",
                SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyDirect);
            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyIndirectOne),
                "Did not find expected result property: {0}",
                SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyIndirectOne);
            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyIndirectTwo),
                "Did not find expected result property: {0}",
                SimpleClassWithDependantProperties.PropNames.DependsOnBasePropertyIndirectTwo);
            Assert.IsTrue(results.Count() == 3, "Received more properties than were expected.");
        }

        /// <summary>
        /// Ensures that the <see cref="DependsOnAttribute.GetAllDependants"/> method
        /// correctly handles circular dependancies by removing duplicates.
        /// </summary>
        [TestMethod]
        public void GetAllDependants_ReturnValue_DoesNotContainDuplicates()
        {
            //Retrieve the list of all properties which are dependant on "MultiCircularBaseProperty,"
            //either directly or indirectly.
            var results = DependsOnAttribute.GetAllDependants(typeof(SimpleClassWithDependantProperties),
                SimpleClassWithDependantProperties.PropNames.MultiCircularBaseProperty);

            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyOne),
                "Did not find expected result property: {0}",
                SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyOne);
            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyTwo),
                "Did not find expected result property: {0}",
                SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyTwo);
            Assert.IsTrue(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyThree),
                "Did not find expected result property: {0}",
                SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyThree);
            Assert.IsTrue(results.Count() == 3, "Received more properties than were expected.");
        }

        /// <summary>
        /// Ensures that the results from the <see cref="DependsOnAttribute.GetAllDependants"/>
        /// method do not contain the source property.
        /// </summary>
        [TestMethod]
        public void GetAllDependants_ReturnValue_DoesNotContainSelf()
        {
            //Retrieve the list of all properties which are dependant on "MultiCircularBaseProperty,"
            //either directly or indirectly.
            var results = DependsOnAttribute.GetAllDependants(typeof(SimpleClassWithDependantProperties),
                SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyOne);

            Assert.IsFalse(results.Any((x) => x.Name == SimpleClassWithDependantProperties.PropNames.MultiCircularDependencyOne),
                "Results should not contain the property given as the source.");
        }
    }
}
