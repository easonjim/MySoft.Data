using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace MySoft.Data.Mvp
{
    /// <summary>
    /// 创建Presenter的接口
    /// </summary>
    public sealed class PresenterFactory : IPresenterFactory
    {
        private static IPresenterFactory singleton;
        private Dictionary<string, MethodInfo> methods;
        private IServiceFactory container;
        private PresenterFactory()
        {
            container = ServiceFactory.Create();
            methods = new Dictionary<string, MethodInfo>();
        }

        public static IPresenterFactory Create()
        {
            if (singleton == null)
            {
                singleton = new PresenterFactory();
            }
            return singleton;
        }

        public TPresenter GetPresenter<TPresenter>(object view)
            where TPresenter : IPresenter
        {
            Type controllerType = typeof(TPresenter);
            TPresenter controller = (TPresenter)CreateObject(controllerType);
            if (controller != null)
            {
                if (controller.TypeOfView.IsAssignableFrom(view.GetType()))
                {
                    controller.BindView(view);
                }
                if (controller.TypeOfModel != null)
                {
                    Type[] types = controller.TypeOfModel;
                    object[] models = new object[types.Length];
                    for (int index = 0; index < types.Length; index++)
                    {
                        models[index] = CreateObject(types[index]);
                    }
                    controller.BindModel(models);
                }
            }
            return controller;
        }

        private object CreateObject(Type type)
        {
            if (type.IsClass)
            {
                return Activator.CreateInstance(type);
            }
            else if (container != null)
            {
                if (container.Services.ContainsKey(type.FullName))
                {
                    MethodInfo method;
                    if (methods.ContainsKey(type.FullName))
                    {
                        method = methods[type.FullName];
                    }
                    else
                    {
                        method = container.GetType().GetMethod("GetService", Type.EmptyTypes).MakeGenericMethod(type);
                        methods.Add(type.FullName, method);
                    }
                    return method.Invoke(container, null);
                }
            }
            return null;
        }
    }
}
