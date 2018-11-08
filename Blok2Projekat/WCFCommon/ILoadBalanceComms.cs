using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFCommon
{
    [ServiceContract]
    public interface ILoadBalanceComms
    {
        [OperationContract]
        bool Modify(ModifyType type, string id, string newVersion);
    }
}