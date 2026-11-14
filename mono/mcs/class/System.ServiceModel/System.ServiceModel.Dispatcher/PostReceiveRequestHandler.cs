using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ServiceModel.Dispatcher
{
    internal class PostReceiveRequestHandler : BaseRequestProcessorHandler
    {
        protected override bool ProcessRequest(MessageProcessingContext mrc)
        {
            EnsureInstanceContextOpen(mrc.InstanceContext);
            AfterReceiveRequest(mrc);
            return false;
        }

        void AfterReceiveRequest(MessageProcessingContext mrc)
        {
            mrc.EventsHandler.AfterReceiveRequest();
        }

        void EnsureInstanceContextOpen(InstanceContext ictx)
        {
            if (ictx.State != CommunicationState.Opened)
                ictx.Open();
        }
    }
}
