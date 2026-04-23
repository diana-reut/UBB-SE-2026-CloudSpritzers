using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryTicketRepository : ITicketRepository
    {
        private List<Ticket> _tickets;

        public InMemoryTicketRepository()
        {
            _tickets = new List<Ticket>();
        }

        public int CreateNewEntity(Ticket ticketEntity)
        {
            if (ticketEntity == null)
                throw new ArgumentNullException(nameof(ticketEntity), "Ticket can't be null.");

            
            if (_tickets.Any(t => t.TicketId == ticketEntity.TicketId))
                throw new ArgumentException("Ticket already exists.");

            _tickets.Add(ticketEntity);
            return ticketEntity.TicketId;
        }

        public Ticket GetById(int id)
        {
            var ticket = _tickets.FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
                throw new KeyNotFoundException($"Ticket with TicketId {id} was not found.");

            return ticket;
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _tickets;
        }

        public void UpdateById(int id, Ticket newTicket)
        {
            var index = _tickets.FindIndex(t => t.TicketId == id);
            if (index == -1)
                throw new KeyNotFoundException($"Ticket with ID {id} not found.");
            _tickets[index] = newTicket;
            //this approach simply cause i don't have setters in the Ticket class,
            //if i had them i would have updated the existing ticket instead of replacing it with a new one

        }

        public void DeleteById(int id)
        {
            var ticketToRemove = _tickets.FirstOrDefault(t => t.TicketId == id);
            if (ticketToRemove == null)
                throw new KeyNotFoundException($"Ticket with ID {id} not found.");

            _tickets.Remove(ticketToRemove);
        }
    }
}