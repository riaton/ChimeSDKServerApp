// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ValidationRules.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace ChimeAppTest {

  /// <summary>Holder for reflection information generated from ValidationRules.proto</summary>
  public static partial class ValidationRulesReflection {

    #region Descriptor
    /// <summary>File descriptor for ValidationRules.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ValidationRulesReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVWYWxpZGF0aW9uUnVsZXMucHJvdG8SDENoaW1lQXBwVGVzdBogZ29vZ2xl",
            "L3Byb3RvYnVmL2Rlc2NyaXB0b3IucHJvdG8aHmdvb2dsZS9wcm90b2J1Zi93",
            "cmFwcGVycy5wcm90bzpMCgZNYXh2YWwSHS5nb29nbGUucHJvdG9idWYuRmll",
            "bGRPcHRpb25zGNGGAyABKAsyGy5nb29nbGUucHJvdG9idWYuSW50MzJWYWx1",
            "ZTpMCgZNaW52YWwSHS5nb29nbGUucHJvdG9idWYuRmllbGRPcHRpb25zGNKG",
            "AyABKAsyGy5nb29nbGUucHJvdG9idWYuSW50MzJWYWx1ZTpNCghSZXF1aXJl",
            "ZBIdLmdvb2dsZS5wcm90b2J1Zi5GaWVsZE9wdGlvbnMY04YDIAEoCzIaLmdv",
            "b2dsZS5wcm90b2J1Zi5Cb29sVmFsdWU6TAoGU3RybGVuEh0uZ29vZ2xlLnBy",
            "b3RvYnVmLkZpZWxkT3B0aW9ucxjUhgMgASgLMhsuZ29vZ2xlLnByb3RvYnVm",
            "LkludDMyVmFsdWU6TAoFUmVnZXgSHS5nb29nbGUucHJvdG9idWYuRmllbGRP",
            "cHRpb25zGNWGAyABKAsyHC5nb29nbGUucHJvdG9idWYuU3RyaW5nVmFsdWVi",
            "BnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.Reflection.DescriptorReflection.Descriptor, global::Google.Protobuf.WellKnownTypes.WrappersReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pb::Extension[] { ValidationRulesExtensions.Maxval, ValidationRulesExtensions.Minval, ValidationRulesExtensions.Required, ValidationRulesExtensions.Strlen, ValidationRulesExtensions.Regex }, null));
    }
    #endregion

  }
  /// <summary>Holder for extension identifiers generated from the top level of ValidationRules.proto</summary>
  public static partial class ValidationRulesExtensions {
    public static readonly pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, int?> Maxval =
      new pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, int?>(50001, pb::FieldCodec.ForStructWrapper<int>(400010));
    public static readonly pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, int?> Minval =
      new pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, int?>(50002, pb::FieldCodec.ForStructWrapper<int>(400018));
    public static readonly pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, bool?> Required =
      new pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, bool?>(50003, pb::FieldCodec.ForStructWrapper<bool>(400026));
    public static readonly pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, int?> Strlen =
      new pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, int?>(50004, pb::FieldCodec.ForStructWrapper<int>(400034));
    public static readonly pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, string> Regex =
      new pb::Extension<global::Google.Protobuf.Reflection.FieldOptions, string>(50005, pb::FieldCodec.ForClassWrapper<string>(400042));
  }

}

#endregion Designer generated code
