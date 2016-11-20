using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSpace
{
    class Services
    {
        public int locId;
        public string service;
        public Services() { locId = -1; }
        public Services(int lid, string serv)
        {
            locId = lid;
            service = serv;
        } 
    }
}
