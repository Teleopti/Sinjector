using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Sinjector.Internals
{
	internal class Injector
	{
		private IComponentContext _container;
		private readonly List<object> _targets = new List<object>();

		public Injector Source(IComponentContext container)
		{
			_container = container;
			return this;
		}

		public Injector Target(object target)
		{
			_targets.Add(target);
			return this;
		}

		public void Inject() => InjectFrom(_container);

		public void InjectFrom(IComponentContext container) =>
			_targets.ForEach(x => injectTo(container, x));

		private static void injectTo(IComponentContext container, object target)
		{
			var type = target.GetType();
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.CanWrite);
			properties.ForEach(x => x.SetValue(target, container.Resolve(x.PropertyType), null));
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			fields.ForEach(x => x.SetValue(target, container.Resolve(x.FieldType)));
		}

		internal static void Inject(object target, object instance)
		{
			var type = target.GetType();
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.CanWrite)
				.Where(x => x.PropertyType.IsInstanceOfType(instance));
			properties.ForEach(x => x.SetValue(target, instance, null));
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.FieldType.IsInstanceOfType(instance));
			fields.ForEach(x => x.SetValue(target, instance));
		}
	}
}
