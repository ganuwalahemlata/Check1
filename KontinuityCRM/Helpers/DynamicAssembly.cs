using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace KontinuityCRM.Helpers
{
    public class DynamicAssembly
    {
        /// <summary>
        /// Create DynamicAssembly
        /// </summary>
        public static void Create(/*Dictionary<string, string> dic*/)
        {
            // Get the current application domain for the current thread.
            AppDomain currentDomain = AppDomain.CurrentDomain;

            // Get the path for the assembly
            string path = HttpRuntime.AppDomainAppPath + "\\bin";

            // Create a dynamic assembly in the current application domain, and allow it to be executed and saved to disk.
            AssemblyName aName = new AssemblyName("EnumAssembly");
            AssemblyBuilder ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave, path);

            // Define a dynamic module in "TempAssembly" assembly. For a single-module assembly, the module has the same name as the assembly.
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");


            

            // Define a public enumeration with the name "Elevation" and an underlying type of Integer.
            Assembly enumAssembly = null;
            try
            {
                enumAssembly = System.Reflection.Assembly.Load("EnumAssembly");
            }
            catch { }

            DefineEnum("ViewPermissiond", mb, 
                Assembly.GetExecutingAssembly().GetType("KontinuityCRM.Controllers.BaseController"), enumAssembly);
            DefineEnum("ViewPermission", mb, Assembly.GetExecutingAssembly().GetType("KontinuityCRM.Controllers.Base1Controller"), enumAssembly);
            DefineEnum("ViewPermission2", mb, Assembly.GetExecutingAssembly().GetType("KontinuityCRM.Controllers.Base2Controller"), enumAssembly);
                        

            ab.Save(aName.Name + ".dll");
        }

        public class Permission
        {
            /// <summary>
            /// Indicates Name for Permission
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Indicates Value for Permission
            /// </summary>
            public long Value { get; set; }
        }
        /// <summary>
        /// Generic function to define Enum
        /// </summary>
        /// <param name="enumName">Enum Name</param>
        /// <param name="mb">module binder</param>
        /// <param name="basetype">base type</param>
        /// <param name="enumAssembly">enum assembbly</param>
        private static void DefineEnum(string enumName, ModuleBuilder mb, Type basetype, Assembly enumAssembly)
        {
            // Define a public enumeration with the name "Elevation" and an underlying type of Integer.
            EnumBuilder eb = mb.DefineEnum(enumName, TypeAttributes.Public, typeof(long));

            // Set the flag attribute
            eb.SetCustomAttribute(new CustomAttributeBuilder(typeof(FlagsAttribute).GetConstructor(Type.EmptyTypes), new object[] { }));

            var currentactions = GetSubClasses(basetype).SelectMany(t =>
                                   ActionNames(t).Select(a => a + t.Name.Substring(0, t.Name.Length - 10)));

            var pots = Enumerable.Range(0, 63); // 64

            // get the current assign values
            List<Permission> presentvalues;

            try
            {
                var enumtype = enumAssembly.GetType(enumName);

                presentvalues = Enum.GetNames(enumtype).Select(s => new Permission
                {
                    Name = s,
                    Value = (long)enumtype.GetField(s).GetRawConstantValue(),
                }).ToList();
            }
            catch 
            {
                presentvalues = new List<Permission>();
            }

            // see what needs to be deleted
            var deletedactions = presentvalues.Select(p => p.Name).Except(currentactions);

            foreach (var permission in presentvalues.Where(p => deletedactions.Contains(p.Name)).ToList()) // ToList because we modify the list 
            {
                presentvalues.Remove(permission);
            }

            // get the possible available values ordered
            var availablePots = pots.Select(p => Convert.ToInt64(Math.Pow(2, p))).Except(presentvalues.Select(p => p.Value));

            // see what needs to be added using the available values
            var newactions = currentactions.Except(presentvalues.Select(p => p.Name));

            if (newactions.Count() > availablePots.Count())
                throw new Exception(string.Format("There are too many permissions for the {0} enum", enumName));

            foreach (var permission in newactions.Zip(availablePots, (a, p) => new Permission { Name = a, Value = p }))
            {
                presentvalues.Add(permission);
            }

            foreach (var permission in presentvalues)
	        {
		        eb.DefineLiteral(permission.Name, permission.Value);
	        }

             // Create the type and save the assembly. 
            Type finished = eb.CreateType();

        }
        /// <summary>
        /// Get Sub Class Type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns></returns>
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
        /// <summary>
        /// Get Sub Class Type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        private static List<Type> GetSubClasses(Type type)
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                t => t.IsSubclassOf(type)).ToList();
        }
        /// <summary>
        /// Get Action Names
        /// </summary>
        /// <param name="controllerType">type</param>
        /// <returns></returns>
        private static List<string> ActionNames(Type controllerType)
        {
            //return new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Select(x => x.ActionName).ToList();

            //return new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().
            //    Where(
            //    x => !x.IsDefined(typeof(HttpPostAttribute), false) &&
            //    (((ReflectedActionDescriptor)x).MethodInfo.ReturnType == typeof(ActionResult) || ((ReflectedActionDescriptor)x).MethodInfo.ReturnType.IsSubclassOf(typeof(ActionResult)))
            //    ).Select(x => x.ActionName.ToLower()).ToList();

            ActionDescriptor[] adlist = new ReflectedControllerDescriptor(controllerType).GetCanonicalActions();
            List<string> rlist = new List<string>();
            foreach (var action in adlist)
            {

                // this is for skip actions with the httppostattribute
                if (action.IsDefined(typeof(HttpPostAttribute), false))
                    continue;
                ReflectedActionDescriptor rad = action as ReflectedActionDescriptor;
                if (rad != null)
                {
                    if (rad.MethodInfo.ReturnType == typeof(ActionResult) || 
                        rad.MethodInfo.ReturnType.IsSubclassOf(typeof(ActionResult)) ||
                        rad.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task<ActionResult>)))
                        rlist.Add(rad.ActionName.ToLower());
                }

            }

            return rlist;

        }
        /// <summary>
        /// Get Controller Names
        /// </summary>
        /// <returns></returns>
        private static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }
         
    }

    public static class DynamicType
    {
        /// <summary>
        /// Create Type Builder
        /// </summary>
        /// <param name="assemblyName">Assembly Name</param>
        /// <param name="moduleName">Module Name</param>
        /// <param name="typeName">Type</param>
        /// <returns></returns>
        public static TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }
        /// <summary>
        /// Create Auto Implemented Property
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="propertyName">property Name</param>
        /// <param name="propertyType">property Type</param>
        public static void CreateAutoImplementedProperty(
            TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }
    }
}