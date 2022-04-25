using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Autofac.Extras.DynamicProxy;
namespace HzTalk
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static ILifetimeScope LifetimeScope { get; private set; }

        public static T Resolve<T>()
        {
            return (T)LifetimeScope.Resolve(typeof(T));
        }

        static App()
        {
            var builder = new Autofac.ContainerBuilder();
            builder.RegisterType<HzTalk.InterceptedHandler>().AsSelf().SingleInstance();
            builder.RegisterType<LoginUser>().As<IUserInfo>()
                 .SingleInstance()
                .InterceptedBy(typeof(HzTalk.InterceptedHandler))
                 .EnableInterfaceInterceptors();

            builder.RegisterType<RootData>().AsSelf().SingleInstance();
            builder.RegisterType<ChatList>().AsSelf().SingleInstance();

            var container = builder.Build();
            LifetimeScope = container.BeginLifetimeScope(typeof(App).ToString());
        }
    }
}
