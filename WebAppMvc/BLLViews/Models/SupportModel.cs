using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLLViews.Models
{

    public class MyTicketsModel
    {
        public MyTicketsModel()
        {
            tickets = new List<Ticket>();
        }
        public ICollection<Ticket> tickets { get; set; }
        public string uid { get; set; }
        public bool IsSupport { get; set; }
    }
    public class TicketsModel
    {
        public TicketsModel()
        {
            tickets = new List<Ticket>();
        }
        public ICollection<Ticket> tickets { get; set; }
        public string uid { get; set; }
    }
    public class AdmTicketModel
    {
        public AdmTicketModel()
        {
            ticketMSGs = new List<TicketMSG>();
            listStatus = new List<SelectListItem>();
        }
        public Ticket ticket { get; set; }
        public ICollection<TicketMSG> ticketMSGs { get; set; }
        public TicketMSG mSG { get; set; }
        public string uid { get; set; }
        public string newStatus { get; set; }
        public ICollection<SelectListItem> listStatus { get; set; }
    }


    public class NewTicketModel
    {
        public Ticket ticket { get; set; }
        public TicketMSG mSG { get; set; }
        public string uid { get; set; }
    }

    public class TicketModel
    {
        public TicketModel()
        {
            ticketMSGs = new List<TicketMSG>();
        }
        public Ticket ticket { get; set; }
        public ICollection<TicketMSG> ticketMSGs { get; set; }
        public TicketMSG mSG { get; set; }
        public string uid { get; set; }
    }
    public class BanControlModels
    {
        [Required(ErrorMessage = "IdOrEmail is Required")]
        public string IdOrEmail { get; set; }
        public bool BanOrUnban { get; set; }
        public BansList Ban { get; set; }
        [Required(ErrorMessage = "CountHour is Required")]
        public int CountHour { get; set; }
    }

}