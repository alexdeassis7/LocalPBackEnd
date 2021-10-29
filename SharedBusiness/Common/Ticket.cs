using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.Common
{
    public class Ticket
    {
        public string CrearTicket(string ticket)
        {
            //int id_ticket;

            DAO.DataAccess.Common.Ticket dbD = new DAO.DataAccess.Common.Ticket();

            ticket = dbD.CrearTicket(ticket);


            return ticket;

        }
    }
}
