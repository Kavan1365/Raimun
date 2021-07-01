using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Hangfire
{
    public static class MyJob
    {
        public static  void SendMyJobForEmail(string Temp)
            {
            Console.Write("Ok send job: "+ Temp);
}
    }
}
