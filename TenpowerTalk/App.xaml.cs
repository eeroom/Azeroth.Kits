using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Autofac;
namespace TenpowerTalk
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

            builder.RegisterType<LoginUser>().AsSelf().SingleInstance();
            builder.RegisterType<RootData>().AsSelf().SingleInstance();

            var container = builder.Build();
            LifetimeScope = container.BeginLifetimeScope(typeof(App).ToString());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
           
        }

    }
}
