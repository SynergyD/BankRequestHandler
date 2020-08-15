using System;
using System.Collections.Generic;
using RequestHandler.DataAccess.Entities;

namespace RequestHandler.DataAccess.Repositories
{
    public interface IRequestRepository
    {
        int Save(Request request);

        RequestDto Get(int requestId);
        
        RequestDto GetByClient(RequestByClient requestByClient);
    }
}