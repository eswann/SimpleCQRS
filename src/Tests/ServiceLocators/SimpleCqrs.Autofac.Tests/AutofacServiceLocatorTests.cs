﻿using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using FluentAssertions.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleCqrs.Autofac.Tests
{
    [TestClass]
    public class AutofacServiceLocatorTests
    {
        [TestMethod]
        public void can_create_new_default_instance()
        {
            var sut = new AutofacServiceLocator();

            sut.Should().NotBeNull();
        }

        [TestMethod]
        public void can_initialize_a_new_instance_with_a_existing_container()
        {
            var sut = new AutofacServiceLocator(new ContainerBuilder().Build());

            sut.Should().NotBeNull();
        }

        [TestMethod]
        public void when_initializing_a_new_instance_with_a_null_container_it_should_throw_an_argument_null_exception()
        {
            AutofacServiceLocator sut = null;
            Action initializing = () => sut = new AutofacServiceLocator(null);
            initializing.ShouldThrow<ArgumentNullException>()
                .WithMessage(AutofacServiceLocator.ContainerNullErrorMessage, ComparisonMode.Substring)
                .WithMessage("container", ComparisonMode.Substring);
        }
    }

    [TestClass]
    public class TheResolveMethod
    {
        [TestMethod]
        public void it_should_resolve_the_type_registered_with_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>();
                helper.AutofacServiceLocator.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>();
            }
        }

        [TestMethod]
        public void
            it_should_resolve_the_type_registered_with_the_generic_contract_specified_and_with_the_key_specifdied()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>("my key");
                helper.AutofacServiceLocator.Resolve<IMyTestingContract>("my key").Should().NotBeNull().And.BeOfType
                    <MyImplementation>();
            }
        }

        [TestMethod]
        public void it_should_resolve_the_type_registered_for_the_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>();
                helper.AutofacServiceLocator.Resolve(typeof(IMyTestingContract)).Should().NotBeNull().And.BeOfType
                    <MyImplementation>();
            }
        }
    }

    [TestClass]
    public class TheResolveServicesMethod
    {
        [TestMethod]
        public void
            it_should_return_an_empty_enumerable_when_there_are_not_types_registered_for_the_generic_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.ResolveServices<IMyTestingContract>().Should().NotBeNull().And.
                    HaveCount(0);
            }
        }

        [TestMethod]
        public void it_should_resolve_all_the_types_registered_for_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>();
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation3>();
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation2>();
                helper.AutofacServiceLocator.ResolveServices<IMyTestingContract>().Should()
                    .NotBeNull().And.HaveCount(3).And.ContainItemsAssignableTo<IMyTestingContract>()
                    .And.OnlyHaveUniqueItems()
                    .And.Match(x => x.OfType<MyImplementation>().Count() == 1)
                    .And.Match(x => x.OfType<MyImplementation2>().Count() == 1)
                    .And.Match(x => x.OfType<MyImplementation3>().Count() == 1);
            }
        }
    }

    [TestClass]
    public class TheRegisteMethod
    {
        [TestMethod]
        public void
            it_should_throw_an_ArgumentNullException_when_the_register_type_specified_is_null_for_the_generic_contract_specified
            ()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateType(
                    helper.AutofacServiceLocator.Invoking(x => x.Register<IMyTestingContract>((Type) null)),
                    "implType");
            }
        }

        [TestMethod]
        public void it_should_register_the_type_specified_with_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract>(typeof (MyImplementation));
                helper.Container.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>();
            }
        }

        [TestMethod]
        public void it_should_register_the_generic_type_specified_with_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>();
                helper.Container.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>();
            }
        }

        [TestMethod]
        public void
            it_should_throw_an_ArgumentNullException_when_the_key_specified_is_null_for_the_generic_contract_and_the_generic_type_specified
            ()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateKey(
                    helper.AutofacServiceLocator.Invoking(
                        x => x.Register<IMyTestingContract, MyImplementation>(null)));
                helper.ValidateKey(
                    helper.AutofacServiceLocator.Invoking(
                        x => x.Register<IMyTestingContract, MyImplementation>(string.Empty)));
                helper.ValidateKey(
                    helper.AutofacServiceLocator.Invoking(x => x.Register<IMyTestingContract, MyImplementation>(" ")));
            }
        }

        [TestMethod]
        public void it_should_register_the_generic_type_with_the_generic_contract_and_the_key_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>("my key");
                helper.Container.ResolveNamed<IMyTestingContract>("my key").Should().BeOfType<MyImplementation>();
            }
        }

        [TestMethod]
        public void it_should_throw_an_ArgumentNullException_when_the_key_is_null_for_the_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateKey(
                    helper.AutofacServiceLocator.Invoking(x => x.Register((string) null, typeof (MyImplementation))));
                helper.ValidateKey(
                    helper.AutofacServiceLocator.Invoking(x => x.Register(string.Empty, typeof (MyImplementation))));
                helper.ValidateKey(
                    helper.AutofacServiceLocator.Invoking(x => x.Register(" ", typeof (MyImplementation))));
            }
        }

        [TestMethod]
        public void it_should_throw_an_ArgumentNullException_when_the_type_is_null_for_the_key_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateType(helper.AutofacServiceLocator.Invoking(x => x.Register("my key", null)), "implType");
            }
        }

        [TestMethod]
        public void it_should_register_the_specified_type_with_the_key_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register("my key", typeof (MyImplementation));
                helper.Container.ResolveNamed<MyImplementation>("my key").Should().NotBeNull().And.BeOfType
                    <MyImplementation>();
            }
        }

        [TestMethod]
        public void it_should_throw_an_ArgumentNullException_when_the_contract_type_is_null_for_the_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateType(
                    helper.AutofacServiceLocator.Invoking(x => x.Register((Type) null, typeof (MyImplementation))),
                    "serviceType");
            }
        }

        [TestMethod]
        public void it_should_throw_an_ArgumentNullException_when_the_type_is_null_for_the_contract_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateType(
                    helper.AutofacServiceLocator.Invoking(x => x.Register(typeof (IMyTestingContract), (Type) null)),
                    "implType");
            }
        }

        [TestMethod]
        public void it_should_register_the_type_specified_with_the_contract_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register(typeof (IMyTestingContract), typeof (MyImplementation));
                helper.Container.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>();
            }
        }

        [TestMethod]
        public void
            it_should_throw_an_ArgumentNullException_when_the_instance_is_null_for_the_generic_type_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateInstance(
                    helper.AutofacServiceLocator.Invoking(
                        x => x.Register<IMyTestingContract>((IMyTestingContract) null)), "instance");
            }
        }

        [TestMethod]
        public void it_should_register_the_instance_specified_with_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                var myType = new MyImplementation();
                helper.AutofacServiceLocator.Register<IMyTestingContract>(myType);
                helper.Container.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>().And.Be(myType);
            }
        }

        [TestMethod]
        public void
            it_should_throw_an_ArgumentNullException_when_the_delegate_is_null_for_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateFuncDelegate(
                    helper.AutofacServiceLocator.Invoking(
                        x => x.Register<IMyTestingContract>((Func<IMyTestingContract>) null)), "factoryMethod");
            }
        }


        [TestMethod]
        public void
            it_should_register_the_instance_returned_from_the_Func_delegate_with_the_generic_contract_specified()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                var myType = new MyImplementation();
                helper.AutofacServiceLocator.Register<IMyTestingContract>(() => myType);
                helper.Container.Resolve<IMyTestingContract>().Should().BeOfType<MyImplementation>().And.Be(myType);
            }
        }
    }

    [TestClass]
    public class TheReleaseMethod
    {
        [TestMethod]
        public void when_the_instance_is_null_it_should_not_throw_any_exceptions()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Invoking(x => x.Release(null)).ShouldNotThrow();
            }
        }

        [TestMethod]
        public void it_should_release_the_specified_object()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                var myObject = new MyImplementation();

                helper.AutofacServiceLocator.Invoking(x => x.Release(myObject));

                helper.AutofacServiceLocator.Register<IMyTestingContract>(myObject);
                helper.AutofacServiceLocator.Invoking(x => x.Release(myObject))
                    .ShouldNotThrow();
            }
        }
    }

    [TestClass]
    public class TheTearDownMethod
    {
        [TestMethod]
        public void when_the_instance_is_null_it_should_not_throw_any_exceptions()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Invoking(x => x.TearDown<IMyTestingContract>(null)).ShouldNotThrow();
            }
        }

        [TestMethod]
        public void it_should_tear_down_the_specified_instance()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                var myObject = new MyImplementation();

                helper.AutofacServiceLocator.Invoking(x => x.Release(myObject));

                helper.AutofacServiceLocator.Register<IMyTestingContract>(myObject);
                helper.AutofacServiceLocator.Invoking(x => x.TearDown(myObject))
                    .ShouldNotThrow();
            }
        }
    }

    [TestClass]
    public class TheResetMethod
    {
        [TestMethod]
        public void
            it_should_throw_a_NotImplementedException_because_Autofac_does_not_support_reseting_the_container()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Invoking(x => x.Reset()).ShouldThrow<NotImplementedException>();
            }
        }
    }

    [TestClass]
    public class TheDisposeMethod
    {
        [TestMethod]
        public void it_should_dispose_the_service_locator_container()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.Container.Should().NotBeNull();
                helper.AutofacServiceLocator.Register<IMyTestingContract, MyImplementation>();
                helper.AutofacServiceLocator.IsDisposed.Should().BeFalse();
                helper.AutofacServiceLocator.Dispose();
                GC.Collect();
                helper.AutofacServiceLocator.IsDisposed.Should().BeTrue();
            }
        }
    }

    [TestClass]
    public class TheInjectMethod
    {
        [TestMethod]
        public void it_should_throw_an_ArgumentNullException_when_the_instance_to_be_injected_is_null()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.ValidateInstance(
                    helper.AutofacServiceLocator.Invoking(x => x.Inject<IMyTestingContract>(null)), "instance");
            }
        }

        [TestMethod]
        public void it_should_inject_an_existing_instance()
        {
            using (var helper = new AutofacServiceLocatorHelper())
            {
                helper.AutofacServiceLocator.Register<Additionaltype>(new Additionaltype());
                var myObject = new MyImplementation();
                myObject = helper.AutofacServiceLocator.Inject(myObject);
                myObject.Should().NotBeNull().And.BeOfType<MyImplementation>();
                myObject.AdditionalType.Should().NotBeNull().And.BeOfType<Additionaltype>();
            }
        }
    }


    internal class AutofacServiceLocatorHelper : IDisposable
    {
        public IContainer Container { get { return AutofacServiceLocator.Container; } }
        public AutofacServiceLocator AutofacServiceLocator { get; private set; }

        public AutofacServiceLocatorHelper()
        {
            this.AutofacServiceLocator = new AutofacServiceLocator(new ContainerBuilder().Build());
        }

        public void Dispose()
        {
            this.AutofacServiceLocator.Dispose();
        }

        public void ValidateKey(Action action)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage(AutofacServiceLocator.KeyNullErrorMessage,
                             ComparisonMode.Substring)
                .WithMessage("key", ComparisonMode.Substring);
        }

        public void ValidateType(Action action, string argumentName)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage(AutofacServiceLocator.TypeNullErrorMessage, ComparisonMode.Substring)
                .WithMessage(argumentName, ComparisonMode.Substring);
        }

        public void ValidateInstance(Action action, string argumentName)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage(AutofacServiceLocator.InstanceNullErrorMessage, ComparisonMode.Substring)
                .WithMessage(argumentName, ComparisonMode.Substring);
        }

        public void ValidateFuncDelegate(Action action, string argumentName)
        {
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage(AutofacServiceLocator.CallingDelegateNullErrorMessage, ComparisonMode.Substring)
                .WithMessage(argumentName, ComparisonMode.Substring);
        }
    }

    internal interface IMyTestingContract
    {
        int Add(int num1, int num2);
    }

    internal class Additionaltype
    {
    }

    internal class MyImplementation : IMyTestingContract
    {

        public Additionaltype AdditionalType { get; set; }

        public int Add(int num1, int num2)
        {
            throw new NotImplementedException();
        }
    }

    internal class MyImplementation2 : IMyTestingContract
    {
        public int Add(int num1, int num2)
        {
            throw new NotImplementedException();
        }
    }

    internal class MyImplementation3 : IMyTestingContract
    {
        public int Add(int num1, int num2)
        {
            throw new NotImplementedException();
        }
    }
}
