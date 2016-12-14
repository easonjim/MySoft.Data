using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data.Mvp
{
    public interface IPresenterFactory
    {
        TPresenter GetPresenter<TPresenter>(object view) where TPresenter : IPresenter;
    }

    //控制器
    public interface IPresenter
    {
        void BindView(object view);
        void BindModel(params object[] models);
        Type TypeOfView { get; }
        Type[] TypeOfModel { get; }
    }

    public abstract class Presenter<TView> : IPresenter
    {
        private TView _view;
        protected TView view
        {
            get
            {
                return _view;
            }
        }

        public void BindView(object view)
        {
            this._view = (TView)view;
        }

        public void BindModel(params object[] models)
        {
            //继承此类无model
        }

        public Type TypeOfView
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type[] TypeOfModel
        {
            get
            {
                return null;
            }
        }
    }

    public abstract class Presenter<TView, TModel> : IPresenter
    {
        private TView _view;
        protected TView view
        {
            get
            {
                return _view;
            }
        }

        private TModel _model;
        protected TModel model
        {
            get
            {
                return _model;
            }
        }

        public void BindView(object view)
        {
            this._view = (TView)view;
        }

        public void BindModel(params object[] models)
        {
            this._model = (TModel)models[0];
        }

        public Type TypeOfView
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type[] TypeOfModel
        {
            get
            {
                return new Type[] { typeof(TModel) };
            }
        }
    }

    public abstract class Presenter<TView, TModel1, TModel2> : IPresenter
    {
        private TView _view;
        protected TView view
        {
            get
            {
                return _view;
            }
        }

        private TModel1 _model1;
        protected TModel1 model1
        {
            get
            {
                return _model1;
            }
        }

        private TModel2 _model2;
        protected TModel2 model2
        {
            get
            {
                return _model2;
            }
        }

        public void BindView(object view)
        {
            this._view = (TView)view;
        }

        public void BindModel(params object[] models)
        {
            this._model1 = (TModel1)models[0];
            this._model2 = (TModel2)models[1];
        }

        public Type TypeOfView
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type[] TypeOfModel
        {
            get
            {
                return new Type[] { typeof(TModel1), typeof(TModel2) };
            }
        }
    }

    public abstract class Presenter<TView, TModel1, TModel2, TModel3> : IPresenter
    {
        private TView _view;
        protected TView view
        {
            get
            {
                return _view;
            }
        }

        private TModel1 _model1;
        protected TModel1 model1
        {
            get
            {
                return _model1;
            }
        }

        private TModel2 _model2;
        protected TModel2 model2
        {
            get
            {
                return _model2;
            }
        }

        private TModel3 _model3;
        protected TModel3 model3
        {
            get
            {
                return _model3;
            }
        }

        public void BindView(object view)
        {
            this._view = (TView)view;
        }

        public void BindModel(params object[] models)
        {
            this._model1 = (TModel1)models[0];
            this._model2 = (TModel2)models[1];
            this._model3 = (TModel3)models[2];
        }

        public Type TypeOfView
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type[] TypeOfModel
        {
            get
            {
                return new Type[] { typeof(TModel1), typeof(TModel2), typeof(TModel3) };
            }
        }
    }

    public abstract class Presenter<TView, TModel1, TModel2, TModel3, TModel4> : IPresenter
    {
        private TView _view;
        protected TView view
        {
            get
            {
                return _view;
            }
        }

        private TModel1 _model1;
        protected TModel1 model1
        {
            get
            {
                return _model1;
            }
        }

        private TModel2 _model2;
        protected TModel2 model2
        {
            get
            {
                return _model2;
            }
        }

        private TModel3 _model3;
        protected TModel3 model3
        {
            get
            {
                return _model3;
            }
        }

        private TModel4 _model4;
        protected TModel4 model4
        {
            get
            {
                return _model4;
            }
        }

        public void BindView(object view)
        {
            this._view = (TView)view;
        }

        public void BindModel(params object[] models)
        {
            this._model1 = (TModel1)models[0];
            this._model2 = (TModel2)models[1];
            this._model3 = (TModel3)models[2];
            this._model4 = (TModel4)models[3];
        }

        public Type TypeOfView
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type[] TypeOfModel
        {
            get
            {
                return new Type[] { typeof(TModel1), typeof(TModel2), typeof(TModel3), typeof(TModel4) };
            }
        }
    }

    public abstract class Presenter<TView, TModel1, TModel2, TModel3, TModel4, TModel5> : IPresenter
    {
        private TView _view;
        protected TView view
        {
            get
            {
                return _view;
            }
        }

        private TModel1 _model1;
        protected TModel1 model1
        {
            get
            {
                return _model1;
            }
        }

        private TModel2 _model2;
        protected TModel2 model2
        {
            get
            {
                return _model2;
            }
        }

        private TModel3 _model3;
        protected TModel3 model3
        {
            get
            {
                return _model3;
            }
        }

        private TModel4 _model4;
        protected TModel4 model4
        {
            get
            {
                return _model4;
            }
        }

        private TModel5 _model5;
        protected TModel5 model5
        {
            get
            {
                return _model5;
            }
        }

        public void BindView(object view)
        {
            this._view = (TView)view;
        }

        public void BindModel(params object[] models)
        {
            this._model1 = (TModel1)models[0];
            this._model2 = (TModel2)models[1];
            this._model3 = (TModel3)models[2];
            this._model4 = (TModel4)models[3];
            this._model5 = (TModel5)models[4];
        }

        public Type TypeOfView
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type[] TypeOfModel
        {
            get
            {
                return new Type[] { typeof(TModel1), typeof(TModel2), typeof(TModel3), typeof(TModel4), typeof(TModel5) };
            }
        }
    }
}
