using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.Services.Banks.Bind
{
    public class BlDebin
    {

        public int CrearTicket(int ticket)
        {
            //int id_ticket;

            DAO.DataAccess.Services.DbDebin dbD = new DAO.DataAccess.Services.DbDebin();

            ticket = dbD.CrearTicket(ticket);


            return ticket;

        }
    }
}
