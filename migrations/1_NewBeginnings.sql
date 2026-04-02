use CloudSpritzers
go


drop table if exists FAQEntry
drop table if exists [Message] 
drop table if exists Chat
drop table if exists Sender
drop table if exists Review
drop table if exists Ticket
drop table if exists TicketSubcategory
drop table if exists TicketCategory
drop table if exists [User]
drop table if exists Employee
drop table if exists FAQOption
drop table if exists FAQNode


create table Employee(
	employee_id int identity(1,1),
	constraint PK_Employee primary key (employee_id),
	[name] nvarchar(255),
	email nvarchar(255),
	[group] nvarchar(100)
)

create table [User](
	[user_id] int identity(1,1),
	constraint PK_User primary key ([user_id]),
	[name] nvarchar(255),
	[email] nvarchar(255)
)

create table TicketCategory(
	category_id int identity(1,1),
	constraint PK_TicketCategory primary key (category_id),
	[name] nvarchar(255)
)

create table TicketSubcategory(
	subcategory_id int identity(1,1),
	constraint PK_TicketSubcategory primary key (subcategory_id),
	[name] nvarchar(255),
	external_id int,
	category_id int,
	constraint FK_TicketSubcategory_TicketCategory foreign key (category_id) references TicketCategory(category_id),
)

create table Ticket(
	ticket_id int identity(1,1),
	constraint PK_Ticket primary key (ticket_id),
	[user_id] int,
	constraint FK_Ticket_User foreign key ([user_id]) references [User]([user_id]),
	urgency_level nvarchar(100),
	[status] nvarchar(100),
	[subject] nvarchar(255),
	[description] nvarchar(MAX),
	category_id int,
	constraint FK_Ticket_TicketCategory foreign key (category_id) references TicketCategory(category_id),
	subcategory_id int,
	constraint FK_Ticket_TicketSubcategory foreign key (subcategory_id) references TicketSubcategory(subcategory_id),
	created_at datetime, 
	employee_id int,
	constraint FK_Ticket_Employee foreign key (employee_id) references Employee(employee_id)
)

create table Review(
	review_id int identity(1,1),
	constraint PK_Review primary key (review_id),
	[user_id] int,
	constraint FK_Review_User foreign key ([user_id]) references [User]([user_id]),
	[message] nvarchar(MAX),
	duty_free_rating int,
	flight_experience_rating int,
	staff_friendliness_rating int,
	cleanliness_rating int,
	constraint CHK_Review_StarInterval check (
		(duty_free_rating > 0 and duty_free_rating <= 5) and
		(flight_experience_rating > 0 and flight_experience_rating <= 5) and
		(staff_friendliness_rating > 0 and staff_friendliness_rating <= 5) and
		(cleanliness_rating > 0 and cleanliness_rating <= 5)
	)
)


create table Chat(
	chat_id int identity(1,1),
	constraint PK_Chat primary key (chat_id),
	[user_id] int,
	constraint FK_Chat_User foreign key ([user_id]) references [User]([user_id]),
	employee_id int,
	constraint FK_Chat_Employee foreign key (employee_id) references Employee(employee_id),
	[status] nvarchar(100)
)

create table [Message](
	message_id int identity(1,1),
	constraint PK_Message primary key (message_id),
	sender_id int,
	constraint FK_Message_User foreign key (sender_id) references [User]([user_id]),
	chat_id int,
	constraint FK_Message_Chat foreign key (chat_id) references Chat(chat_id),
	[timestamp] datetime,
	[text] nvarchar(MAX),
	is_read BIT default 0
)

create table FAQEntry(
	FAQentry_id int identity(1,1),
	constraint PK_FAQentry primary key (FAQentry_id),
	question nvarchar(MAX),
	answer nvarchar(MAX),
	category nvarchar(100),
	view_count int default 0,
	was_helpful_votes int default 0,
	was_not_helpful_votes int default 0,
	constraint CHK_FAQEntry_ViewsAndVotesNotNegative check (
		(view_count >= 0) and
		(was_helpful_votes >= 0) and
		(was_not_helpful_votes >= 0)
	)
)

create table FAQNode (
	node_id int identity(1,1),
	constraint PK_Node primary key (node_id),
	question_text nvarchar(255),
	is_final_answer bit
)

create table FAQOption (
	option_id int identity(1,1),
	constraint PK_Option primary key (option_id),
	node_id int,
	constraint FK_ParentNode foreign key (node_id) references FAQNode(node_id),
	[label] nvarchar(255),
	next_option_id int,
	constraint FK_NextNode foreign key (next_option_id) references FAQNode(node_id),
)