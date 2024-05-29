using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Extend
{
    public class AutofacResolverExtend
    {
        private static Func<IContainer> _containerFactory;

        public static void SetContainerFactory(Func<IContainer> containerFactory)
        {
            _containerFactory = containerFactory ?? throw new ArgumentNullException(nameof(containerFactory));
        }

        private static IContainer Container
        {
            get
            {
                if (_containerFactory == null)
                {
                    throw new InvalidOperationException("Container factory is not initialized.");
                }

                return _containerFactory();
            }
        }

        /// <summary>
        /// 通过接口进行实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// 通过名称获取实例
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static TService ResolveNamed<TService>(string serviceName)
        {
            return Container.ResolveNamed<TService>(serviceName);
        }
    }
}
