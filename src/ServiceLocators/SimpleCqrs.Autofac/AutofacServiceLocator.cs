using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace SimpleCqrs.Autofac
{
    public class AutofacServiceLocator : IServiceLocator
    {

        /// <summary>
        /// Error message used when the type specified in the register methods is null
        /// </summary>
        public const string TypeNullErrorMessage = "The container does not accept null types to be registered";

        /// <summary>
        /// Error message used when the key specified in the register methods is null, empty or a string with white spaces only
        /// </summary>
        public const string KeyNullErrorMessage = "The key cannot be null, empty or a string with white spaces only";

        /// <summary>
        /// Error message used when the kernel is null
        /// </summary>
        public const string BuilderNullErrorMessage = "The specified builder cannot be null";

        /// <summary>
        /// Error message used when the reset method is called, actually it is not supported because Autofac does not suppor reseting the container
        /// </summary>
        public const string ResetingContainerErrorMessage = "Autofac does not support reseting the container";

        /// <summary>
        /// Error message used when the instance passed to the register methods is null
        /// </summary>
        public const string InstanceNullErrorMessage = "Null objects cannot be registered in the container";

        /// <summary>
        /// Error message used when the calling delegate passed to the register methods is null
        /// </summary>
        public const string CallingDelegateNullErrorMessage = "The calling delegate cannot be null";

        private ContainerBuilder _builder;

        private bool _registrationsUpdated;

        private IContainer _container;

		public AutofacServiceLocator(){}


        public AutofacServiceLocator(ContainerBuilder builder) : this()
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder", BuilderNullErrorMessage);
            }

            _builder = builder;
        }

        public IContainer Container
        {
            get
            {
                if(_container == null)
                {
                    _container = Builder.Build();
                }
                else if (_registrationsUpdated)
                {
                    Builder.Update(_container);
                }
                _registrationsUpdated = false;

                return _container;
            }
        }

		public T Resolve<T>() where T : class
		{
			try
			{
				return Container.Resolve<T>();
			}
			catch (Exception ex)
			{
				throw new ServiceResolutionException(typeof(T), ex);
			}
		}

		public T Resolve<T>(string key) where T : class
		{
			try
			{
                return Container.ResolveNamed<T>(key);
			}
			catch (Exception ex)
			{
				throw new ServiceResolutionException(typeof(T), ex);
			}
		}

		public object Resolve(Type type)
		{
			try
			{
				return Container.Resolve(type);
			}
			catch (Exception ex)
			{
				throw new ServiceResolutionException(type, ex);
			}
		}

		public IList<T> ResolveServices<T>() where T : class
		{
		    return Container.Resolve<IEnumerable<T>>().ToList();
		}

		public void Register<TInterface>(Type implType) where TInterface : class
		{
            if (implType == null)
            {
                throw new ArgumentNullException("implType", TypeNullErrorMessage);
            }

			var key = string.Format("{0}-{1}", typeof(TInterface).Name, implType.FullName);

		    Builder.RegisterType(implType).Named<TInterface>(key);

			// Work-around, also register this implementation to service mapping
			// without the generated key above.
            Builder.RegisterType(implType).As<TInterface>();
		}

		public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
		{
            Builder.RegisterType<TImplementation>().As<TInterface>();
		}

		public void Register<TInterface, TImplementation>(string key) where TImplementation : class, TInterface
		{
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", KeyNullErrorMessage);
            }

            Builder.RegisterType<TImplementation>().Named<TInterface>(key);
		}

        public void Register(string key, Type implType)
		{

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key", KeyNullErrorMessage);
            }

            if (implType == null)
            {
                throw new ArgumentNullException("implType", TypeNullErrorMessage);
            }

            Builder.RegisterType(implType).Named(key, implType);
		}

		public void Register(Type serviceType, Type implType)
		{
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType", TypeNullErrorMessage);
            }

            if (implType == null)
            {
                throw new ArgumentNullException("implType", TypeNullErrorMessage);
            }

            Builder.RegisterType(implType).As(serviceType);
		}

		public void Register<TInterface>(TInterface instance) where TInterface : class
		{
            if (instance == null)
            {
                throw new ArgumentNullException("instance", InstanceNullErrorMessage);
            }

            Builder.RegisterInstance(instance).As<TInterface>();
		}

		public void Register<TInterface>(Func<TInterface> factoryMethod) where TInterface : class
		{
            if (factoryMethod == null)
            {
                throw new ArgumentNullException("factoryMethod", CallingDelegateNullErrorMessage);
            }

            Builder.Register(x => factoryMethod()).As<TInterface>();
		}

		public void Release(object instance)
		{
			//Not needed for StructureMap it doesn't keep references beyond the life cycle that was configured.
		}

		public void Reset()
		{
            throw new NotImplementedException(ResetingContainerErrorMessage);
		}

		public TService Inject<TService>(TService instance) where TService : class
		{
            if (instance == null)
            {
                throw new ArgumentNullException("instance", InstanceNullErrorMessage);
            }

		    Container.InjectProperties(instance);
			
			return instance;
		}

		public void TearDown<TService>(TService instance) where TService : class
		{
			//Not needed for Autofac it doesn't keep references beyond the life cycle that was configured.
		}

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;
            
            if (disposing)
            {
                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
            }
            IsDisposed = true;
        }


        protected ContainerBuilder Builder
        {
            get
            {
                if(_builder == null)
                {
                    _registrationsUpdated = true;
                    _builder = new ContainerBuilder();
                }
                return _builder;
            }
        }
    }
}