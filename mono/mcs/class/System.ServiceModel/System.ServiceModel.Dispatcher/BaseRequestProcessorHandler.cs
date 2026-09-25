using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace System.ServiceModel.Dispatcher
{
    internal abstract class BaseRequestProcessorHandler
    {
        BaseRequestProcessorHandler next;

        public virtual void ProcessRequestChain(MessageProcessingContext mrc)
        {
            if (!ProcessRequest(mrc) && next != null)
            {
                next.ProcessRequestChain(mrc);
            }
        }

        public BaseRequestProcessorHandler Next
        {
            get { return next; }
            set { next = value; }
        }

        protected abstract bool ProcessRequest(MessageProcessingContext mrc);
    }
}
