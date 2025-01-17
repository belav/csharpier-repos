using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Mono.Debugger.Soft
{
    public struct CustomAttributeNamedArgumentMirror
    {
        CustomAttributeTypedArgumentMirror arg;
        PropertyInfoMirror prop;
        FieldInfoMirror field;

        internal CustomAttributeNamedArgumentMirror(
            PropertyInfoMirror prop,
            FieldInfoMirror field,
            CustomAttributeTypedArgumentMirror arg
        )
        {
            this.arg = arg;
            this.prop = prop;
            this.field = field;
        }

        public PropertyInfoMirror Property
        {
            get { return prop; }
        }

        public FieldInfoMirror Field
        {
            get { return field; }
        }

        public CustomAttributeTypedArgumentMirror TypedValue
        {
            get { return arg; }
        }
    }
}
