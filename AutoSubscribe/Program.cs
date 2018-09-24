using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AutoSubscribe
{
    class Program
    {
        static void Main(string[] args)
        {
            Manage(args[0]);
        }

        private static void Useless()
        {
            Type.GetType("AttributeValidator.Initialisation, AttributeValidator").GetMethod("AutoSubscription").Invoke(null, new object[0]);
        }

        private static void Manage(string assembly)
        {
            var _resolver = new DefaultAssemblyResolver();
            var _directory = Path.GetDirectoryName(assembly);
            _resolver.AddSearchDirectory(_directory);
            var module = AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters() { AssemblyResolver = _resolver, ReadSymbols = true, ReadingMode = ReadingMode.Immediate }).MainModule;
            var mainModuleType = module.GetTypes().First(_Type => _Type.Name == "<Module>");
            var cctor = mainModuleType.Methods.SingleOrDefault(_Method => _Method.IsConstructor);
            if (cctor == null)
            {
                //Create the .cctor method in the Main Module
                var _cctor = new MethodDefinition(".cctor", Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, mainModuleType.Module.TypeSystem.Void);
                mainModuleType.Methods.Add(_cctor);

                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "AttributeValidator.Initialisation, Puresharp.AttributeValidator"));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, module.Import(typeof(Type).GetMethod("GetType", new Type[] { typeof(string) }))));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, "AutoInscription"));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, module.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string) }))));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldnull));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Newarr, module.Import(typeof(object))));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, module.Import(Metadata<MethodBase>.Method(_MethodBase => _MethodBase.Invoke(Metadata<object>.Value, Metadata<object[]>.Value)))));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Pop));
                _cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            }
            else
            {
                var index = cctor.Body.Instructions.Count - 1; // position à laquelle on peut insérer 
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Ldstr, "AttributeValidator.Initialisation, Puresharp.AttributeValidator"));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Call, module.Import(typeof(Type).GetMethod("GetType", new Type[] { typeof(string) }))));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Ldstr, "AutoInscription"));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string) }))));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Ldnull));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Ldc_I4_0));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Newarr, module.Import(typeof(object))));
                //cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[])}))));
                cctor.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Callvirt, module.Import(Metadata<MethodBase>.Method(_MethodBase => _MethodBase.Invoke(Metadata<object>.Value, Metadata<object[]>.Value)))));
                cctor.Body.Instructions.Insert(index, Instruction.Create(OpCodes.Pop));
            }
            module.Assembly.Write(assembly, new WriterParameters { WriteSymbols = true });
        }
    }
}
