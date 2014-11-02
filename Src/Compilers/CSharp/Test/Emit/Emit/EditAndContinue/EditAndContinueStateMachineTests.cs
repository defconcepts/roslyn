﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.CSharp.UnitTests;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.PdbUtilities;
using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.EditAndContinue.UnitTests
{
    public class EditAndContinueStateMachineTests : EditAndContinueTestBase
    {
        [Fact]
        public void AddIteratorMethod()
        {
            var source0 =
@"using System.Collections.Generic;
class C
{
    static IEnumerable<object> F()
    {
        yield return 0;
    }
    static void M()
    {
    }
}";
            var source1 =
@"using System.Collections.Generic;
class C
{
    static IEnumerable<object> F()
    {
        yield return 0;
    }
    static IEnumerable<int> G()
    {
        yield return 1;
    }
    static void M()
    {
    }
}";
            var compilation0 = CreateCompilationWithMscorlib(Parse(source0, "a.cs"), options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib(Parse(source1, "a.cs"), options: TestOptions.DebugDll);

            var bytes0 = compilation0.EmitToArray();
            var generation0 = EmitBaseline.CreateInitialBaseline(ModuleMetadata.CreateFromImage(bytes0), EmptyLocalsProvider);
            var diff1 = compilation1.EmitDifference(
                generation0,
                ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Insert, null, compilation1.GetMember<MethodSymbol>("C.G"))));

            using (var md1 = diff1.GetMetadata())
            {
                var reader1 = md1.Reader;

                CheckEncLog(reader1,
                    Row(2, TableIndex.AssemblyRef, EditAndContinueOperation.Default),
                    Row(17, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(18, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(19, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(20, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(21, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(22, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(23, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(24, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(25, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(26, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(27, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(28, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(29, TableIndex.MemberRef, EditAndContinueOperation.Default),
                    Row(16, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(17, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(18, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(19, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(20, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(21, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(22, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(23, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(24, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(25, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(26, TableIndex.TypeRef, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeSpec, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeSpec, EditAndContinueOperation.Default),
                    Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                    Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                    Row(6, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.Default),
                    Row(2, TableIndex.PropertyMap, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                    Row(4, TableIndex.Field, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                    Row(5, TableIndex.Field, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                    Row(6, TableIndex.Field, EditAndContinueOperation.Default),
                    Row(2, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(12, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(13, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(14, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(15, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(16, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(17, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(18, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(19, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(4, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(20, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(2, TableIndex.PropertyMap, EditAndContinueOperation.AddProperty),
                    Row(3, TableIndex.Property, EditAndContinueOperation.Default),
                    Row(2, TableIndex.PropertyMap, EditAndContinueOperation.AddProperty),
                    Row(4, TableIndex.Property, EditAndContinueOperation.Default),
                    Row(13, TableIndex.MethodDef, EditAndContinueOperation.AddParameter),
                    Row(2, TableIndex.Param, EditAndContinueOperation.Default),
                    Row(12, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(15, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(16, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(17, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(18, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(19, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(3, TableIndex.MethodSemantics, EditAndContinueOperation.Default),
                    Row(4, TableIndex.MethodSemantics, EditAndContinueOperation.Default),
                    Row(8, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(9, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(10, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(11, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(12, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(13, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(14, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(2, TableIndex.NestedClass, EditAndContinueOperation.Default),
                    Row(6, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                    Row(7, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                    Row(8, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                    Row(9, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                    Row(10, TableIndex.InterfaceImpl, EditAndContinueOperation.Default));
                CheckEncMap(reader1,
                    Handle(16, TableIndex.TypeRef),
                    Handle(17, TableIndex.TypeRef),
                    Handle(18, TableIndex.TypeRef),
                    Handle(19, TableIndex.TypeRef),
                    Handle(20, TableIndex.TypeRef),
                    Handle(21, TableIndex.TypeRef),
                    Handle(22, TableIndex.TypeRef),
                    Handle(23, TableIndex.TypeRef),
                    Handle(24, TableIndex.TypeRef),
                    Handle(25, TableIndex.TypeRef),
                    Handle(26, TableIndex.TypeRef),
                    Handle(4, TableIndex.TypeDef),
                    Handle(4, TableIndex.Field),
                    Handle(5, TableIndex.Field),
                    Handle(6, TableIndex.Field),
                    Handle(12, TableIndex.MethodDef),
                    Handle(13, TableIndex.MethodDef),
                    Handle(14, TableIndex.MethodDef),
                    Handle(15, TableIndex.MethodDef),
                    Handle(16, TableIndex.MethodDef),
                    Handle(17, TableIndex.MethodDef),
                    Handle(18, TableIndex.MethodDef),
                    Handle(19, TableIndex.MethodDef),
                    Handle(20, TableIndex.MethodDef),
                    Handle(2, TableIndex.Param),
                    Handle(6, TableIndex.InterfaceImpl),
                    Handle(7, TableIndex.InterfaceImpl),
                    Handle(8, TableIndex.InterfaceImpl),
                    Handle(9, TableIndex.InterfaceImpl),
                    Handle(10, TableIndex.InterfaceImpl),
                    Handle(17, TableIndex.MemberRef),
                    Handle(18, TableIndex.MemberRef),
                    Handle(19, TableIndex.MemberRef),
                    Handle(20, TableIndex.MemberRef),
                    Handle(21, TableIndex.MemberRef),
                    Handle(22, TableIndex.MemberRef),
                    Handle(23, TableIndex.MemberRef),
                    Handle(24, TableIndex.MemberRef),
                    Handle(25, TableIndex.MemberRef),
                    Handle(26, TableIndex.MemberRef),
                    Handle(27, TableIndex.MemberRef),
                    Handle(28, TableIndex.MemberRef),
                    Handle(29, TableIndex.MemberRef),
                    Handle(12, TableIndex.CustomAttribute),
                    Handle(13, TableIndex.CustomAttribute),
                    Handle(14, TableIndex.CustomAttribute),
                    Handle(15, TableIndex.CustomAttribute),
                    Handle(16, TableIndex.CustomAttribute),
                    Handle(17, TableIndex.CustomAttribute),
                    Handle(18, TableIndex.CustomAttribute),
                    Handle(19, TableIndex.CustomAttribute),
                    Handle(4, TableIndex.StandAloneSig),
                    Handle(5, TableIndex.StandAloneSig),
                    Handle(6, TableIndex.StandAloneSig),
                    Handle(2, TableIndex.PropertyMap),
                    Handle(3, TableIndex.Property),
                    Handle(4, TableIndex.Property),
                    Handle(3, TableIndex.MethodSemantics),
                    Handle(4, TableIndex.MethodSemantics),
                    Handle(8, TableIndex.MethodImpl),
                    Handle(9, TableIndex.MethodImpl),
                    Handle(10, TableIndex.MethodImpl),
                    Handle(11, TableIndex.MethodImpl),
                    Handle(12, TableIndex.MethodImpl),
                    Handle(13, TableIndex.MethodImpl),
                    Handle(14, TableIndex.MethodImpl),
                    Handle(3, TableIndex.TypeSpec),
                    Handle(4, TableIndex.TypeSpec),
                    Handle(2, TableIndex.AssemblyRef),
                    Handle(2, TableIndex.NestedClass));
            }

            string actualPdb1 = PdbToXmlConverter.DeltaPdbToXml(diff1.PdbDelta, Enumerable.Range(1, 100).Select(rid => 0x06000000U | (uint)rid));

            // TODO (tomat): bug in SymWriter.
            // The PDB is missing debug info for G method. The info is written to the PDB but the native SymWriter 
            // seems to ignore it. If another method is added to the class all information is written. 
            // This happens regardless of whether we emit just the delta or full PDB.

            string expectedPdb1 = @"
<symbols>
  <files>
    <file id=""1"" name=""a.cs"" language=""3f5162f8-07c6-11d3-9053-00c04fa302a1"" languageVendor=""994b45c4-e6e9-11d2-903f-00c04fa302a1"" documentType=""5a869d0b-6611-11d3-bd2a-0000f80849bd"" checkSumAlgorithmId=""ff1816ec-aa5e-4d10-87f7-6f4963833460"" checkSum=""6E, 19, 36, 2B, 9A, 28, AB, E3, A2, DA, EB, 51, C1, 37,  1, 10, B0, 4F, CA, 84, "" />
  </files>
  <methods>
    <method token=""0x600000f"">
      <customDebugInfo version=""4"" count=""1"">
        <using version=""4"" kind=""UsingInfo"" size=""12"" namespaceCount=""1"">
          <namespace usingCount=""1"" />
        </using>
      </customDebugInfo>
      <sequencepoints total=""5"">
        <entry il_offset=""0x0"" hidden=""true"" start_row=""16707566"" start_column=""0"" end_row=""16707566"" end_column=""0"" file_ref=""1"" />
        <entry il_offset=""0x21"" start_row=""9"" start_column=""5"" end_row=""9"" end_column=""6"" file_ref=""1"" />
        <entry il_offset=""0x22"" start_row=""10"" start_column=""9"" end_row=""10"" end_column=""24"" file_ref=""1"" />
        <entry il_offset=""0x34"" hidden=""true"" start_row=""16707566"" start_column=""0"" end_row=""16707566"" end_column=""0"" file_ref=""1"" />
        <entry il_offset=""0x3b"" start_row=""11"" start_column=""5"" end_row=""11"" end_column=""6"" file_ref=""1"" />
      </sequencepoints>
      <locals />
      <scope startOffset=""0x0"" endOffset=""0x3f"">
        <namespace name=""System.Collections.Generic"" />
      </scope>
    </method>
  </methods>
</symbols>";

            AssertXmlEqual(expectedPdb1, actualPdb1);
        }

        [Fact]
        public void AddAsyncMethod()
        {
            var source0 = @"
using System.Threading.Tasks;

class C
{
}";
            var source1 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F() 
    {
        await Task.FromResult(10);
        return 20;
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);
            var v0 = CompileAndVerify(compilation0);

            var generation0 = EmitBaseline.CreateInitialBaseline(ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData), EmptyLocalsProvider);
            var diff1 = compilation1.EmitDifference(
                generation0,
                ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Insert, null, compilation1.GetMember<MethodSymbol>("C.F"))));

            using (var md1 = diff1.GetMetadata())
            {
                var reader1 = md1.Reader;

                // Add state machine type and its members:
                // - Method '.ctor'
                // - Method 'MoveNext'
                // - Method 'SetStateMachine'
                // - Field '<>1__state'
                // - Field '<>t__builder'
                // - Field '<>u__0'
                // Add method F()
                CheckEncLogDefinitions(reader1,
                    Row(1, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                    Row(2, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                    Row(1, TableIndex.Field, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                    Row(2, TableIndex.Field, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                    Row(3, TableIndex.Field, EditAndContinueOperation.Default),
                    Row(2, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(2, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(3, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(4, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                    Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                    Row(5, TableIndex.MethodDef, EditAndContinueOperation.AddParameter),
                    Row(1, TableIndex.Param, EditAndContinueOperation.Default),
                    Row(4, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(5, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(6, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                    Row(1, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(2, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                    Row(1, TableIndex.NestedClass, EditAndContinueOperation.Default),
                    Row(1, TableIndex.InterfaceImpl, EditAndContinueOperation.Default));
            }
        }

        [Fact]
        public void MethodToIteratorMethod()
        {
            var source0 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        return new int[] { 1, 2, 3 };
    }
}";
            var source1 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        yield return 2;
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1)));

                using (var md1 = diff1.GetMetadata())
                {
                    CheckEncLogDefinitions(md1.Reader,
                        Row(2, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.Default),
                        Row(1, TableIndex.PropertyMap, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(2, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(3, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(4, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(3, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(4, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(6, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(7, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(8, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(9, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(10, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(1, TableIndex.PropertyMap, EditAndContinueOperation.AddProperty),
                        Row(1, TableIndex.Property, EditAndContinueOperation.Default),
                        Row(1, TableIndex.PropertyMap, EditAndContinueOperation.AddProperty),
                        Row(2, TableIndex.Property, EditAndContinueOperation.Default),
                        Row(3, TableIndex.MethodDef, EditAndContinueOperation.AddParameter),
                        Row(1, TableIndex.Param, EditAndContinueOperation.Default),
                        Row(5, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(6, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(7, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(8, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(9, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(10, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(11, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(12, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodSemantics, EditAndContinueOperation.Default),
                        Row(2, TableIndex.MethodSemantics, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(2, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(3, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(4, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(6, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(7, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(2, TableIndex.NestedClass, EditAndContinueOperation.Default),
                        Row(1, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                        Row(2, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                        Row(3, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                        Row(4, TableIndex.InterfaceImpl, EditAndContinueOperation.Default),
                        Row(5, TableIndex.InterfaceImpl, EditAndContinueOperation.Default));
                }
            }
        }

        [Fact]
        public void MethodToAsyncMethod()
        {
            var source0 = @"
using System.Threading.Tasks;

class C
{
    static Task<int> F() 
    {
        return Task.FromResult(1);
    }
}";
            var source1 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F() 
    {
        return await Task.FromResult(1);
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1)));

                using (var md1 = diff1.GetMetadata())
                {
                    CheckEncLogDefinitions(md1.Reader,
                        Row(2, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(1, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(2, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(3, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(4, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(3, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(4, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddMethod),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.AddParameter),
                        Row(1, TableIndex.Param, EditAndContinueOperation.Default),
                        Row(4, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(5, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(6, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(2, TableIndex.MethodImpl, EditAndContinueOperation.Default),
                        Row(1, TableIndex.NestedClass, EditAndContinueOperation.Default),
                        Row(1, TableIndex.InterfaceImpl, EditAndContinueOperation.Default));
                }
            }
        }

        [Fact]
        public void IteratorMethodToMethod()
        {
            var source0 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        yield return 2;
    }
}";
            var source1 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        return new int[] { 1, 2, 3 };
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1)));

                using (var md1 = diff1.GetMetadata())
                {
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default));
                }
            }
        }

        [Fact]
        public void AsyncMethodToMethod()
        {
            var source0 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F() 
    {
        return await Task.FromResult(1);
    }
}";
            var source1 = @"
using System.Threading.Tasks;

class C
{
    static Task<int> F() 
    {
        return Task.FromResult(1);
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1)));

                using (var md1 = diff1.GetMetadata())
                {
                    CheckEncLogDefinitions(md1.Reader,
                        Row(3, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default));
                }
            }
        }

        [Fact]
        public void AsyncMethodOverloads()
        {
            var source0 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F(long a) 
    {
        return await Task.FromResult(1);
    }

    static async Task<int> F(int a) 
    {
        return await Task.FromResult(1);
    }

    static async Task<int> F(short a) 
    {
        return await Task.FromResult(1);
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var source1 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F(short a) 
    {
        return await Task.FromResult(2);
    }

    static async Task<int> F(long a) 
    {
        return await Task.FromResult(3);
    }

    static async Task<int> F(int a) 
    {
        return await Task.FromResult(4);
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var methodShort0 = compilation0.GetMembers("C.F").Single(m => m.ToTestDisplayString() == "System.Threading.Tasks.Task<System.Int32> C.F(System.Int16 a)");
                var methodShort1 = compilation1.GetMembers("C.F").Single(m => m.ToTestDisplayString() == "System.Threading.Tasks.Task<System.Int32> C.F(System.Int16 a)");

                var methodInt0 = compilation0.GetMembers("C.F").Single(m => m.ToTestDisplayString() == "System.Threading.Tasks.Task<System.Int32> C.F(System.Int32 a)");
                var methodInt1 = compilation1.GetMembers("C.F").Single(m => m.ToTestDisplayString() == "System.Threading.Tasks.Task<System.Int32> C.F(System.Int32 a)");

                var methodLong0 = compilation0.GetMembers("C.F").Single(m => m.ToTestDisplayString() == "System.Threading.Tasks.Task<System.Int32> C.F(System.Int64 a)");
                var methodLong1 = compilation1.GetMembers("C.F").Single(m => m.ToTestDisplayString() == "System.Threading.Tasks.Task<System.Int32> C.F(System.Int64 a)");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(
                        new SemanticEdit(SemanticEditKind.Update, methodShort0, methodShort1, preserveLocalVariables: true),
                        new SemanticEdit(SemanticEditKind.Update, methodInt0, methodInt1, preserveLocalVariables: true),
                        new SemanticEdit(SemanticEditKind.Update, methodLong0, methodLong1, preserveLocalVariables: true)
                    ));

                using (var md1 = diff1.GetMetadata())
                {
                    // notice no TypeDefs, FieldDefs
                    CheckEncLogDefinitions(md1.Reader,
                        Row(7, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(8, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(9, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(10, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(11, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(12, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(2, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(3, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(7, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(10, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(15, TableIndex.CustomAttribute, EditAndContinueOperation.Default));
                }
            }
        }

        [Fact]
        public void UpdateIterator_NoVariables()
        {
            var source0 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        yield return 1;
    }
}";
            var source1 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        yield return 2;
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1)));

                // Verify delta metadata contains expected rows.
                using (var md1 = diff1.GetMetadata())
                {
                    // Verify that no new TypeDefs, FieldDefs or MethodDefs were added,
                    // 3 methods were updated: 
                    // - the kick-off method (might be changed if the method previously wasn't an iterator)
                    // - Finally method
                    // - MoveNext method
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(4, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size       63 (0x3f)
  .maxstack  2
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_0034
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  ldarg.0
  IL_0023:  ldc.i4.2
  IL_0024:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0029:  ldarg.0
  IL_002a:  ldc.i4.1
  IL_002b:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0030:  ldc.i4.1
  IL_0031:  stloc.1
  IL_0032:  br.s       IL_0018
  IL_0034:  ldarg.0
  IL_0035:  ldc.i4.m1
  IL_0036:  stfld      ""int C.<F>d__1.<>1__state""
  IL_003b:  ldc.i4.0
  IL_003c:  stloc.1
  IL_003d:  br.s       IL_0018
}
");
                    v0.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size       63 (0x3f)
  .maxstack  2
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_0034
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  ldarg.0
  IL_0023:  ldc.i4.1
  IL_0024:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0029:  ldarg.0
  IL_002a:  ldc.i4.1
  IL_002b:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0030:  ldc.i4.1
  IL_0031:  stloc.1
  IL_0032:  br.s       IL_0018
  IL_0034:  ldarg.0
  IL_0035:  ldc.i4.m1
  IL_0036:  stfld      ""int C.<F>d__1.<>1__state""
  IL_003b:  ldc.i4.0
  IL_003c:  stloc.1
  IL_003d:  br.s       IL_0018
}");
                }
            }
        }

        [Fact]
        public void UpdateAsync_NoVariables()
        {
            var source0 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F() 
    {
        await Task.FromResult(1);
        return 2;
    }
}";
            var source1 = @"
using System.Threading.Tasks;

class C
{
    static async Task<int> F() 
    {
        await Task.FromResult(10);
        return 20;
    }
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: TestOptions.DebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: TestOptions.DebugDll);

            var v0 = CompileAndVerify(compilation0);

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, EmptyLocalsProvider);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1)));

                using (var md1 = diff1.GetMetadata())
                {
                    // Verify that no new TypeDefs, FieldDefs or MethodDefs were added,
                    // 2 methods were updated: 
                    // - the kick-off method (might be changed if the method previously wasn't async)
                    // - MoveNext method
                    CheckEncLogDefinitions(md1.Reader,
                        Row(3, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(4, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(7, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext", @"
{
  // Code size      171 (0xab)
  .maxstack  3
  .locals init (int V_0,
                int V_1,
                System.Runtime.CompilerServices.TaskAwaiter<int> V_2,
                C.<F>d__1 V_3,
                System.Exception V_4)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  .try
  {
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_000c
    IL_000a:  br.s       IL_000e
    IL_000c:  br.s       IL_0049
    IL_000e:  nop
    IL_000f:  ldc.i4.s   10
    IL_0011:  call       ""System.Threading.Tasks.Task<int> System.Threading.Tasks.Task.FromResult<int>(int)""
    IL_0016:  callvirt   ""System.Runtime.CompilerServices.TaskAwaiter<int> System.Threading.Tasks.Task<int>.GetAwaiter()""
    IL_001b:  stloc.2
    IL_001c:  ldloca.s   V_2
    IL_001e:  call       ""bool System.Runtime.CompilerServices.TaskAwaiter<int>.IsCompleted.get""
    IL_0023:  brtrue.s   IL_0065
    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.0
    IL_0027:  dup
    IL_0028:  stloc.0
    IL_0029:  stfld      ""int C.<F>d__1.<>1__state""
    IL_002e:  ldarg.0
    IL_002f:  ldloc.2
    IL_0030:  stfld      ""System.Runtime.CompilerServices.TaskAwaiter<int> C.<F>d__1.<>u__0""
    IL_0035:  ldarg.0
    IL_0036:  stloc.3
    IL_0037:  ldarg.0
    IL_0038:  ldflda     ""System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> C.<F>d__1.<>t__builder""
    IL_003d:  ldloca.s   V_2
    IL_003f:  ldloca.s   V_3
    IL_0041:  call       ""void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>, C.<F>d__1>(ref System.Runtime.CompilerServices.TaskAwaiter<int>, ref C.<F>d__1)""
    IL_0046:  nop
    IL_0047:  leave.s    IL_00aa
    IL_0049:  ldarg.0
    IL_004a:  ldfld      ""System.Runtime.CompilerServices.TaskAwaiter<int> C.<F>d__1.<>u__0""
    IL_004f:  stloc.2
    IL_0050:  ldarg.0
    IL_0051:  ldflda     ""System.Runtime.CompilerServices.TaskAwaiter<int> C.<F>d__1.<>u__0""
    IL_0056:  initobj    ""System.Runtime.CompilerServices.TaskAwaiter<int>""
    IL_005c:  ldarg.0
    IL_005d:  ldc.i4.m1
    IL_005e:  dup
    IL_005f:  stloc.0
    IL_0060:  stfld      ""int C.<F>d__1.<>1__state""
    IL_0065:  ldloca.s   V_2
    IL_0067:  call       ""int System.Runtime.CompilerServices.TaskAwaiter<int>.GetResult()""
    IL_006c:  pop
    IL_006d:  ldloca.s   V_2
    IL_006f:  initobj    ""System.Runtime.CompilerServices.TaskAwaiter<int>""
    IL_0075:  ldc.i4.s   20
    IL_0077:  stloc.1
    IL_0078:  leave.s    IL_0095
  }
  catch System.Exception
  {
    IL_007a:  stloc.s    V_4
    IL_007c:  nop
    IL_007d:  ldarg.0
    IL_007e:  ldc.i4.s   -2
    IL_0080:  stfld      ""int C.<F>d__1.<>1__state""
    IL_0085:  ldarg.0
    IL_0086:  ldflda     ""System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> C.<F>d__1.<>t__builder""
    IL_008b:  ldloc.s    V_4
    IL_008d:  call       ""void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.SetException(System.Exception)""
    IL_0092:  nop
    IL_0093:  leave.s    IL_00aa
  }
  IL_0095:  ldarg.0
  IL_0096:  ldc.i4.s   -2
  IL_0098:  stfld      ""int C.<F>d__1.<>1__state""
  IL_009d:  ldarg.0
  IL_009e:  ldflda     ""System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> C.<F>d__1.<>t__builder""
  IL_00a3:  ldloc.1
  IL_00a4:  call       ""void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.SetResult(int)""
  IL_00a9:  nop
  IL_00aa:  ret
}
");
                    v0.VerifyIL("C.<F>d__1.System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext", @"
{
  // Code size      169 (0xa9)
  .maxstack  3
  .locals init (int V_0,
                int V_1,
                System.Runtime.CompilerServices.TaskAwaiter<int> V_2,
                C.<F>d__1 V_3,
                System.Exception V_4)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  .try
  {
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_000c
    IL_000a:  br.s       IL_000e
    IL_000c:  br.s       IL_0048
    IL_000e:  nop
    IL_000f:  ldc.i4.1
    IL_0010:  call       ""System.Threading.Tasks.Task<int> System.Threading.Tasks.Task.FromResult<int>(int)""
    IL_0015:  callvirt   ""System.Runtime.CompilerServices.TaskAwaiter<int> System.Threading.Tasks.Task<int>.GetAwaiter()""
    IL_001a:  stloc.2
    IL_001b:  ldloca.s   V_2
    IL_001d:  call       ""bool System.Runtime.CompilerServices.TaskAwaiter<int>.IsCompleted.get""
    IL_0022:  brtrue.s   IL_0064
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.0
    IL_0026:  dup
    IL_0027:  stloc.0
    IL_0028:  stfld      ""int C.<F>d__1.<>1__state""
    IL_002d:  ldarg.0
    IL_002e:  ldloc.2
    IL_002f:  stfld      ""System.Runtime.CompilerServices.TaskAwaiter<int> C.<F>d__1.<>u__0""
    IL_0034:  ldarg.0
    IL_0035:  stloc.3
    IL_0036:  ldarg.0
    IL_0037:  ldflda     ""System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> C.<F>d__1.<>t__builder""
    IL_003c:  ldloca.s   V_2
    IL_003e:  ldloca.s   V_3
    IL_0040:  call       ""void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>, C.<F>d__1>(ref System.Runtime.CompilerServices.TaskAwaiter<int>, ref C.<F>d__1)""
    IL_0045:  nop
    IL_0046:  leave.s    IL_00a8
    IL_0048:  ldarg.0
    IL_0049:  ldfld      ""System.Runtime.CompilerServices.TaskAwaiter<int> C.<F>d__1.<>u__0""
    IL_004e:  stloc.2
    IL_004f:  ldarg.0
    IL_0050:  ldflda     ""System.Runtime.CompilerServices.TaskAwaiter<int> C.<F>d__1.<>u__0""
    IL_0055:  initobj    ""System.Runtime.CompilerServices.TaskAwaiter<int>""
    IL_005b:  ldarg.0
    IL_005c:  ldc.i4.m1
    IL_005d:  dup
    IL_005e:  stloc.0
    IL_005f:  stfld      ""int C.<F>d__1.<>1__state""
    IL_0064:  ldloca.s   V_2
    IL_0066:  call       ""int System.Runtime.CompilerServices.TaskAwaiter<int>.GetResult()""
    IL_006b:  pop
    IL_006c:  ldloca.s   V_2
    IL_006e:  initobj    ""System.Runtime.CompilerServices.TaskAwaiter<int>""
    IL_0074:  ldc.i4.2
    IL_0075:  stloc.1
    IL_0076:  leave.s    IL_0093
  }
  catch System.Exception
  {
    IL_0078:  stloc.s    V_4
    IL_007a:  nop
    IL_007b:  ldarg.0
    IL_007c:  ldc.i4.s   -2
    IL_007e:  stfld      ""int C.<F>d__1.<>1__state""
    IL_0083:  ldarg.0
    IL_0084:  ldflda     ""System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> C.<F>d__1.<>t__builder""
    IL_0089:  ldloc.s    V_4
    IL_008b:  call       ""void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.SetException(System.Exception)""
    IL_0090:  nop
    IL_0091:  leave.s    IL_00a8
  }
  IL_0093:  ldarg.0
  IL_0094:  ldc.i4.s   -2
  IL_0096:  stfld      ""int C.<F>d__1.<>1__state""
  IL_009b:  ldarg.0
  IL_009c:  ldflda     ""System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int> C.<F>d__1.<>t__builder""
  IL_00a1:  ldloc.1
  IL_00a2:  call       ""void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>.SetResult(int)""
  IL_00a7:  nop
  IL_00a8:  ret
}
");
                }
            }
        }

        [Fact]
        public void UpdateIterator_UserDefinedVariables_NoChange()
        {
            var source0 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F(int p) 
    {
        int x = p;
        yield return 1;
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var source1 = @"
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F(int p) 
    {
        int x = p;
        yield return 2;
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: ComSafeDebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: ComSafeDebugDll);

            var v0 = CompileAndVerify(compilation0);
            var debugInfoProvider = v0.CreatePdbInfoProvider();

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, debugInfoProvider.GetEncMethodDebugInfo);

                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1, GetEquivalentNodesMap(method1, method0), preserveLocalVariables: true)));

                // Verify delta metadata contains expected rows.
                using (var md1 = diff1.GetMetadata())
                {
                    // Verify that no new TypeDefs, FieldDefs or MethodDefs were added,
                    // 3 methods were updated: 
                    // - the kick-off method (might be changed if the method previously wasn't an iterator)
                    // - Finally method
                    // - MoveNext method
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(6, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size       75 (0x4b)
  .maxstack  2
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_0040
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  ldarg.0
  IL_0023:  ldarg.0
  IL_0024:  ldfld      ""int C.<F>d__1.p""
  IL_0029:  stfld      ""int C.<F>d__1.<x>5__1""
  IL_002e:  ldarg.0
  IL_002f:  ldc.i4.2
  IL_0030:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0035:  ldarg.0
  IL_0036:  ldc.i4.1
  IL_0037:  stfld      ""int C.<F>d__1.<>1__state""
  IL_003c:  ldc.i4.1
  IL_003d:  stloc.1
  IL_003e:  br.s       IL_0018
  IL_0040:  ldarg.0
  IL_0041:  ldc.i4.m1
  IL_0042:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0047:  ldc.i4.0
  IL_0048:  stloc.1
  IL_0049:  br.s       IL_0018
}
");
                }
            }
        }

        [Fact]
        public void UpdateIterator_UserDefinedVariables_AddVariable()
        {
            var source0 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F(int p) 
    {
        int x = p;
        yield return x;
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var source1 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F(int p) 
    {
        int y = 1234;
        int x = p;
        yield return y;
        Console.WriteLine(x);
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: ComSafeDebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: ComSafeDebugDll);

            var v0 = CompileAndVerify(compilation0);
            var debugInfoProvider = v0.CreatePdbInfoProvider();

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, debugInfoProvider.GetEncMethodDebugInfo);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1, GetEquivalentNodesMap(method1, method0), preserveLocalVariables: true)));

                // Verify delta metadata contains expected rows.
                using (var md1 = diff1.GetMetadata())
                {
                    // 1 field def added & 3 methods updated
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(7, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(6, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size      103 (0x67)
  .maxstack  2
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_0050
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  ldarg.0
  IL_0023:  ldc.i4     0x4d2
  IL_0028:  stfld      ""int C.<F>d__1.<y>5__2""
  IL_002d:  ldarg.0
  IL_002e:  ldarg.0
  IL_002f:  ldfld      ""int C.<F>d__1.p""
  IL_0034:  stfld      ""int C.<F>d__1.<x>5__1""
  IL_0039:  ldarg.0
  IL_003a:  ldarg.0
  IL_003b:  ldfld      ""int C.<F>d__1.<y>5__2""
  IL_0040:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0045:  ldarg.0
  IL_0046:  ldc.i4.1
  IL_0047:  stfld      ""int C.<F>d__1.<>1__state""
  IL_004c:  ldc.i4.1
  IL_004d:  stloc.1
  IL_004e:  br.s       IL_0018
  IL_0050:  ldarg.0
  IL_0051:  ldc.i4.m1
  IL_0052:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0057:  ldarg.0
  IL_0058:  ldfld      ""int C.<F>d__1.<x>5__1""
  IL_005d:  call       ""void System.Console.WriteLine(int)""
  IL_0062:  nop
  IL_0063:  ldc.i4.0
  IL_0064:  stloc.1
  IL_0065:  br.s       IL_0018
}
");
                }
            }
        }

        [Fact]
        public void UpdateIterator_UserDefinedVariables_AddAndRemoveVariable()
        {
            var source0 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F(int p) 
    {
        int x = p;
        yield return x;
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var source1 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F(int p) 
    {
        int y = 1234;
        yield return y;
        Console.WriteLine(p);
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: ComSafeDebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: ComSafeDebugDll);

            var v0 = CompileAndVerify(compilation0);
            var debugInfoProvider = v0.CreatePdbInfoProvider();

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, debugInfoProvider.GetEncMethodDebugInfo);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1, GetEquivalentNodesMap(method1, method0), preserveLocalVariables: true)));

                // Verify delta metadata contains expected rows.
                using (var md1 = diff1.GetMetadata())
                {
                    // 1 field def added & 3 methods updated
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(7, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(6, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size       91 (0x5b)
  .maxstack  2
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_0044
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  ldarg.0
  IL_0023:  ldc.i4     0x4d2
  IL_0028:  stfld      ""int C.<F>d__1.<y>5__2""
  IL_002d:  ldarg.0
  IL_002e:  ldarg.0
  IL_002f:  ldfld      ""int C.<F>d__1.<y>5__2""
  IL_0034:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0039:  ldarg.0
  IL_003a:  ldc.i4.1
  IL_003b:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0040:  ldc.i4.1
  IL_0041:  stloc.1
  IL_0042:  br.s       IL_0018
  IL_0044:  ldarg.0
  IL_0045:  ldc.i4.m1
  IL_0046:  stfld      ""int C.<F>d__1.<>1__state""
  IL_004b:  ldarg.0
  IL_004c:  ldfld      ""int C.<F>d__1.p""
  IL_0051:  call       ""void System.Console.WriteLine(int)""
  IL_0056:  nop
  IL_0057:  ldc.i4.0
  IL_0058:  stloc.1
  IL_0059:  br.s       IL_0018
}
");
                }
            }
        }

        [Fact]
        public void UpdateIterator_UserDefinedVariables_ChangeVariableType()
        {
            var source0 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        var x = 1;
        yield return 1;
        Console.WriteLine(x);
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var source1 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        var x = 1.0;
        yield return 2;
        Console.WriteLine(x);
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var compilation0 = CreateCompilationWithMscorlib45(source0, options: ComSafeDebugDll);
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: ComSafeDebugDll);

            var v0 = CompileAndVerify(compilation0);
            var debugInfoProvider = v0.CreatePdbInfoProvider();

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, debugInfoProvider.GetEncMethodDebugInfo);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1, GetEquivalentNodesMap(method1, method0), preserveLocalVariables: true)));

                // Verify delta metadata contains expected rows.
                using (var md1 = diff1.GetMetadata())
                {
                    // 1 field def added & 3 methods updated
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(5, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(6, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size       90 (0x5a)
  .maxstack  2
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_0043
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  ldarg.0
  IL_0023:  ldc.r8     1
  IL_002c:  stfld      ""double C.<F>d__1.<x>5__2""
  IL_0031:  ldarg.0
  IL_0032:  ldc.i4.2
  IL_0033:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0038:  ldarg.0
  IL_0039:  ldc.i4.1
  IL_003a:  stfld      ""int C.<F>d__1.<>1__state""
  IL_003f:  ldc.i4.1
  IL_0040:  stloc.1
  IL_0041:  br.s       IL_0018
  IL_0043:  ldarg.0
  IL_0044:  ldc.i4.m1
  IL_0045:  stfld      ""int C.<F>d__1.<>1__state""
  IL_004a:  ldarg.0
  IL_004b:  ldfld      ""double C.<F>d__1.<x>5__2""
  IL_0050:  call       ""void System.Console.WriteLine(double)""
  IL_0055:  nop
  IL_0056:  ldc.i4.0
  IL_0057:  stloc.1
  IL_0058:  br.s       IL_0018
}
");
                }
            }
        }

        [Fact]
        public void UpdateIterator_SynthesizedVariables_ChangeVariableType()
        {
            var source0 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        foreach (object item in new[] { 1 }) { yield return 1; }
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            var source1 = @"
using System;
using System.Collections.Generic;

class C
{
    static IEnumerable<int> F() 
    {
        foreach (object item in new[] { 1.0 }) { yield return 1; }
    }

    public void X() { } // needs to be present to work around SymWriter bug #1068894
}";
            // Rude edit but the compiler should handle it.

            var compilation0 = CreateCompilationWithMscorlib45(source0, options: ComSafeDebugDll.WithMetadataImportOptions(MetadataImportOptions.All));
            var compilation1 = CreateCompilationWithMscorlib45(source1, options: ComSafeDebugDll.WithMetadataImportOptions(MetadataImportOptions.All));

            var v0 = CompileAndVerify(compilation0, symbolValidator: module =>
            {
                Assert.Equal(new[]
                {
                    "<>1__state: int",
                    "<>2__current: int",
                    "<>l__initialThreadId: int",
                    "<>s__1: int[]",
                    "<>s__2: int",
                    "<item>5__3: object"
                }, module.GetFieldNamesAndTypes("C.<F>d__1"));
            });
            
            var debugInfoProvider = v0.CreatePdbInfoProvider();

            using (var md0 = ModuleMetadata.CreateFromImage(v0.EmittedAssemblyData))
            {
                var method0 = compilation0.GetMember<MethodSymbol>("C.F");
                var method1 = compilation1.GetMember<MethodSymbol>("C.F");

                var generation0 = EmitBaseline.CreateInitialBaseline(md0, debugInfoProvider.GetEncMethodDebugInfo);
                var diff1 = compilation1.EmitDifference(
                    generation0,
                    ImmutableArray.Create(new SemanticEdit(SemanticEditKind.Update, method0, method1, GetSyntaxMapByKind(method0, SyntaxKind.ForEachStatement), preserveLocalVariables: true)));

                // Verify delta metadata contains expected rows.
                using (var md1 = diff1.GetMetadata())
                {
                    // 1 field def added & 3 methods updated
                    CheckEncLogDefinitions(md1.Reader,
                        Row(4, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(5, TableIndex.StandAloneSig, EditAndContinueOperation.Default),
                        Row(3, TableIndex.TypeDef, EditAndContinueOperation.AddField),
                        Row(7, TableIndex.Field, EditAndContinueOperation.Default),
                        Row(1, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(5, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(6, TableIndex.MethodDef, EditAndContinueOperation.Default),
                        Row(13, TableIndex.CustomAttribute, EditAndContinueOperation.Default),
                        Row(14, TableIndex.CustomAttribute, EditAndContinueOperation.Default));

                    diff1.VerifyIL("C.<F>d__1.System.Collections.IEnumerator.MoveNext", @"
{
  // Code size      170 (0xaa)
  .maxstack  5
  .locals init (int V_0,
                bool V_1)
  IL_0000:  ldarg.0
  IL_0001:  ldfld      ""int C.<F>d__1.<>1__state""
  IL_0006:  stloc.0
  IL_0007:  ldloc.0
  IL_0008:  brfalse.s  IL_0012
  IL_000a:  br.s       IL_000c
  IL_000c:  ldloc.0
  IL_000d:  ldc.i4.1
  IL_000e:  beq.s      IL_0014
  IL_0010:  br.s       IL_0016
  IL_0012:  br.s       IL_001a
  IL_0014:  br.s       IL_006f
  IL_0016:  ldc.i4.0
  IL_0017:  stloc.1
  IL_0018:  ldloc.1
  IL_0019:  ret
  IL_001a:  ldarg.0
  IL_001b:  ldc.i4.m1
  IL_001c:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0021:  nop
  IL_0022:  nop
  IL_0023:  ldarg.0
  IL_0024:  ldc.i4.1
  IL_0025:  newarr     ""double""
  IL_002a:  dup
  IL_002b:  ldc.i4.0
  IL_002c:  ldc.r8     1
  IL_0035:  stelem.r8
  IL_0036:  stfld      ""double[] C.<F>d__1.<>s__4""
  IL_003b:  ldarg.0
  IL_003c:  ldc.i4.0
  IL_003d:  stfld      ""int C.<F>d__1.<>s__2""
  IL_0042:  br.s       IL_008c
  IL_0044:  ldarg.0
  IL_0045:  ldarg.0
  IL_0046:  ldfld      ""double[] C.<F>d__1.<>s__4""
  IL_004b:  ldarg.0
  IL_004c:  ldfld      ""int C.<F>d__1.<>s__2""
  IL_0051:  ldelem.r8
  IL_0052:  box        ""double""
  IL_0057:  stfld      ""object C.<F>d__1.<item>5__3""
  IL_005c:  nop
  IL_005d:  ldarg.0
  IL_005e:  ldc.i4.1
  IL_005f:  stfld      ""int C.<F>d__1.<>2__current""
  IL_0064:  ldarg.0
  IL_0065:  ldc.i4.1
  IL_0066:  stfld      ""int C.<F>d__1.<>1__state""
  IL_006b:  ldc.i4.1
  IL_006c:  stloc.1
  IL_006d:  br.s       IL_0018
  IL_006f:  ldarg.0
  IL_0070:  ldc.i4.m1
  IL_0071:  stfld      ""int C.<F>d__1.<>1__state""
  IL_0076:  nop
  IL_0077:  ldarg.0
  IL_0078:  ldnull
  IL_0079:  stfld      ""object C.<F>d__1.<item>5__3""
  IL_007e:  ldarg.0
  IL_007f:  ldarg.0
  IL_0080:  ldfld      ""int C.<F>d__1.<>s__2""
  IL_0085:  ldc.i4.1
  IL_0086:  add
  IL_0087:  stfld      ""int C.<F>d__1.<>s__2""
  IL_008c:  ldarg.0
  IL_008d:  ldfld      ""int C.<F>d__1.<>s__2""
  IL_0092:  ldarg.0
  IL_0093:  ldfld      ""double[] C.<F>d__1.<>s__4""
  IL_0098:  ldlen
  IL_0099:  conv.i4
  IL_009a:  blt.s      IL_0044
  IL_009c:  ldarg.0
  IL_009d:  ldnull
  IL_009e:  stfld      ""double[] C.<F>d__1.<>s__4""
  IL_00a3:  ldc.i4.0
  IL_00a4:  stloc.1
  IL_00a5:  br         IL_0018
}
");
                }
            }
        }
    }
}
