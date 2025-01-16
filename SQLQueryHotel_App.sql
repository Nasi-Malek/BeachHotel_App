--Hämta alla rum

SELECT * 
FROM Rooms;



--Hämta endast RoomNumber och Type från tabellen Rooms:

SELECT RoomNumber, Type 
FROM Rooms;



--Hämta alla rum som är tillgängliga (IsAvailable = 1):

SELECT * 
FROM Rooms
WHERE IsAvailable = 1;



--Hämta alla bokningar för en specifik kund

SELECT * 
FROM Bookings
WHERE GuestId = 2;



--Sortera rum efter Type

SELECT * 
FROM Rooms
ORDER BY Type;




--Sortera kunder efter efternamn (Name) i fallande ordning:

SELECT * 
FROM Guests
ORDER BY Name DESC;




--Hämta alla bokningar för rum som är tillgängliga och sortera efter CheckInDate:

SELECT BookingId, RoomNumber, CheckInDate, CheckOutDate 
FROM Bookings
WHERE RoomNumber IN (SELECT RoomNumber FROM Rooms WHERE IsAvailable = 1)
ORDER BY CheckInDate;




--Hämta alla bokningar och inkludera information om rum och kunder:

SELECT b.BookingId, b.CheckInDate, b.CheckOutDate, r.Type, c.Name
FROM Bookings b
INNER JOIN Rooms r ON b.RoomNumber = r.RoomNumber
INNER JOIN Guests c ON b.GuestId = c.GuestId
ORDER BY b.CheckInDate;





--Använd en gruppering med GROUP BY för att räkna bokningar per rum:

SELECT RoomNumber, COUNT(*) AS TotalBookings
FROM Bookings
GROUP BY RoomNumber
ORDER BY TotalBookings DESC;




--Filtrera bokningar med ett tidsintervall:

SELECT * 
FROM Bookings
WHERE CheckInDate >= '2024-12-01'
  AND CheckOutDate <= '2025-01-31';




--Använd en LEFT JOIN för att visa alla rum och deras bokningar:

SELECT r.RoomNumber, r.Type, r.IsAvailable, b.BookingId, b.CheckInDate, b.CheckOutDate
FROM Rooms r
LEFT JOIN Bookings b ON r.RoomNumber = b.RoomNumber
ORDER BY r.Type;




--Använd en LEFT JOIN med filtrering av null-värden:

SELECT c.GuestId, c.Name
FROM Guests c
LEFT JOIN Bookings b ON c.GuestId = b.GuestId
WHERE b.BookingId IS NULL;

