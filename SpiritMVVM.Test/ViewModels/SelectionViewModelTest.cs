using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.ViewModels;

namespace SpiritMVVM.Test.ViewModels
{
    /// <summary>
    /// Unit tests for the <see cref="SelectionViewModel{T}"/> class.
    /// </summary>
    [TestClass]
    public class SelectionViewModelTest
    {
        /// <summary>
        /// Ensures the default constructor is successful.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_Success()
        {
            SelectionViewModel<int> viewModel = new SelectionViewModel<int>();
        }

        /// <summary>
        /// Ensures the default constructor has a default IsSelected value of false.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_IsSelectedIsFalse()
        {
            SelectionViewModel<int> viewModel = new SelectionViewModel<int>();
            Assert.IsFalse(viewModel.IsSelected, "Expected IsSelected to be false by default.");
        }

        /// <summary>
        /// Ensures the default constructor has a default Value property of equal to default(T).
        /// </summary>
        [TestMethod]
        public void Constructor_Default_ItemIsDefaultClassInstance()
        {
            //Repeat with a couple different types
            Constructor_Default_ItemIsDefaultInstanceOfT<int>();
            Constructor_Default_ItemIsDefaultInstanceOfT<object>();
            Constructor_Default_ItemIsDefaultInstanceOfT<System.Net.IPAddress>();
            Constructor_Default_ItemIsDefaultInstanceOfT<string>();
        }

        /// <summary>
        /// Executes the Constructor_Default_ItemIsDefaultClassInstance test
        /// for the specified type.
        /// </summary>
        /// <typeparam name="TItem">The type for which to execute the unit test.</typeparam>
        private void Constructor_Default_ItemIsDefaultInstanceOfT<TItem>()
        {
            SelectionViewModel<TItem> viewModel = new SelectionViewModel<TItem>();
            Assert.AreEqual(viewModel.Item, default(TItem), "Expected Value to be the default class instance.");
        }

        /// <summary>
        /// Ensures the constructor with an initial isSelected parameter sets
        /// the IsSelected property to the specified state.
        /// </summary>
        [TestMethod]
        public void Constructor_WithItem_AssignsItemProperty()
        {
            int expectedValue = 12;
            SelectionViewModel<int> viewModel = new SelectionViewModel<int>(expectedValue);
            Assert.AreEqual(expectedValue, viewModel.Item, "Expected Item propertyName to equal the constructor parameter.");
        }

        /// <summary>
        /// Ensures the constructor with an initial isSelected parameter sets
        /// the IsSelected property to the specified state.
        /// </summary>
        [TestMethod]
        public void Constructor_WithItemAndInitialState_AssignsItemPropertyAndInitialState()
        {
            int expectedValue = 12;
            bool initialState = false;
            SelectionViewModel<int> viewModel = new SelectionViewModel<int>(expectedValue, initialState);
            Assert.AreEqual(expectedValue, viewModel.Item, "Expected Item propertyName to equal the constructor parameter.");
            Assert.AreEqual(initialState, viewModel.IsSelected, "Expected IsSelected propertyName to equal the constructor parameter.");

            expectedValue = 32;
            initialState = true;
            viewModel = new SelectionViewModel<int>(expectedValue, initialState);
            Assert.AreEqual(expectedValue, viewModel.Item, "Expected Item propertyName to equal the constructor parameter.");
            Assert.AreEqual(initialState, viewModel.IsSelected, "Expected IsSelected propertyName to equal the constructor parameter.");
        }

        /// <summary>
        /// Ensures that the IsSelected property raises the PropertyChanged event,
        /// when modified.
        /// </summary>
        [TestMethod]
        public void IsSelected_Changed_RaisesPropertyChangedEvent()
        {
            bool pass = false;
            SelectionViewModel<int> viewModel = new SelectionViewModel<int>();
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IsSelected")
                    pass = true;
            };
            viewModel.IsSelected = !viewModel.IsSelected;
            Assert.IsTrue(pass, "Expected PropertyChanged event, which was never raised.");
        }

        /// <summary>
        /// Ensures that the Item property raises the PropertyChanged event,
        /// when modified.
        /// </summary>
        [TestMethod]
        public void Item_Changed_RaisesPropertyChangedEvent()
        {
            bool pass = false;
            SelectionViewModel<int> viewModel = new SelectionViewModel<int>();
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Item")
                    pass = true;
            };
            viewModel.Item = 34;
            Assert.IsTrue(pass, "Expected PropertyChanged event, which was never raised.");
        }
    }
}