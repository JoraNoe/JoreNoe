using Autofac;
using System;

namespace JoreNoe.Extend
{
    public class AutofacResolverExtend
    {
        // 使用 Lazy<T> 确保线程安全且只初始化一次
        private static Lazy<ILifetimeScope> _containerFactory;

        /// <summary>
        /// 设置容器工厂
        /// </summary>
        /// <param name="containerFactory">容器工厂</param>
        public static void SetContainerFactory(Func<ILifetimeScope> containerFactory)
        {
            // 确保容器工厂不会为空
            _containerFactory = new Lazy<ILifetimeScope>(containerFactory ?? throw new ArgumentNullException(nameof(containerFactory)));
        }

        /// <summary>
        /// 获取当前的容器
        /// </summary>
        private static ILifetimeScope Container
        {
            get
            {
                if (_containerFactory == null)
                {
                    throw new InvalidOperationException("Container factory is not initialized.");
                }

                // 使用 Lazy<T> 确保容器工厂初始化只发生一次
                return _containerFactory.Value;
            }
        }

        /// <summary>
        /// 通过接口解析实例
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns>解析出的实例</returns>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// 通过名称解析实例
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="serviceName">服务名称</param>
        /// <returns>解析出的实例</returns>
        public static TService ResolveNamed<TService>(string serviceName)
        {
            return Container.ResolveNamed<TService>(serviceName);
        }
    }
}
