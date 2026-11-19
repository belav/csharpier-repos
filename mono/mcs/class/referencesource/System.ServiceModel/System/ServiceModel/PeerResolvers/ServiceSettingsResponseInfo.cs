//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace System.ServiceModel.PeerResolvers
{
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    [MessageContract(IsWrapped = false)]
    public class ServiceSettingsResponseInfo
    {
        [DataContract(Name = "ServiceSettingsResponseInfo", Namespace = PeerStrings.Namespace)]
        class ServiceSettingsResponseInfoDC
        {
            [DataMember(Name = "ControlMeshShape")]
            public bool ControlMeshShape;

            public ServiceSettingsResponseInfoDC(bool control)
            {
                ControlMeshShape = control;
            }
        }

        public ServiceSettingsResponseInfo()
            : this(false) { }

        public ServiceSettingsResponseInfo(bool control)
        {
            this.body = new ServiceSettingsResponseInfoDC(control);
        }

        [MessageBodyMember(Name = "ServiceSettings", Namespace = PeerStrings.Namespace)]
        ServiceSettingsResponseInfoDC body;

        public bool ControlMeshShape
        {
            get { return body.ControlMeshShape; }
            set { body.ControlMeshShape = value; }
        }

        public bool HasBody()
        {
            return body != null;
        }
    }
}
