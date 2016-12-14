using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using MySoft.Data.Design;

namespace MySoft.Data
{
    /// <summary>
    /// ʵ�����
    /// </summary>
    [Serializable]
    public abstract class Entity : EntityBase, IEntity
    {
        #region ���÷���

        #region ʵ����²������(��ԭ�е��ֶ�)

        #region �ı����״̬

        /// <summary>
        /// ��ʵ����Ϊ�޸�״̬
        /// </summary>
        public void Attach()
        {
            lock (this)
            {
                isUpdate = true;
            }
        }

        /// <summary>
        /// ��ʵ����Ϊ����״̬
        /// </summary>
        public void Detach()
        {
            lock (this)
            {
                isUpdate = false;
            }
        }

        #endregion

        #region �Ƴ�ָ������

        /// <summary>
        /// ��ʵ����Ϊ�޸�״̬���Ƴ�ָ������
        /// </summary>
        /// <param name="removeFields"></param>
        public void Attach(params Field[] removeFields)
        {
            lock (this)
            {
                isUpdate = true;
                RemoveFieldsToUpdate(removeFields);
            }
        }

        /// <summary>
        /// ��ʵ����Ϊ����״̬���Ƴ�ָ������
        /// </summary>
        /// <param name="removeFields"></param>
        public void Detach(params Field[] removeFields)
        {
            lock (this)
            {
                isUpdate = false;
                RemoveFieldsToInsert(removeFields);
            }
        }

        #endregion

        #endregion

        #region ʵ����²������(�����е��ֶ�)

        #region �ı����״̬

        /// <summary>
        /// ��ʵ����Ϊ�޸�״̬(�����ֶ�)
        /// </summary>
        public void AttachAll()
        {
            lock (this)
            {
                AddFieldsToUpdate(this.GetFields());
                Attach();
            }
        }

        /// <summary>
        /// ��ʵ����Ϊ����״̬(�����ֶ�)
        /// </summary>
        public void DetachAll()
        {
            lock (this)
            {
                removeinsertlist.Clear();
                Detach();
            }
        }

        #endregion

        #region �Ƴ�ָ������

        /// <summary>
        /// ��ʵ����Ϊ�޸�״̬���Ƴ�ָ������(�����ֶ�)
        /// </summary>
        /// <param name="removeFields"></param>
        public void AttachAll(params Field[] removeFields)
        {
            lock (this)
            {
                AddFieldsToUpdate(this.GetFields());
                Attach(removeFields);
            }
        }

        /// <summary>
        /// ��ʵ����Ϊ����״̬���Ƴ�ָ������(�����ֶ�)
        /// </summary>
        /// <param name="removeFields"></param>
        public void DetachAll(params Field[] removeFields)
        {
            lock (this)
            {
                removeinsertlist.Clear();
                Detach(removeFields);
            }
        }

        #endregion

        #region ָ���Ƴ��������

        /// <summary>
        /// ��ʵ����Ϊ�޸�״̬������ָ������
        /// </summary>
        /// <param name="setFields"></param>
        public void AttachSet(params Field[] setFields)
        {
            lock (this)
            {
                updatelist.Clear();
                AddFieldsToUpdate(setFields);
                Attach();
            }
        }

        /// <summary>
        /// ��ʵ����Ϊ����״̬������ָ������
        /// </summary>
        /// <param name="setFields"></param>
        public void DetachSet(params Field[] setFields)
        {
            lock (this)
            {
                removeinsertlist.Clear();
                List<Field> fields = new List<Field>(setFields);
                List<Field> list = new List<Field>(this.GetFields());
                list.RemoveAll(f =>
                {
                    if (fields.Contains(f)) return true;
                    return false;
                });
                RemoveFieldsToInsert(list);
                Detach();
            }
        }

        #endregion

        #endregion

        #endregion

        #region ���صķ���

        /// <summary>
        /// ���ӵ������Ե��޸��б�
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected void OnPropertyValueChange(Field field, object oldValue, object newValue)
        {
            lock (this)
            {
                bool isChanged = false;
                //����Ǵ����ݿ��ж�����ʵ�壬���ж��Ƿ���Ҫ����
                if (isFromDB)
                {
                    if (oldValue != null)
                    {
                        if (!oldValue.Equals(newValue))
                        {
                            AddFieldsToUpdate(new Field[] { field });
                            isChanged = true;
                        }
                    }
                    else if (newValue != null)
                    {
                        if (!newValue.Equals(oldValue))
                        {
                            AddFieldsToUpdate(new Field[] { field });
                            isChanged = true;
                        }
                    }

                    //���ֵ�иı䣬����ԭʼ����
                    if (isChanged && originalObject == null)
                    {
                        try
                        {
                            originalObject = this.As<IEntityBase>().Clone();
                        }
                        catch (Exception ex)
                        {
                            throw new MySoftException(ex.Message);
                        }
                    }
                }
                else
                {
                    AddFieldsToUpdate(new Field[] { field });
                }
            }
        }

        /// <summary>
        /// ����ֶε��޸��б�
        /// </summary>
        /// <param name="fields"></param>
        private void AddFieldsToUpdate(IList<Field> fields)
        {
            if (fields == null || fields.Count == 0) return;

            foreach (Field field in fields)
            {
                if (!updatelist.Exists(p => p.Name == field.Name))
                {
                    updatelist.Add(field);
                }
            }
        }

        /// <summary>
        /// �Ƴ��ֶε��޸��б�
        /// </summary>
        /// <param name="fields"></param>
        private void RemoveFieldsToUpdate(IList<Field> fields)
        {
            if (fields == null || fields.Count == 0) return;

            foreach (Field field in fields)
            {
                updatelist.RemoveAll(p => p.Name == field.Name);
            }
        }

        /// <summary>
        /// �Ƴ��ֶε��޸��б�
        /// </summary>
        /// <param name="fields"></param>
        private void RemoveFieldsToInsert(IList<Field> fields)
        {
            if (fields == null || fields.Count == 0) return;

            foreach (Field field in fields)
            {
                if (!removeinsertlist.Exists(p => p.Name == field.Name))
                {
                    removeinsertlist.Add(field);
                }
            }
        }

        #endregion
    }


}
