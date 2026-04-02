use CloudSpritzers
go

INSERT INTO Employee ([name], email, [group]) VALUES
('Alice Smith', 'alice@airport.com', 'ADMIN'),
('Bob Jones', 'bob@airport.com', 'HR'),
('Charlie Brown', 'charlie@airport.com', 'CASHBACK');
 
 
INSERT INTO [User] ([name], [email]) VALUES 
('John Doe', 'john.doe@gmail.com'),
('Jane Doe', 'jane.doe@yahoo.com'),
('Mark Spector', 'mark.s@outlook.com');
 
-- Add the bot :)

SET IDENTITY_INSERT Employee ON;

INSERT INTO Employee ([employee_id], [name], email, [group]) VALUES
(0, 'Carlos', 'customer-support@cloudspritzers.com', 'ADMIN');

SET IDENTITY_INSERT Employee OFF;
 

INSERT INTO TicketCategory ([name]) VALUES ('Baggage'), ('Flight Change'), ('Refunds'), ('Duty Free');
 
INSERT INTO TicketSubcategory ([name], category_id, external_id) VALUES 
('Lost Suitcase', 1, 0), 
('Damaged Item', 1, 0),
('Date Change', 2, 0),
('Full Refund Request', 3, 0),
('Sephora', 4, 0),
('Animax', 4, 0),
('Kaufland', 4, 0),
('Carturesti', 4, 0),
('Lidl', 4, 0);
 
 
INSERT INTO Ticket ([user_id], urgency_level, [status], [subject], [description], category_id, subcategory_id, created_at, employee_id)
VALUES (1, 'HIGH', 'OPEN', 'Lost Gold Suitcase', 'My suitcase didn''t arrive on flight RO123.', 1, 1, GETDATE(), 1),
(2, 'LOW', 'RESOLVED', 'Stumbeled on red carpet', 'Fell due to the carpet layed at the entrance.', 4, 5, GETDATE(), 1),
(3, 'MEDIUM', 'IN_PROGRESS', 'Flight RO123 got delayed.', 'It is unacceptable that you can not control the weather.', 2, 2, GETDATE(), 1);
 
INSERT INTO Chat ([user_id], employee_id, [status]) VALUES (1, 0, 'Active');
 
INSERT INTO [Message] (sender_id, chat_id, [timestamp], [text], is_read) 
VALUES (2, 1, GETDATE(), 'Hello, I need help with my lost bag.', 1);
 
INSERT INTO Review ([user_id], [message], duty_free_rating, flight_experience_rating, staff_friendliness_rating, cleanliness_rating)
VALUES (1, 'Great service, but the airport was a bit messy.', 4, 5, 5, 3),
(2, 'Best airport ever. (I am held at gunpoint)', 5, 5, 5, 5);
 

 
INSERT INTO FAQEntry (question, answer, category, view_count, was_helpful_votes)
VALUES ('What is the liquid limit?', 'The limit is 100ml per container.', 'All', 150, 45),
('How big can my carry-on luggage be?', 'The limit for the luggage depends on the ticket bought and the flight provider.', 'Tickets', 200, 30),
('How early should I arrive at the airport?', 'We recommend arriving at least 3 hours before international flights and 2 hours for domestic flights.', 'CheckIn', 450, 120),
('Can I bring my pet on the flight?', 'Pet policies vary by airline. Generally, small pets can travel in the cabin in a carrier, while larger ones must go in the cargo hold.', 'Facilities', 85, 20),
('Where is the lost and found office?', 'The lost and found office is located on the ground floor of Terminal B, near the arrivals exit.', 'Baggage', 310, 88),
('Do you have free Wi-Fi?', 'Yes, select "Airport_Free_WiFi" from your settings. No password is required.', 'Facilities', 900, 450  ),
('What items are prohibited in checked baggage?', 'Explosives, flammable liquids, and loose lithium batteries are strictly prohibited.', 'Baggage', 125, 60),
('Is there a prayer room in the airport?', 'Yes, multi-faith prayer rooms are located in Terminals A and C after security.', 'Facilities', 45, 15);
 
 
---------------------------- FAQ Bot is falling down, falling down, falling down

SET IDENTITY_INSERT FAQNode ON;

INSERT INTO FAQNode (node_id, question_text, is_final_answer) VALUES 
(1, 'How can I help you today?', 0),
(2, 'What is the issue with your baggage?', 0),
(3, 'Check your email for a tracking link or visit the lost & found desk.', 1),
(4, 'Please file a "Property Irregularity Report" at the arrival hall.', 1),
(5, 'What would you like to do with your booking?', 0),
(6, 'You can change your flight via the "My Bookings" section on our website.', 1);

SET IDENTITY_INSERT FAQNode OFF;
 
INSERT INTO FAQOption (node_id, [label], next_option_id) VALUES 
(1, 'Baggage Issues', 2),    -- From Node 1 to Node 2
(1, 'Manage Booking', 5),    -- From Node 1 to Node 5
(2, 'Lost Baggage', 3),      -- From Node 2 to Node 3 (Final)
(2, 'Damaged Baggage', 4),   -- From Node 2 to Node 4 (Final)
(5, 'Change Flight Date', 6);-- From Node 5 to Node 6 (Final)
 
 
--- Even more data

SET IDENTITY_INSERT FAQNode ON;

INSERT INTO FAQNode (node_id, question_text, is_final_answer) VALUES 
(7, 'How would you like to get to the airport?', 0), -- New Question Node
(8, 'The taxi stand is located right outside Arrivals Gate 3.', 1), -- Final Answer
(9, 'Short-term parking is in Lot P1; Long-term is in Lot P4 (shuttle available).', 1), -- Final Answer
(10, 'The airport shuttle bus runs every 15 minutes from the city center.', 1); -- Final Answer

SET IDENTITY_INSERT FAQNode OFF;
 
INSERT INTO FAQOption (node_id, [label], next_option_id) VALUES 
(1, 'Parking & Transport', 7),  -- Connects Main Menu to the Transport Question
(7, 'By Taxi', 8),              -- Path to Taxi info
(7, 'By Personal Car', 9),      -- Path to Parking info
(7, 'By Public Bus', 10);       -- Path to Shuttle info
 