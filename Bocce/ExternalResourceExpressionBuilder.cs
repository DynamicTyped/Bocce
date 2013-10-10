using System;
using System.Diagnostics;
using System.Globalization;
using System.CodeDom;
using System.Threading;
using System.Web.Compilation;
using System.Web.UI;

namespace Bocce
{
    /// <summary>
    /// Custom expression builder support for $ExternalResources expressions.
    /// </summary>
    public class ExternalResourceExpressionBuilder : ExpressionBuilder
    {
        private static ResourceProviderFactory s_resourceProviderFactory;

        public ExternalResourceExpressionBuilder()
        {
            Debug.WriteLine("ExternalResourceExpressionBuilder");

        }

        public static object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceExpressionBuilder.GetGlobalResourceObject({0}, {1})", classKey, resourceKey));

            return ExternalResourceExpressionBuilder.GetGlobalResourceObject(classKey, resourceKey, null);
        }

        public static object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceExpressionBuilder.GetGlobalResourceObject({0}, {1}, {2})", classKey, resourceKey, culture));

            ExternalResourceExpressionBuilder.EnsureResourceProviderFactory();
            IResourceProvider provider = ExternalResourceExpressionBuilder.s_resourceProviderFactory.CreateGlobalResourceProvider(classKey);
            return provider.GetObject(resourceKey, culture);
        }

        public override object EvaluateExpression(object target, BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceExpressionBuilder.EvaluateExpression({0}, {1}, {2}, {3})", target, entry, parsedData, context));

            ExternalResourceExpressionFields fields = parsedData as ExternalResourceExpressionFields;

            ExternalResourceExpressionBuilder.EnsureResourceProviderFactory();
            IResourceProvider provider = ExternalResourceExpressionBuilder.s_resourceProviderFactory.CreateGlobalResourceProvider(fields.ClassKey);

            return provider.GetObject(fields.ResourceKey, null);
        }

        public override System.CodeDom.CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceExpressionBuilder.GetCodeExpression({0}, {1}, {2})", entry, parsedData, context));

            ExternalResourceExpressionFields fields = parsedData as ExternalResourceExpressionFields;

            CodeMethodInvokeExpression exp = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(ExternalResourceExpressionBuilder)), "GetGlobalResourceObject", new CodePrimitiveExpression(fields.ClassKey), new CodePrimitiveExpression(fields.ResourceKey));

            return exp;
        }

        public override object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context)
        {
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "ExternalResourceExpressionBuilder.ParseExpression({0}, {1}, {2})", expression, propertyType, context));

            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentException(String.Format(Thread.CurrentThread.CurrentUICulture, "Too few parameters: {0}. Must provide a resource assembly name, resource type and resource key in the format '[AssemblyName]|[ResourceType], ResourceKey'.", expression));
            }

            ExternalResourceExpressionFields fields = null;
            string classKey = null;
            string resourceKey = null;

            string[] expParams = expression.Split(new char[] { ',' });

            if (expParams.Length > 2)
            {
                throw new ArgumentException(String.Format(Thread.CurrentThread.CurrentUICulture, "Too many parameters: {0}. Must provide a resource assembly name, resource type and resouce key in the format '[AssemblyName]|[ResourceType], ResourceKey'.", expression));
            }
            if (expParams.Length == 1)
            {
                throw new ArgumentException(String.Format(Thread.CurrentThread.CurrentUICulture, "Too few parameters: {0}. Must provide a resource assembly name, resource type and resource key in the format '[AssemblyName]|[ResourceType], ResourceKey'.", expression));
            }
            else
            {
                classKey = expParams[0].Trim();
                resourceKey = expParams[1].Trim();
            }

            fields = new ExternalResourceExpressionFields(classKey, resourceKey);

            ExternalResourceExpressionBuilder.EnsureResourceProviderFactory();
            IResourceProvider rp = ExternalResourceExpressionBuilder.s_resourceProviderFactory.CreateGlobalResourceProvider(fields.ClassKey);

            object res = rp.GetObject(fields.ResourceKey, CultureInfo.InvariantCulture);

            if (res == null)
            {
                throw new ArgumentException(String.Format(Thread.CurrentThread.CurrentUICulture, "Resource not found: {0}", fields.ResourceKey));
            }

            return fields;
        }

        private static void EnsureResourceProviderFactory()
        {
            if (ExternalResourceExpressionBuilder.s_resourceProviderFactory == null)
            {
                ExternalResourceExpressionBuilder.s_resourceProviderFactory = new ExternalResourceProviderFactory();
            }
        }

        public override bool SupportsEvaluate
        {
            get
            {
                Debug.WriteLine("ExternalResourceExpressionBuilder.SupportsEvaluate");
                return true;
            }
        }
    }

    public class ExternalResourceExpressionFields
    {
        internal ExternalResourceExpressionFields(string classKey, string resourceKey)
        {
            this.m_classKey = classKey;
            this.m_resourceKey = resourceKey;
        }

        public string ClassKey
        {
            get
            {
                return this.m_classKey;
            }
        }

        public string ResourceKey
        {
            get
            {
                return this.m_resourceKey;
            }
        }

        private string m_classKey;
        private string m_resourceKey;
    }
}