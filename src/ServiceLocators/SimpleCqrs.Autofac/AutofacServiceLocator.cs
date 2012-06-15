using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace SimpleCqrs.Autofac
{
    public class AutofacServiceLocator : IServiceLocator
    {
       	private bool _isDisposing;
        private ContainerBuilder _builder;

        private bool _registrationsUpdated;

        private IContainer _container;

		public AutofacServiceLocator(){}

        public AutofacServiceLocator(IContainer container) : this()
        {
            _container = container;
        }

        public AutofacServiceLocator(ContainerBuilder builder) : this()
        {
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
            Builder.RegisterType<TImplementation>().Named<TInterface>(key);
		}

		public void Register(string key, Type type)
		{
            Builder.RegisterType(type).Named(key, type);
		}

		public void Register(Type serviceType, Type implType)
		{
            Builder.RegisterType(implType).As(serviceType);
		}

		public void Register<TInterface>(TInterface instance) where TInterface : class
		{
            Builder.RegisterInstance(instance).As<TInterface>();
		}

		public void Register<TInterface>(Func<TInterface> factoryMethod) where TInterface : class
		{
            Builder.Register(x => factoryMethod).As<TInterface>();
		}

		public void Release(object instance)
		{
			//Not needed for StructureMap it doesn't keep references beyond the life cycle that was configured.
		}

		public void Reset()
		{
            Dispose();
		}

		public TService Inject<TService>(TService instance) where TService : class
		{
			if (instance == null)
				return null;

		    Container.InjectProperties(instance);
			
			return instance;
		}

		public void TearDown<TService>(TService instance) where TService : class
		{
			//Not needed for Autofac it doesn't keep references beyond the life cycle that was configured.
		}

		public void Dispose()
		{
			if (_isDisposing) return;

            if (_container == null) return;

			_isDisposing = true;
			_container.Dispose();
			_container = null;
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