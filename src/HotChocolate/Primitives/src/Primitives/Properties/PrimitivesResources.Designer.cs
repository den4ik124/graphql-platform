﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotChocolate.Properties {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class PrimitivesResources {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PrimitivesResources() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("HotChocolate.Properties.PrimitivesResources", typeof(PrimitivesResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string NameUtils_InvalidGraphQLName {
            get {
                return ResourceManager.GetString("NameUtils_InvalidGraphQLName", resourceCulture);
            }
        }
        
        internal static string ThrowHelper_SchemaCoordinate_ArgumentNameCannotBeSetWithoutMemberName {
            get {
                return ResourceManager.GetString("ThrowHelper_SchemaCoordinate_ArgumentNameCannotBeSetWithoutMemberName", resourceCulture);
            }
        }
        
        internal static string ThrowHelper_SchemaCoordinate_MemberNameCannotBeSetOnADirectiveCoordinate {
            get {
                return ResourceManager.GetString("ThrowHelper_SchemaCoordinate_MemberNameCannotBeSetOnADirectiveCoordinate", resourceCulture);
            }
        }
    }
}
