using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MySoft.Data;
using MySoft.Data.Design;

namespace MySoft.Tools.EntityDesign
{
    public class CodeGenHelper
    {
        private string outNs;
        IAdvOpt advOpt;

        public CodeGenHelper(string outNs, IAdvOpt advOpt)
        {
            this.outNs = outNs;
            this.advOpt = advOpt;
        }

        #region Helper Methods

        internal static AttributeType GetPropertyAttribute<AttributeType>(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(typeof(AttributeType), false);
            if (attrs != null && attrs.Length > 0)
            {
                return (AttributeType)attrs[0];
            }
            return default(AttributeType);
        }

        internal static AttributeType[] GetPropertyAttributes<AttributeType>(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(typeof(AttributeType), false);
            if (attrs != null && attrs.Length > 0)
            {
                AttributeType[] objs = new AttributeType[attrs.Length];
                for (int i = 0; i < attrs.Length; i++)
                {
                    objs[i] = (AttributeType)attrs[i];
                }
                return objs;
            }
            return null;
        }

        internal static AttributeType GetEntityAttribute<AttributeType>(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(AttributeType), false);
            if (attrs != null && attrs.Length > 0)
            {
                return (AttributeType)attrs[0];
            }
            return default(AttributeType);
        }

        internal static AttributeType[] GetEntityAttributes<AttributeType>(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(AttributeType), false);
            if (attrs != null && attrs.Length > 0)
            {
                AttributeType[] objs = new AttributeType[attrs.Length];
                for (int i = 0; i < attrs.Length; i++)
                {
                    objs[i] = (AttributeType)attrs[i];
                }
                return objs;
            }
            return null;
        }

        #endregion

        #region Gen Entities

        public string GenEntitiesEx(Assembly ass, int outLang)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace ns;
            //ns = new CodeNamespace(GetOutputNamespace(type));
            //unit.Namespaces.Add(ns);
            int i = 0;
            foreach (Type type in ass.GetTypes())
            {
                if (typeof(IEntity).IsAssignableFrom(type) && typeof(IEntity) != type && advOpt.IsEntityEnabled(type.Name))
                {
                    ns = new CodeNamespace(outNs);
                    unit.Namespaces.Add(ns);
                    ns.Imports.Add(new CodeNamespaceImport("System"));
                    //ns.Imports.Add(new CodeNamespaceImport("MySoft.Data"));
                    //ns.Imports.Add(new CodeNamespaceImport("MySoft.Data.Design"));
                    //sb.Append("namespace " + GetOutputNamespace(type) + "\r\n{\r\n");
                    GenEntityEx(ns, type, outLang);
                    //sb.Append("}\r\n\r\n");

                    ++i;
                }
            }

            CodeDomProvider provider = null;
            switch (outLang)
            {
                case 0: provider = new Microsoft.CSharp.CSharpCodeProvider();
                    break;
                case 1: provider = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                //case 2: provider = new Microsoft.VisualC.CppCodeProvider();
                //    break;
                //case 3: provider = new Microsoft.VisualC.CppCodeProvider7();
                //    break;
                //case 4: provider = new Microsoft.JScript.JScriptCodeProvider();
                //    break;
                default: provider = new Microsoft.CSharp.CSharpCodeProvider();
                    break;
            }

            StringBuilder codeBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(codeBuilder);
            IndentedTextWriter indentedWriter = new IndentedTextWriter(stringWriter, "  ");
            indentedWriter.Indent = 2;
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BlankLinesBetweenMembers = true;
            options.IndentString = "\t";
            provider.GenerateCodeFromCompileUnit(unit, indentedWriter, options);
            return codeBuilder.ToString();
        }

        #region Generate C# Entities

        private void GenPropertyQueryCodeEx(CodeTypeDeclaration entity, Type type, List<string> generatedProperties, bool isReadonly)
        {
            foreach (Type item in GetContractInterfaceTypes(type))
            {
                GenPropertyQueryCodeEx(entity, item, generatedProperties, isReadonly);
            }

            foreach (PropertyInfo item in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedProperties.Contains(item.Name))
                {
                    generatedProperties.Add(item.Name);

                    entity.Members.Add(GenMemberFieldEx(type, item, isReadonly));
                }
            }
        }

        private string GetTableName(Type type)
        {
            MappingAttribute table = GetEntityAttribute<MappingAttribute>(type);
            string tableName = type.Name;

            if (table != null)
            {
                tableName = table.Name;
            }
            return tableName;
        }

        private CodeMemberField GenMemberFieldEx(Type type, PropertyInfo item, bool isReadonly)
        {
            string fieldName = null;
            MappingAttribute field = GetPropertyAttribute<MappingAttribute>(item);
            if (field != null)
            {
                fieldName = field.Name;
            }
            else
            {
                fieldName = item.Name;
            }

            //gen static field
            CodeMemberField memberfield = new CodeMemberField();
            memberfield.Name = item.Name;
            //sb.Append("\t\t\tpublic static PropertyItem ");

            //sb.Append(" = new PropertyItem(\"");
            //sb.Append(item.Name);
            //sb.Append("\");\r\n");

            memberfield.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            memberfield.Type = new CodeTypeReference(typeof(Field));

            //new CodeObjectCreateExpression(typeof(AllField), new CodeExpression[] { new CodePrimitiveExpression(tableName) });

            //new CodeTypeReference(type.Name, )
            CodeTypeReference reference = new CodeTypeReference(typeof(Field).FullName, new CodeTypeReference(type.Name, CodeTypeReferenceOptions.GenericTypeParameter));

            if (item.Name == fieldName)
                memberfield.InitExpression = new CodeObjectCreateExpression(reference, new CodeExpression[] { new CodePrimitiveExpression(fieldName) });
            else
                memberfield.InitExpression = new CodeObjectCreateExpression(reference, new CodeExpression[] { new CodePrimitiveExpression(item.Name), new CodePrimitiveExpression(fieldName) });

            DescriptionAttribute ca = GetPropertyAttribute<DescriptionAttribute>(item);
            string typeName = item.PropertyType.Name;
            if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                typeName = Nullable.GetUnderlyingType(item.PropertyType).Name + "(可空)";
            }

            if (ca != null)
            {
                //sbProperties.Append("\t\t/// <summary>\r\n");
                //sbProperties.Append("\t\t/// ");
                //sbProperties.Append(ca.Content.Replace("\n", "\n\t\t/// "));
                //sbProperties.Append("\r\n\t\t/// </summary>\r\n");
                memberfield.Comments.Add(new CodeCommentStatement("<summary>", true));
                memberfield.Comments.Add(new CodeCommentStatement(ca.Description + string.Format(" - 字段名：{0} - 数据类型：{1}", fieldName, typeName), true));
                memberfield.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            else
            {
                memberfield.Comments.Add(new CodeCommentStatement("<summary>", true));
                memberfield.Comments.Add(new CodeCommentStatement(string.Format("字段名：{0} - 数据类型：{1}", fieldName, typeName), true));
                memberfield.Comments.Add(new CodeCommentStatement("</summary>", true));
            }

            return memberfield;
        }

        private void GenGetIdentityFieldEx(StringBuilder sb, Type type, List<string> generatedProperties, int outLang)
        {
            foreach (Type item in GetContractInterfaceTypes(type))
            {
                GenGetIdentityFieldEx(sb, item, generatedProperties, outLang);
            }

            ReadOnlyAttribute read = GetEntityAttribute<ReadOnlyAttribute>(type);

            foreach (PropertyInfo item in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedProperties.Contains(item.Name))
                {
                    if (!item.CanWrite)
                    {
                        if (read == null)
                        {
                            sb.Append((outLang == 0 ? "_" : "__") + ".");
                            sb.Append(item.Name);
                            sb.Append(", ");
                            generatedProperties.Add(item.Name);
                        }
                    }
                }
            }
        }

        private void GenGetPrimaryKeyFieldListEx(StringBuilder sb, Type type, List<string> generatedProperties, int outLang)
        {
            foreach (Type item in GetContractInterfaceTypes(type))
            {
                GenGetPrimaryKeyFieldListEx(sb, item, generatedProperties, outLang);
            }

            foreach (PropertyInfo item in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedProperties.Contains(item.Name))
                {
                    PrimaryKeyAttribute key = GetPropertyAttribute<PrimaryKeyAttribute>(item);
                    if (key != null)
                    {
                        sb.Append((outLang == 0 ? "_" : "__") + ".");
                        sb.Append(item.Name);
                        sb.Append(", ");

                        generatedProperties.Add(item.Name);
                    }
                }
            }

            //去除最后的逗号
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
        }

        private void GenGetFieldListEx(StringBuilder sb, Type type, List<string> generatedProperties, int outLang)
        {
            foreach (Type item in GetContractInterfaceTypes(type))
            {
                GenGetFieldListEx(sb, item, generatedProperties, outLang);
            }

            foreach (PropertyInfo item in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedProperties.Contains(item.Name))
                {
                    sb.Append((outLang == 0 ? "_" : "__") + ".");
                    sb.Append(item.Name);
                    sb.Append(", ");

                    generatedProperties.Add(item.Name);
                }
            }

            //去除最后的逗号
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
        }

        private void GenGetPropertyValues(StringBuilder sb, Type type, List<string> generatedProperties)
        {
            foreach (Type item in GetContractInterfaceTypes(type))
            {
                GenGetPropertyValues(sb, item, generatedProperties);
            }

            foreach (PropertyInfo item in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedProperties.Contains(item.Name))
                {
                    sb.Append("_");
                    sb.Append(item.Name);
                    sb.Append(", ");

                    generatedProperties.Add(item.Name);
                }
            }

            //去除最后的逗号
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
        }

        private Type[] GetContractInterfaceTypes(Type type)
        {
            List<Type> list = new List<Type>();
            Type[] interfaceTypes = type.GetInterfaces();
            foreach (Type interfaceType in interfaceTypes)
            {
                if (typeof(IEntity).IsAssignableFrom(interfaceType) && typeof(IEntity) != interfaceType)
                {
                    bool isInOtherInterfaces = false;
                    foreach (Type item in interfaceTypes)
                    {
                        if (item != interfaceType && typeof(IEntity).IsAssignableFrom(item) && typeof(IEntity) != item)
                        {
                            foreach (Type obj in item.GetInterfaces())
                            {
                                if (interfaceType == obj)
                                {
                                    isInOtherInterfaces = true;
                                    break;
                                }
                            }

                            if (isInOtherInterfaces)
                            {
                                break;
                            }
                        }
                    }

                    if (!isInOtherInterfaces)
                    {
                        list.Add(interfaceType);
                    }
                }
            }

            return list.ToArray();
        }

        private string RemoveTypePrefix(string typeName)
        {
            string name = typeName;
            while (name.Contains("."))
            {
                name = name.Substring(name.IndexOf(".")).TrimStart('.');
            }
            return name;
        }

        private string GenType(int outLang, string typeStr)
        {
            if (outLang == 0)
            {
                return GenTypeCSharp(typeStr);
            }
            else
            {
                return GenTypeVB(typeStr);
            }
        }

        private string GenTypeCSharp(string typeStr)
        {
            if (typeStr.StartsWith("System.Nullable`1["))
            {
                return GenTypeCSharp(typeStr.Substring("System.Nullable`1[".Length).Trim('[', ']')) + "?";
            }

            return typeStr.Replace("System.", "");
        }

        private string GenTypeVB(string typeStr)
        {
            if (typeStr.StartsWith("System.Nullable`1["))
            {
                return "System.Nullable(Of " + GenTypeCSharp(typeStr.Substring("System.Nullable`1[".Length).Trim('[', ']')) + ")";
            }

            if (typeStr.EndsWith("?"))
            {
                return "System.Nullable(Of " + GenTypeCSharp(typeStr.TrimEnd('?')) + ")";
            }

            typeStr = typeStr.Replace("{0}", "(").Replace("{1}", ")");

            return typeStr.Replace("System.", "");
        }

        #endregion

        #endregion

        #region new method

        private void GenEntityEx(CodeNamespace ns, Type type, int outLang)
        {
            CodeTypeDeclaration entity;
            StringBuilder sb = new StringBuilder();

            entity = new CodeTypeDeclaration(type.Name);

            ns.Types.Add(entity);
            entity.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(System.SerializableAttribute).Name));
            entity.IsClass = true;
            entity.IsPartial = true;
            Type[] interfaces = GetContractInterfaceTypes(type);
            bool findNonEntityBaseEntity = false;
            string entityBaseTypeName = null;
            foreach (Type item in interfaces)
            {
                if (typeof(IEntity).IsAssignableFrom(item) && (typeof(IEntity) != item))
                {
                    entityBaseTypeName = item.Name;
                    entity.BaseTypes.Add(entityBaseTypeName);
                    findNonEntityBaseEntity = true;
                    break;
                }
            }
            if (!findNonEntityBaseEntity)
            {
                entity.BaseTypes.Add(typeof(Entity));
            }

            string tableName = GetTableName(type);
            DescriptionAttribute ca = GetEntityAttribute<DescriptionAttribute>(type);

            #region 获取主键列

            StringBuilder sbKey = new StringBuilder();
            List<string> listKey = new List<string>();
            GenGetPrimaryKeyFieldListEx(sbKey, type, listKey, outLang);

            #endregion

            if (ca != null)
            {
                //sb.Append("\t/// <summary>\r\n");
                //sb.Append("\t/// ");
                //sb.Append(ca.Description.Replace("\n", "\n\t/// "));
                //sb.Append("\r\n\t/// </summary>\r\n");
                entity.Comments.Add(new CodeCommentStatement("<summary>", true));
                entity.Comments.Add(new CodeCommentStatement(ca.Description + string.Format(" - 表名：{0} 主键列：{1}", tableName, string.Join(",", listKey.ToArray())), true));
                entity.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            else
            {
                entity.Comments.Add(new CodeCommentStatement("<summary>", true));
                entity.Comments.Add(new CodeCommentStatement(string.Format("表名：{0} 主键列：{1}", tableName, string.Join(",", listKey.ToArray())), true));
                entity.Comments.Add(new CodeCommentStatement("</summary>", true));
            }

            bool isReadonly = false;
            ReadOnlyAttribute read = GetEntityAttribute<ReadOnlyAttribute>(type);
            if (read != null) isReadonly = true;

            //generate properties
            CodeStatementCollection reloadQueryStatements = new CodeStatementCollection();
            GenPropertiesEx(entity, reloadQueryStatements, type, isReadonly, outLang);

            List<string> generatedProperties = new List<string>();

            CodeMemberMethod method;
            CodeExpression[] arrayInit;
            StringBuilder sbPropertyValuesList;
            string[] fieldsList;

            #region 实现重载的信息

            #region 重载获取表名和只读

            CodeTypeReference reference = new CodeTypeReference(typeof(Table).FullName, new CodeTypeReference(type.Name, CodeTypeReferenceOptions.GenericTypeParameter));
            CodeExpression codeExpression = new CodeObjectCreateExpression(reference, new CodeExpression[] { new CodePrimitiveExpression(tableName) });

            //生成重载的方法
            method = new CodeMemberMethod();
            method.Name = "GetTable";
            method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(typeof(Table));

            //添加注释
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("获取实体对应的表名", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            //CodeAssignStatement ass = new CodeAssignStatement();
            //ass.Left = new CodeSnippetExpression("mappingTable");
            //ass.Right = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeSnippetExpression("EntityConfig"), "GetTable"), new CodePrimitiveExpression(tableName));

            //CodeConditionStatement condition = new CodeConditionStatement();
            //condition.Condition = new CodeBinaryOperatorExpression(new CodePrimitiveExpression(null), CodeBinaryOperatorType.IdentityEquality, new CodeSnippetExpression("mappingTable"));
            //condition.TrueStatements.Add(ass);

            //method.Statements.Add(condition);
            //method.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("mappingTable")));
            //entity.Members.Add(method);

            //new CodeTypeReference(type.Name,

            method.Statements.Add(new CodeMethodReturnStatement(codeExpression));
            entity.Members.Add(method);

            if (isReadonly)
            {
                method = new CodeMemberMethod();
                method.Name = "GetReadOnly";
                method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                method.ReturnType = new CodeTypeReference(typeof(bool));

                //添加注释
                method.Comments.Add(new CodeCommentStatement("<summary>", true));
                method.Comments.Add(new CodeCommentStatement("获取实体是否只读", true));
                method.Comments.Add(new CodeCommentStatement("</summary>", true));

                method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));
                entity.Members.Add(method);
            }

            SequenceAttribute auto = GetEntityAttribute<SequenceAttribute>(type);
            if (auto != null)
            {
                method = new CodeMemberMethod();
                method.Name = "GetSequence";
                method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                method.ReturnType = new CodeTypeReference(typeof(string));

                //添加注释
                method.Comments.Add(new CodeCommentStatement("<summary>", true));
                method.Comments.Add(new CodeCommentStatement("获取自增长列的名称", true));
                method.Comments.Add(new CodeCommentStatement("</summary>", true));

                method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(auto.Name)));
                entity.Members.Add(method);
            }

            #endregion

            sbPropertyValuesList = new StringBuilder();
            generatedProperties.Clear();
            //sb.Append("\t\tpublic override object[] GetPropertyValues()\r\n\t\t{\r\n");
            //sb.Append("\t\t\treturn new object[] { ");
            GenGetIdentityFieldEx(sbPropertyValuesList, type, generatedProperties, outLang);
            //sb.Append(sbPropertyValuesList.ToString().TrimEnd(' ', ','));
            //sb.Append(" };\r\n\t\t}\r\n\r\n");
            fieldsList = sbPropertyValuesList.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (generatedProperties.Count > 0)
            {
                method = new CodeMemberMethod();
                method.Name = "GetIdentityField";
                method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                method.ReturnType = new CodeTypeReference(typeof(Field));

                //添加注释
                method.Comments.Add(new CodeCommentStatement("<summary>", true));
                method.Comments.Add(new CodeCommentStatement("获取实体中的标识列", true));
                method.Comments.Add(new CodeCommentStatement("</summary>", true));

                method.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression(fieldsList[0].Trim())));
                entity.Members.Add(method);
            }

            sbPropertyValuesList = new StringBuilder();
            generatedProperties.Clear();
            //sb.Append("\t\tpublic override object[] GetPropertyValues()\r\n\t\t{\r\n");
            //sb.Append("\t\t\treturn new object[] { ");
            GenGetPrimaryKeyFieldListEx(sbPropertyValuesList, type, generatedProperties, outLang);
            //sb.Append(sbPropertyValuesList.ToString().TrimEnd(' ', ','));
            //sb.Append(" };\r\n\t\t}\r\n\r\n");
            fieldsList = sbPropertyValuesList.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            arrayInit = new CodeExpression[fieldsList.Length];
            for (int i = 0; i < fieldsList.Length; i++)
            {
                arrayInit[i] = new CodeSnippetExpression(fieldsList[i].Trim());
            }
            if (arrayInit.Length > 0)
            {
                method = new CodeMemberMethod();
                method.Name = "GetPrimaryKeyFields";
                method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                method.ReturnType = new CodeTypeReference(new CodeTypeReference(typeof(Field)), 1);

                //添加注释
                method.Comments.Add(new CodeCommentStatement("<summary>", true));
                method.Comments.Add(new CodeCommentStatement("获取实体中的主键列", true));
                method.Comments.Add(new CodeCommentStatement("</summary>", true));

                method.Statements.Add(new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(Field), arrayInit)));
                entity.Members.Add(method);
            }

            method = new CodeMemberMethod();
            method.Name = "GetFields";
            method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(new CodeTypeReference(typeof(Field)), 1);

            sbPropertyValuesList = new StringBuilder();
            generatedProperties.Clear();
            //sb.Append("\t\tpublic override object[] GetPropertyValues()\r\n\t\t{\r\n");
            //sb.Append("\t\t\treturn new object[] { ");
            GenGetFieldListEx(sbPropertyValuesList, type, generatedProperties, outLang);
            //sb.Append(sbPropertyValuesList.ToString().TrimEnd(' ', ','));
            //sb.Append(" };\r\n\t\t}\r\n\r\n");
            fieldsList = sbPropertyValuesList.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            arrayInit = new CodeExpression[generatedProperties.Count];
            for (int i = 0; i < generatedProperties.Count; i++)
            {
                arrayInit[i] = new CodeSnippetExpression(fieldsList[i].Trim());
            }
            if (arrayInit.Length > 0)
            {
                //添加注释
                method.Comments.Add(new CodeCommentStatement("<summary>", true));
                method.Comments.Add(new CodeCommentStatement(string.Format("获取列信息"), true));
                method.Comments.Add(new CodeCommentStatement("</summary>", true));

                method.Statements.Add(new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(Field), arrayInit)));
                entity.Members.Add(method);
            }

            #endregion

            method = new CodeMemberMethod();
            method.Name = "GetValues";
            method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(new CodeTypeReference(typeof(object)), 1);

            sbPropertyValuesList = new StringBuilder();
            generatedProperties.Clear();
            //sb.Append("\t\tpublic override object[] GetPropertyValues()\r\n\t\t{\r\n");
            //sb.Append("\t\t\treturn new object[] { ");
            GenGetPropertyValues(sbPropertyValuesList, type, generatedProperties);
            //sb.Append(sbPropertyValuesList.ToString().TrimEnd(' ', ','));
            //sb.Append(" };\r\n\t\t}\r\n\r\n");
            fieldsList = sbPropertyValuesList.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            arrayInit = new CodeExpression[generatedProperties.Count];
            for (int i = 0; i < generatedProperties.Count; i++)
            {
                arrayInit[i] = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), fieldsList[i].Trim());
            }
            if (arrayInit.Length > 0)
            {
                //添加注释
                method.Comments.Add(new CodeCommentStatement("<summary>", true));
                method.Comments.Add(new CodeCommentStatement(string.Format("获取列数据"), true));
                method.Comments.Add(new CodeCommentStatement("</summary>", true));

                method.Statements.Add(new CodeMethodReturnStatement(new CodeArrayCreateExpression(typeof(object), arrayInit)));
                entity.Members.Add(method);
            }
            else
            {
                method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
                entity.Members.Add(method);
            }


            method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            method.Name = "SetValues";
            method.ReturnType = null;

            //添加注释
            method.Comments.Add(new CodeCommentStatement("<summary>", true));
            method.Comments.Add(new CodeCommentStatement("给当前实体赋值", true));
            method.Comments.Add(new CodeCommentStatement("</summary>", true));

            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IRowReader), "reader"));
            //sb.Append("\t\tpublic override void SetPropertyValues(System.Data.IDataReader reader)\r\n\t\t{\r\n");
            generatedProperties.Clear();
            GenSetPropertyValuesFromReaderEx(method.Statements, type, generatedProperties, outLang);
            entity.Members.Add(method);

            //outNs + "." + 
            string entityOutputTypeName = type.Name;

            //, CodeTypeReferenceOptions.GlobalReference
            CodeTypeReference entityOutputTypeNameRef = new CodeTypeReference(entityOutputTypeName);
            //sb.Append("\t\tpublic override int GetHashCode() { return base.GetHashCode(); }\r\n\r\n");
            method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            method.Name = "GetHashCode";
            method.ReturnType = new CodeTypeReference(typeof(int));
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "GetHashCode", new CodeExpression[] { })));
            entity.Members.Add(method);

            //sb.Append("\t\tpublic override bool Equals(object obj)\r\n\t\t{\r\n\t\t\treturn obj == null || (!(obj is " + entityOutputTypeName + ")) ? false : ((object)this) == ((object)obj) ? true : this.isAttached && ((" + entityOutputTypeName + ")obj).isAttached");
            method = new CodeMemberMethod();
            method.Name = "Equals";
            method.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            method.ReturnType = new CodeTypeReference(typeof(bool));
            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "obj"));
            //if (obj == null) return false;
            //if ((obj is global::Entities.LocalUser) == false) return false;
            //if (((object)this) == ((object)obj)) return true;
            //return this.isAttached && ((global::Entities.LocalUser)obj).isAttached && this.ID == ((global::Entities.LocalUser)obj).ID;
            method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("obj"), CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null)), new CodeStatement[] { new CodeMethodReturnStatement(new CodePrimitiveExpression(false)) }));
            method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false), CodeBinaryOperatorType.ValueEquality, new CodeMethodInvokeExpression(new CodeTypeOfExpression(entityOutputTypeNameRef), "IsAssignableFrom", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("obj"), "GetType", new CodeExpression[] { }) })), new CodeStatement[] { new CodeMethodReturnStatement(new CodePrimitiveExpression(false)) }));
            method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeCastExpression(typeof(object), new CodeThisReferenceExpression()), CodeBinaryOperatorType.IdentityEquality, new CodeCastExpression(typeof(object), new CodeArgumentReferenceExpression("obj"))), new CodeStatement[] { new CodeMethodReturnStatement(new CodePrimitiveExpression(true)) }));
            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(false)));

            //sb.Append(";\r\n\t\t}\r\n");
            entity.Members.Add(method);
            //sb.Append("\r\n\t\t#endregion\r\n\r\n");

            CodeTypeDeclaration queryClass = new CodeTypeDeclaration();
            queryClass.IsClass = true;
            queryClass.Name = outLang == 0 ? "_" : "__";
            queryClass.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            if (findNonEntityBaseEntity)
            {
                queryClass.Attributes |= MemberAttributes.New;
            }
            entity.Members.Add(queryClass);

            generatedProperties.Clear();

            #region 添加All字段

            if (interfaces.Length == 0)
            {
                CodeMemberField field = new CodeMemberField();
                field.Name = "All";
                field.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                field.Type = new CodeTypeReference(typeof(AllField));

                //new CodeTypeReference(type.Name,
                reference = new CodeTypeReference(typeof(AllField).FullName, new CodeTypeReference(type.Name, CodeTypeReferenceOptions.GenericTypeParameter));
                field.InitExpression = new CodeObjectCreateExpression(reference);

                //添加注释
                field.Comments.Add(new CodeCommentStatement("<summary>", true));
                field.Comments.Add(new CodeCommentStatement("表示选择所有列，与*等同", true));
                field.Comments.Add(new CodeCommentStatement("</summary>", true));

                queryClass.Members.Add(field);
            }

            #endregion

            GenPropertyQueryCodeEx(queryClass, type, generatedProperties, isReadonly);
        }

        private void GenSetPropertyValuesFromReaderEx(CodeStatementCollection statements, Type type, List<string> generatedProperties, int outLang)
        {
            foreach (Type interfaceType in GetContractInterfaceTypes(type))
            {
                GenSetPropertyValuesFromReaderEx(statements, interfaceType, generatedProperties, outLang);
            }

            foreach (PropertyInfo item in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedProperties.Contains(item.Name))
                {
                    CodeAssignStatement assign = new CodeAssignStatement();
                    assign.Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + item.Name);
                    assign.Right = GenReaderGetEx(item, outLang);

                    generatedProperties.Add(item.Name);

                    string name = (outLang == 0 ? "_" : "__") + "." + item.Name;
                    CodeConditionStatement condition = new CodeConditionStatement();
                    condition.Condition = new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false), CodeBinaryOperatorType.ValueEquality,
                        new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "IsDBNull", new CodeExpression[] { new CodeArgumentReferenceExpression(name) }));

                    condition.TrueStatements.Add(assign);

                    statements.Add(condition);
                }
            }
        }

        private CodeExpression GenReaderGetEx(PropertyInfo item, int outLang)
        {
            string className = outLang == 0 ? "_" : "__";
            string name = className + "." + item.Name;

            if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?))
            {
                //sb.Append("reader.GetBoolean");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetBoolean", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(byte) || item.PropertyType == typeof(byte?))
            {
                //sb.Append("reader.GetByte");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetByte", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?))
            {
                //sb.Append("GetDateTime");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetDateTime", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(decimal) || item.PropertyType == typeof(decimal?))
            {
                //sb.Append("reader.GetDecimal");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetDecimal", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(double) || item.PropertyType == typeof(double?))
            {
                //sb.Append("reader.GetDouble");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetDouble", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(float) || item.PropertyType == typeof(float?))
            {
                //sb.Append("reader.GetFloat");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetFloat", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(Guid) || item.PropertyType == typeof(Guid?))
            {
                //sb.Append("GetGuid");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetGuid", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(short) || item.PropertyType == typeof(short?))
            {
                //sb.Append("reader.GetInt16");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetInt16", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(int) || item.PropertyType == typeof(int?))
            {
                //sb.Append("reader.GetInt32");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetInt32", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(long) || item.PropertyType == typeof(long?))
            {
                //sb.Append("reader.GetInt64");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetInt64", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(string))
            {
                //sb.Append("reader.GetString");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetString", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType == typeof(byte[]))
            {
                //sb.Append("reader.GetByte");
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetBytes", new CodeExpression[] { new CodeSnippetExpression(name) });
            }
            else if (item.PropertyType.IsEnum)
            {
                return new CodeCastExpression(new CodeTypeReference(item.PropertyType), new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetInt32", new CodeExpression[] { new CodeSnippetExpression(name) }));
            }
            else if (item.PropertyType == typeof(object))
            {
                return new CodeMethodInvokeExpression(new CodeArgumentReferenceExpression("reader"), "GetValue", new CodeExpression[] { new CodeSnippetExpression(name) });
            }

            //sb.Append("reader.GetValue<TObject>");
            string typeName = GenType(outLang, item.PropertyType.FullName);
            if (typeName.IndexOf(",") > 0)
            {
                typeName = typeName.Substring(0, typeName.IndexOf(",")) + (outLang == 0 ? "?" : ")");
            }
            return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeArgumentReferenceExpression("reader"), "GetValue", new CodeTypeReference(new CodeTypeParameter(typeName))), new CodeSnippetExpression(name));
        }

        private void GenPropertiesEx(CodeTypeDeclaration entity, CodeStatementCollection reloadQueryStatements, Type type, bool isReadOnly, int outLang)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo pi in pis)
            {
                list.Add(pi);
            }

            Type[] types = GetContractInterfaceTypes(type);
            if (types.Length > 1)
            {
                List<Type> typelist = new List<Type>();
                typelist.AddRange(types);
                typelist.RemoveAt(0);
                foreach (PropertyInfo pi in DeepGetProperties(typelist.ToArray()))
                {
                    list.Add(pi);
                }

                foreach (PropertyInfo pi in DeepGetProperties(types[0]))
                {
                    for (int index = list.Count - 1; index >= 0; index--)
                    {
                        if (list[index].Name == pi.Name)
                        {
                            list.RemoveAt(index);
                            break;
                        }
                    }
                }
            }

            List<string> generatedPropertyNames = new List<string>();
            foreach (PropertyInfo item in list)
            {
                //暂不支持关联处理，所以是类则跳过
                if (item.PropertyType.IsInterface) continue;

                if (!generatedPropertyNames.Contains(item.Name))
                {
                    //GenNormalProperty(sbFields, sbProperties, item);
                    GenNormalPropertyEx(entity, type, isReadOnly, item, outLang);

                    generatedPropertyNames.Add(item.Name);
                }
            }
        }

        private void GenNormalPropertyEx(CodeTypeDeclaration entity, Type type, bool isReadOnly, PropertyInfo item, int outLang)
        {
            CodeMemberField field = new CodeMemberField();
            field.Name = "_" + item.Name;
            field.Attributes = MemberAttributes.Family;
            //, CodeTypeReferenceOptions.GlobalReference
            field.Type = new CodeTypeReference(GenType(outLang, item.PropertyType.ToString()));
            entity.Members.Add(field);

            //sbFields.Append("\t\tprotected ");
            //sbFields.Append(GenType(outLang, item.PropertyType.ToString()));
            //sbFields.Append(" _");
            //sbFields.Append(item.Name);
            //sbFields.Append(";\r\n");

            CodeMemberProperty property = new CodeMemberProperty();

            property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            //, CodeTypeReferenceOptions.GlobalReference
            property.Type = new CodeTypeReference(GenType(outLang, item.PropertyType.ToString()));
            property.Name = item.Name;
            DescriptionAttribute ca = GetPropertyAttribute<DescriptionAttribute>(item);
            if (ca != null)
            {
                //sbProperties.Append("\t\t/// <summary>\r\n");
                //sbProperties.Append("\t\t/// ");
                //sbProperties.Append(ca.Content.Replace("\n", "\n\t\t/// "));
                //sbProperties.Append("\r\n\t\t/// </summary>\r\n");
                property.Comments.Add(new CodeCommentStatement("<summary>", true));
                property.Comments.Add(new CodeCommentStatement(ca.Description, true));
                property.Comments.Add(new CodeCommentStatement("</summary>", true));
            }
            //sbProperties.Append("\t\tpublic ");
            //sbProperties.Append(GenType(outLang, item.PropertyType.ToString()));
            //sbProperties.Append(" ");
            //sbProperties.Append(item.Name);
            //sbProperties.Append("\r\n\t\t{\r\n");
            if (item.CanRead)
            {
                property.HasGet = true;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + item.Name)));

                //sbProperties.Append("\t\t\tget { return _");
                //sbProperties.Append(item.Name);
                //sbProperties.Append("; }\r\n");
            }
            //if (item.CanWrite)
            //{

            property.HasSet = true;

            if (!isReadOnly)
            {
                property.SetStatements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "OnPropertyValueChange",
                    new CodeSnippetExpression[]{
                            new CodeSnippetExpression((outLang == 0 ? "_." : "__.") + item.Name),
                            new CodeSnippetExpression("_" + item.Name),
                            new CodeSnippetExpression("value")
                        }));
            }

            property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_" + item.Name), new CodePropertySetValueReferenceExpression()));

            //sbProperties.Append("\t\t\tset { OnPropertyChanged(\"");
            //sbProperties.Append(item.Name);
            //sbProperties.Append("\", _");
            //sbProperties.Append(item.Name);
            //sbProperties.Append(", value); _");
            //sbProperties.Append(item.Name);
            //sbProperties.Append(" = value; }\r\n");


            //}

            entity.Members.Add(property);

            //sbProperties.Append("\t\t}\r\n\r\n");
        }
        #endregion

        public static PropertyInfo[] DeepGetProperties(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return new PropertyInfo[0];
            }
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (Type t in types)
            {
                if (t != null)
                {
                    foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        list.Add(pi);
                    }

                    if (t.IsInterface)
                    {
                        Type[] interfaceTypes = t.GetInterfaces();

                        if (interfaceTypes != null)
                        {
                            foreach (PropertyInfo pi in DeepGetProperties(interfaceTypes))
                            {
                                bool isContained = false;

                                foreach (PropertyInfo item in list)
                                {
                                    if (item.Name == pi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(pi);
                                }
                            }
                        }
                    }
                    else
                    {
                        Type baseType = t.BaseType;

                        if (baseType != typeof(object) && baseType != typeof(ValueType))
                        {
                            foreach (PropertyInfo pi in DeepGetProperties(baseType))
                            {
                                bool isContained = false;

                                foreach (PropertyInfo item in list)
                                {
                                    if (item.Name == pi.Name)
                                    {
                                        isContained = true;
                                        break;
                                    }
                                }

                                if (!isContained)
                                {
                                    list.Add(pi);
                                }
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }
    }
}
