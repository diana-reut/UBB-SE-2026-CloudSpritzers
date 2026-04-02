alter table TicketCategory
add urgency_level varchar(100) default 'LOW'

insert into TicketCategory ([name]) values ('Other')

update TicketCategory
set urgency_level = 'LOW'
where [name] = 'Duty Free'

update TicketCategory
set urgency_level = 'HIGH'
where [name] = 'Flight Change'

update TicketCategory
set urgency_level = 'MEDIUM'
where [name] = 'Baggage' or [name]='Refunds' or [name]='Other'

select * from TicketCategory